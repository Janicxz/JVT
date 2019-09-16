using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace JVT
{
    class ClipEncoder
    {
        public delegate void EncoderEventHandler(int ClipsEncoded, bool Finished);
        public event EncoderEventHandler EncodingStatusChanged;
        public void Encode(List<VideoClip> clips)
        {
            //Console.WriteLine("Starting encode");
            int clipNum = 1;
            foreach (VideoClip clip in clips)
            {
                string outputFolder = Application.StartupPath + "\\render\\";
                MediaFile inputFile = new MediaFile { Filename = clip.filePath };
                MediaFile outputFile = new MediaFile { Filename = outputFolder + clip.OutputName };

                TimeSpan clipLen = clip.End - clip.Start;
                Console.WriteLine("Input: {0}, output: {1}", inputFile.Filename, outputFile.Filename);
                using (Engine engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                    ConversionOptions options = new ConversionOptions();
                    options.CutMedia(clip.Start, clipLen);
                    Console.WriteLine("start at: {0}, end at {1}, duration: {2}", clip.Start, clip.End, clipLen);

                    engine.ConvertProgressEvent += Engine_ConvertProgressEvent;
                    engine.ConversionCompleteEvent += Engine_ConversionCompleteEvent;
                    engine.Convert(inputFile, outputFile, options);
                    EncodingStatusChanged(clipNum, false);
                    clipNum++;
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
