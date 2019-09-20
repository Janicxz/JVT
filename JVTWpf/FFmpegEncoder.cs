using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVTWpf
{
    class FFmpegEncoder
    {
        private List<VideoClip> videoClips;
        private string FFmpegPath = Environment.CurrentDirectory + @"\ffmpeg";
        public FFmpegEncoder(List<VideoClip> clipsToEncode)
        {
            videoClips = clipsToEncode;
        }

        public void Encode(int videoWidth, int videoHeight, int videoBitrate, int videoFramerate, bool hwAccel)
        {
            // FIXME: finish encoder, add encoding status events.
            // Encode all the clips
            bool mergeClips = false;
            string outputFolder = Environment.CurrentDirectory + "\\videos\\";
            string mergeCommand = "";
            int mergeNum = 0;

            //if (hwAccel)
            //    mergeCommand += "-vsync 0 -hwaccel cuvid -c:v h264_cuvid ";

            foreach (VideoClip clip in videoClips)
            {
                string inputFilename = clip.filePath;
                string outputFilename = outputFolder + clip.OutputName;

                if (clip.Merge)
                {
                    mergeCommand += string.Format("-i \"{0}\" ", outputFilename);
                    mergeNum++;
                    mergeClips = true;
                }

                float clipVolume = (float)clip.Volume / 100;
                if (clip.MergeAudioTracks)
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
                {
                    string ffmpegCommand = "";
                    if (hwAccel)
                        ffmpegCommand += string.Format("-vsync 0 -hwaccel cuvid -c:v h264_cuvid -resize {0}x{1} ", videoWidth, videoHeight);

                    // build command string
                    ffmpegCommand += string.Format("-ss {1} -i \"{0}\" ", inputFilename, clip.Start);
                    if (hwAccel)
                        ffmpegCommand += "-c:v h264_nvenc ";
                    else
                        ffmpegCommand += string.Format("-c:v libx264 -preset medium -vf scale={0}x{1},setsar=1 ", videoWidth, videoHeight);
                    ffmpegCommand += string.Format("-b:v {1}K -maxrate {1}K -framerate {4} ", inputFilename, videoBitrate, videoWidth, videoHeight, videoFramerate, clip.Start);


                    if (clipVolume != 1.0) // adjust volume only if necessary
                        ffmpegCommand += String.Format("-filter:a \"volume={0}\" ", clipVolume.ToString(CultureInfo.InvariantCulture));
                    ffmpegCommand += String.Format("-ac 2 -c:a aac -b:a 384k -t {0} \"{1}\"", clip.End - clip.Start, outputFilename);
                    if(clip.Encode)
                    {
                        Console.WriteLine("Running ffmpeg with cmd: " + ffmpegCommand);
                        ffmpegCommandExecute(ffmpegCommand);
                    }
                }
            }
            if(mergeClips)
            {
                // Merge all the selected clips together
                string mergeFilename = "clips_merged.mp4";
                // build command string
                mergeCommand += string.Format("-filter_complex concat=n={0}:v=1:a=1 -f mp4 \"{1}\"", mergeNum, outputFolder + mergeFilename);
                Console.WriteLine("Running ffmpeg with merge cmd: " + mergeCommand);
                ffmpegCommandExecute(mergeCommand);
            }
            Console.WriteLine("Encoding finished.");
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
