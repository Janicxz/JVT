using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVTWpf
{
    public class EncoderProgressEventArgs : EventArgs
    {
        /// <summary>
        /// The number of clips encoded so far.
        /// </summary>
        public int ClipsEncoded { get; set; }
        /// <summary>
        /// The number of clips to encode
        /// </summary>
        public int ClipsTotal { get; set; }
        /// <summary>
        /// The progress of current encoding job (0-100).
        /// </summary>
        public int CurrentClipProcess { get; set; }
    }

    class FFmpegEncoder
    {
        private ObservableCollection<VideoClip> videoClips;
        private string FFmpegPath = Environment.CurrentDirectory + @"\ffmpeg";
        public event EventHandler OnEncodingFinished = delegate { };
        public event EventHandler<EncoderProgressEventArgs> OnEncodingProgress = delegate { };
        public FFmpegEncoder(ObservableCollection<VideoClip> clipsToEncode)
        {
            videoClips = clipsToEncode;
        }

        public void Encode(int videoWidth, int videoHeight, int videoBitrate, int videoFramerate, bool hwAccel)
        {
            // FIXME: finish encoder, add encoding status events.
            // Encode all the clips
            //bool keepAspectRatio = true;
            bool mergeClips = false;
            string outputFolder = Environment.CurrentDirectory + "\\videos\\";
            string mergeCommand = "";
            string mergeConcatCmd = "";
            int mergeNum = 0;
            bool hwEncOnly = false;

            //if (hwAccel)
            //    mergeCommand += "-vsync 0 -hwaccel cuvid -c:v h264_cuvid ";
            int clipsToEncodeNum = 0;
            int clipsEncoded = 0;
            // We need to get the number of clips to encode for the events.
            foreach(VideoClip clip in videoClips)
            {
                if (clip.Encode)
                    clipsToEncodeNum++;
            }

            foreach (VideoClip clip in videoClips)
            {
                string inputFilename = clip.filePath;
                string outputFilename = outputFolder + clip.OutputName;
                string forceOverwrite = clip.forceOverwrite ? "-y" : "-n";
                string hwDecoder = "-vsync 0 -hwaccel cuvid -c:v h264_cuvid ";

                switch (clip.inputFileCodec)
                {
                 /*   case "h264":
                        hwDecoder = "-vsync 0 -hwaccel cuvid -c:v h264_cuvid ";
                        break;
                      case "vp8":
                        hwDecoder = "-vsync 0 -hwaccel cuvid -c:v vp8_cuvid ";
                        break;*/
                    default:
                        // DXVA2 doesn't support -resize? so we have to use software cmd for rest, and then hw encode at end.
                        hwDecoder = " ";
                        hwEncOnly = true;
                        Console.WriteLine("using only nvencoder, nvdec disabled");
                        break;
                }

                if (clip.Merge)
                {
                    // Decode each input file on the gpu decoder or else gpu encoder fails.
                    /*if (hwAccel)
                    {
                        // on clip files we have hardcoded output to h264 so we can hardcode input here to h264
                        mergeCommand += "-vsync 0 -hwaccel cuvid -c:v h264_cuvid ";
                        mergeCommand += string.Format("-resize {0}x{1} ", videoWidth, videoHeight);
                    }*/

                    mergeCommand += string.Format("-i \"{0}\" ", outputFilename);
                    // add silent audio track if no audio for merging.
                    mergeConcatCmd += String.Format("[{0}:v:0]", mergeNum); 
                    if (!clip.HasAudio)
                    {
                       // mergeConcatCmd += String.Format("[{0}:v:0]", mergeNum);
                        mergeConcatCmd += String.Format("[{0}:a]", videoClips.Count); // last stream will be our silent track
                    }
                    else
                    {
                        mergeConcatCmd += String.Format("[{0}:a:0]", mergeNum);
                    }
                    mergeNum++;
                    mergeClips = true;
                }

                float clipVolume = (float)clip.Volume / 100;
                /*if (clip.MergeAudioTracks) // Dead code, replaced by the command string builder.
                {
                    // Merge mic and game audio tracks
                    if(clip.Encode)
                    {
                        // string.Format("-ss {5} -i \"{0}\" -filter_complex \"[0:a:0]volume={8}[a1];[0:a:1][a1]amerge=inputs=2[a]\" -map 0:v:0 -map \"[a]\" -c:v libx264 -preset medium -maxrate {1}K -vf scale={2}x{3},setsar=1 -framerate {4} -ac 2 -c:a aac -b:a 384k -t {6} \"{7}\"", inputFilename, videoBitrate, videoWidth, videoHeight, videoFramerate,clip.Start,clip.End-clip.Start, outputFilename, clipVolume.ToString(CultureInfo.InvariantCulture));

                        string ffmpegCommand = "";
                        if (hwAccel)
                            ffmpegCommand += string.Format("-vsync 0 -hwaccel cuvid -c:v h264_cuvid -resize {0}x{1} ", videoWidth, videoHeight);
                        // Build command string
                        ffmpegCommand += string.Format("-ss {1} -i \"{0}\" -filter_complex \"[0:a:0]volume={2}[a1];[0:a:1][a1]amerge=inputs=2[a]\" -map 0:v:0 -map \"[a]\" ", inputFilename,clip.Start, clipVolume.ToString(CultureInfo.InvariantCulture));
                        if (hwAccel)
                            ffmpegCommand += "-c:v h264_nvenc ";
                        else
                            ffmpegCommand += string.Format("-c:v libx264 -preset medium -vf scale={0}x{1},setsar=1 ", videoWidth, videoHeight);

                        ffmpegCommand += string.Format("-b:v {1}K -maxrate {1}K -framerate {4} -ac 2 -c:a aac -b:a 384k -t {6} \"{7}\"", inputFilename, videoBitrate, videoWidth, videoHeight, videoFramerate, clip.Start, clip.End - clip.Start, outputFilename, clipVolume.ToString(CultureInfo.InvariantCulture));
                        // execute it
                        Console.WriteLine("Running ffmpeg with merge audio cmd: " + ffmpegCommand);
                        ffmpegCommandExecute(ffmpegCommand);
                    }
                }
                else
                {*/
                string ffmpegCommand = "";
                // set hw decoder
                if (hwAccel && !hwEncOnly)
                {
                    ffmpegCommand += hwDecoder;
                    ffmpegCommand += string.Format("-resize {0}x{1} ", videoWidth, videoHeight);
                }

                // build command string
                ffmpegCommand += string.Format("-ss {1} -i \"{0}\" ", inputFilename, clip.Start);
                if (clip.MergeAudioTracks && clip.HasAudio)
                    ffmpegCommand += string.Format("-filter_complex \"[0:a:0]volume={0}[a1];[0:a:1][a1]amerge=inputs=2[a]\" -map 0:v:0 -map \"[a]\" ", clipVolume.ToString(CultureInfo.InvariantCulture));

                // Hardcode encoder output to h264 mp4 for now.
                if (hwAccel && !hwEncOnly)
                    ffmpegCommand += "-c:v h264_nvenc ";
                else if (hwEncOnly)
                {
                    // We did not scale the video before the input here
                    ffmpegCommand += string.Format("-c:v h264_nvenc -vf scale={0}x{1}:force_original_aspect_ratio=decrease,pad={0}:{1}:(ow-iw)/2:(oh-ih)/2,setsar=1 ", videoWidth, videoHeight);
                }
                else
                    ffmpegCommand += string.Format("-c:v libx264 -preset medium -vf scale={0}x{1}:force_original_aspect_ratio=decrease,pad={0}:{1}:(ow-iw)/2:(oh-ih)/2,setsar=1 ", videoWidth, videoHeight);
                ffmpegCommand += string.Format("-b:v {1}K -maxrate {1}K -framerate {4} ", inputFilename, videoBitrate, videoWidth, videoHeight, videoFramerate, clip.Start);


                if (clipVolume != 1.0 && !clip.MergeAudioTracks && clip.HasAudio) // adjust volume only if necessary and only if we didn't already adjust it in audio merge.
                    ffmpegCommand += String.Format("-filter:a \"volume={0}\" ", clipVolume.ToString(CultureInfo.InvariantCulture));
                ffmpegCommand += String.Format("-ac 2 -c:a aac -b:a 384k -t {0}  {2} \"{1}\"", clip.End - clip.Start, outputFilename, forceOverwrite);
                if(clip.Encode)
                {
                    Console.WriteLine("Running ffmpeg with cmd: " + ffmpegCommand);
                    OnEncodingProgress(this, new EncoderProgressEventArgs { ClipsEncoded = clipsEncoded, ClipsTotal = clipsToEncodeNum, CurrentClipProcess = 0 });
                    ffmpegCommandExecute(ffmpegCommand);
                    clipsEncoded++;
                    OnEncodingProgress(this, new EncoderProgressEventArgs {ClipsEncoded = clipsEncoded, ClipsTotal = clipsToEncodeNum, CurrentClipProcess = 100 });
                }
                //}
            }
            if(mergeClips)
            {

                // Merge all the selected clips together
                string mergeFilename = "clips_merged.mp4";

                mergeCommand += "-f lavfi -t 0.1 -i anullsrc "; // add silent track for no audio files

                if (hwAccel)
                    mergeCommand += string.Format("-c:v h264_nvenc ");
                else
                    mergeCommand += string.Format("-c:v libx264 -preset medium ");

                mergeCommand += string.Format("-b:v {0}K -maxrate {0}K -framerate {1} ", videoBitrate,videoFramerate);
                // Always override file for merge clip
                // ffmpeg -i "G:\Programming\repos\JVT\JVTWpf\bin\Debug\videos\1563902695388.mp4" -i "G:\Programming\repos\JVT\JVTWpf\bin\Debug\videos\1566147609312.mp4" -i "G:\Programming\repos\JVT\JVTWpf\bin\Debug\videos\1566100824189.mp4" -f lavfi -t 0.1 -i anullsrc -filter_complex "[0:v:0][3:a][1:v:0][3:a][2:v:0][3:a]concat=n=3:v=1:a=1[outv][outa]" -map "[outv]" -map "[outa]" -vsync 2 -y "G:\Programming\repos\JVT\JVTWpf\bin\Debug\videos\clips_merged.mp4"
                mergeCommand += string.Format("-filter_complex \"{2}concat=n={0}:v=1:a=1[outv][outa]\" -map \"[outv]\" -map \"[outa]\" -vsync 2 -y \"{1}\"", mergeNum, outputFolder + mergeFilename, mergeConcatCmd);
                // build command string
                Console.WriteLine("Running ffmpeg with merge cmd: " + mergeCommand);
                ffmpegCommandExecute(mergeCommand);
            }
            Console.WriteLine("Encoding finished.");
            OnEncodingFinished(this, EventArgs.Empty);
        }

        private void ffmpegCommandExecute(string command)
        {
            Process ffmpegProcess = new Process();
            Console.WriteLine("Working dir: " + FFmpegPath);
            Console.WriteLine("Args: " + command);
            ffmpegProcess.StartInfo.WorkingDirectory = FFmpegPath;
            ffmpegProcess.StartInfo.FileName = "ffmpeg.exe";
            ffmpegProcess.StartInfo.Arguments = command;
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.CreateNoWindow = true;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.StartInfo.RedirectStandardError = true;
            ffmpegProcess.OutputDataReceived += FfmpegProcess_OutputDataReceived;
            ffmpegProcess.ErrorDataReceived += FfmpegProcess_ErrorDataReceived;
            ffmpegProcess.Start();
            ffmpegProcess.BeginOutputReadLine();
            ffmpegProcess.BeginErrorReadLine();
            /*string output = ffmpegProcess.StandardOutput.ReadToEnd();
            Console.WriteLine("FFMPEG output: " + output);
            string error = ffmpegProcess.StandardError.ReadToEnd();
            Console.WriteLine("FFMPEG ERR: " + error);*/
            ffmpegProcess.WaitForExit();
        }

        private void FfmpegProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("FFmpeg E: " + e.Data);
        }

        private void FfmpegProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("FFmpeg O: " + e.Data);
        }
    }
}
