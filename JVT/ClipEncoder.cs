using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace JVT
{
    class EncoderSettings
    {
        public int Bitrate;
        public int Width;
        public int Height;
        public int FPS;
    }

    class ClipEncoder
    {
        public delegate void EncoderEventHandler(int ClipsEncoded, bool Finished);
        public event EncoderEventHandler EncodingStatusChanged;
        public void Encode(List<VideoClip> clips, EncoderSettings encodeSettings)
        {
            // TODO: Move clip name conflict resolving logic from FORM MAIN to HERE, UNNECESSARY DOUBLE VALUES IN BOTH ATM!!
            // Concat fails when the clips have different resolution. force re-encode at encode stage. Add encoding settings to clipslist
            //Console.WriteLine("Starting encode");
            int clipNum = 1;
            int mergeNum = 0;
            bool mergeClips = false;
            string mergeCommand = "";
            string outputFolder = Application.StartupPath + "\\render\\";

            foreach (VideoClip clip in clips)
            {
                MediaFile inputFile = new MediaFile { Filename = clip.filePath };
                MediaFile outputFile = new MediaFile { Filename = outputFolder + clip.OutputName };

                if(clip.Merge)
                {
                    mergeCommand += string.Format("-i \"{0}\" ", outputFolder + clip.OutputName);
                    mergeNum++;
                    mergeClips = true;
                }

                TimeSpan clipLen = clip.End - clip.Start;
                Console.WriteLine("Input: {0}, output: {1}", inputFile.Filename, outputFile.Filename);
                using (Engine engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                    ConversionOptions options = new ConversionOptions();
                    options.CustomHeight = encodeSettings.Height;
                    options.CustomWidth = encodeSettings.Width;
                    options.VideoBitRate = encodeSettings.Bitrate;
                    options.VideoFps = encodeSettings.FPS;
                    options.VideoSize = VideoSize.Custom;
                    options.CutMedia(clip.Start, clipLen);
                    Console.WriteLine("start at: {0}, end at {1}, duration: {2}", clip.Start, clip.End, clipLen);
                    engine.ConvertProgressEvent += Engine_ConvertProgressEvent;
                    engine.ConversionCompleteEvent += Engine_ConversionCompleteEvent;
                    if (clip.MergeAudioTracks)
                    {
                        float clipVolume = (float)clip.Volume / 100;
                        string ffmpegCommand = string.Format("-i \"{0}\" -filter_complex \"[0:a:0]volume={8}[a1];[0:a:1][a1]amerge=inputs=2[a]\" -map 0:v:0 -map \"[a]\" -c:v libx264 -preset medium -maxrate {1}K -vf scale={2}x{3} -framerate {4} -ac 2 -c:a aac -b:a 384k -ss {5} -t {6} \"{7}\"", inputFile.Filename, encodeSettings.Bitrate, encodeSettings.Width, encodeSettings.Height, encodeSettings.FPS,clip.Start,clip.End, outputFile.Filename, clipVolume.ToString(CultureInfo.InvariantCulture));
                        Console.WriteLine("Merging audio with cmd: " + ffmpegCommand);
                        engine.CustomCommand(ffmpegCommand);
                    }
                    else
                    {
                        engine.Convert(inputFile, outputFile, options);
                    }
                    EncodingStatusChanged(clipNum, false);
                    clipNum++;
                }
            }

            if(mergeClips)
            {
                string mergeFilename = "clips_merged.mp4";
                mergeCommand += string.Format("-filter_complex concat=n={0}:v=1:a=1 -f mp4 \"{1}\"", mergeNum, outputFolder + mergeFilename);
                Console.WriteLine("Merging clips, merge cmd: " + mergeCommand);
                using (Engine engine = new Engine())
                {
                    engine.ConvertProgressEvent += Engine_ConvertProgressEvent;
                    engine.ConversionCompleteEvent += Engine_ConversionCompleteEvent;
                    engine.CustomCommand(mergeCommand);
                }
            }
            EncodingStatusChanged(clipNum, true);
        }
        private void Engine_ConversionCompleteEvent(object sender, ConversionCompleteEventArgs e)
        {

            Console.WriteLine("\n------------\nConversion complete!\n------------");
            Console.WriteLine("Bitrate: {0}", e.Bitrate);
            Console.WriteLine("Fps: {0}", e.Fps);
            Console.WriteLine("Frame: {0}", e.Frame);
            Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Console.WriteLine("SizeKb: {0}", e.SizeKb);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
           /* MessageBox.Show("Conversion complete!\n" +
                "Bitrate: " + e.Bitrate + "\n" +
                "Fps: " + e.Fps + "\n" +
                "SizeKb: " + e.SizeKb + "\n" +
                "TotalDuration: " + e.TotalDuration);*/
        }

        private void Engine_ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            Console.WriteLine("\n------------\nConverting...\n------------");
            Console.WriteLine("Bitrate: {0}", e.Bitrate);
            Console.WriteLine("Fps: {0}", e.Fps);
            Console.WriteLine("Frame: {0}", e.Frame);
            Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Console.WriteLine("SizeKb: {0}", e.SizeKb);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }
    }
}
