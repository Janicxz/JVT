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
        private string FFmpegPath = Environment.CurrentDirectory + @"\ffmpeg\ffmpeg.exe";
        public FFmpegEncoder(List<VideoClip> clipsToEncode)
        {
            videoClips = clipsToEncode;
        }

        public void Encode(int videoWidth, int videoHeight, int videoBitrate, int videoFramerate)
        {
            // FIXME: finish encoder, add encoding status events.
            // Encode all the clips
            bool mergeClips = false;
            string outputFolder = Environment.CurrentDirectory + "\\videos\\";
            string mergeCommand = "";
            int mergeNum = 0;

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
                        // Build command string
                        // string ffmpegCommand = string.Format("-ss {5} -i \"{0}\" -filter_complex \"[0:a:0]volume={8}[a1];[0:a:1][a1]amerge=inputs=2[a]\" -map 0:v:0 -map \"[a]\" -c:v libx264 -preset medium -maxrate {1}K -vf scale={2}x{3},setsar=1 -framerate {4} -ac 2 -c:a aac -b:a 384k -t {6} \"{7}\"", inputFile.Filename, encodeSettings.Bitrate, encodeSettings.Width, encodeSettings.Height, encodeSettings.FPS,clip.Start,clip.End-clip.Start, outputFile.Filename, clipVolume.ToString(CultureInfo.InvariantCulture));
                        // execute it
                    }
                }
                else
                {
                    // build command string
                    //string ffmpegCommand = "";//string.Format("-ss {5} -i \"{0}\" -c:v libx264 -preset medium -maxrate {1}K -vf scale={2}x{3},setsar=1 -framerate {4} ", inputFile.Filename, encodeSettings.Bitrate, encodeSettings.Width, encodeSettings.Height, encodeSettings.FPS, clip.Start);
                    //if(clipVolume != 1.0) // adjust volume only if necessary
                    //    ffmpegCommand += String.Format("-filter:a \"volume={0}\" ", clipVolume.ToString(CultureInfo.InvariantCulture));
                    //ffmpegCommand += String.Format("-ac 2 -c:a aac -b:a 384k -t {0} \"{1}\"", clip.End - clip.Start, outputFile.Filename);
                    if(clip.Encode)
                    {
                        // execute command string
                    }
                }
            }
            if(mergeClips)
            {
                // Merge all the selected clips together
                //string mergeFilename = "clips_merged.mp4";
                // build command string
                //mergeCommand += string.Format("-filter_complex concat=n={0}:v=1:a=1 -f mp4 \"{1}\"", mergeNum, outputFolder + mergeFilename);
                // execute command string
            }

            /*string ffmpegCommand = buildCommandString();
            Process ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.FileName = FFmpegPath;
            ffmpegProcess.StartInfo.Arguments = ffmpegCommand;
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.StartInfo.RedirectStandardError = true;
            ffmpegProcess.Start();
            string output = ffmpegProcess.StandardOutput.ReadToEnd();
            Console.WriteLine("FFMPEG: " + output);
            string error = ffmpegProcess.StandardError.ReadToEnd(); ;
            Console.WriteLine("FFMPEG ERR: " + output);
            ffmpegProcess.WaitForExit();*/
        }

    }
}
