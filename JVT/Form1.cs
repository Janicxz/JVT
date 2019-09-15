using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Mpv.NET.Player;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace JVT
{
    public partial class Form1 : Form
    {
        string dllPath = "lib\\mpv-1.dll";
        string clipPath = "test.mp4";
        TimeSpan clipStart;
        TimeSpan clipEnd;
        MpvPlayer player;
        Timer timerTrackBarUpdate;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Init
            player = new MpvPlayer(panelPlayer.Handle, dllPath);
            player.Load(clipPath);
            player.Loop = true;
            player.Resume();
            player.MediaLoaded += Player_MediaLoaded;
            player.MediaPaused += Player_MediaPaused;
            player.MediaResumed += Player_MediaResumed;

            timerTrackBarUpdate = new Timer();
            timerTrackBarUpdate.Interval = 100;
            timerTrackBarUpdate.Tick += TimerTrackBarUpdate_Tick;
            timerTrackBarUpdate.Start();
        }

        private void Player_MediaResumed(object sender, EventArgs e)
        {
            Console.WriteLine("playback resumed");
           // timerTrackBarUpdate.Start();
        }

        private void Player_MediaPaused(object sender, EventArgs e)
        {
            Console.WriteLine("Playback paused.");
           // timerTrackBarUpdate.Stop();
        }

        private void TimerTrackBarUpdate_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("timer Pos {0}, dur {1}", player.Position, player.Duration);
            trackBarPlayer.Invoke((Action)delegate
            {
                trackBarPlayer.Value = (int)player.Position.TotalMilliseconds;
                //Application.DoEvents();
            });
        }

        private void Player_MediaLoaded(object sender, EventArgs e)
        {
            Console.WriteLine("Loaded media, duration: {0}",player.Duration.TotalMilliseconds);
            trackBarPlayer.Invoke((Action)delegate
            {
                trackBarPlayer.Maximum = (int)player.Duration.TotalMilliseconds;
                trackBarPlayer.Minimum = 0;
            });
            /*hScrollBarPlayer.Invoke((Action)delegate
            {
                hScrollBarPlayer.Maximum = (int)player.Duration.TotalMilliseconds;
                hScrollBarPlayer.Minimum = 0;
            });*/

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
             if(player != null) player.Dispose();
        }

        bool playerPaused = false;
        private void ButtonPlayerStop_Click(object sender, EventArgs e)
        {
            togglePlayerPause();
           // Console.WriteLine("Playing {0}", player.IsPlaying);
           /* if (!playerPaused)
            {
                player.Pause();
                buttonPlayerStop.Text = "Resume";
                playerPaused = true;
            }
            else
            {
                player.Resume();
                buttonPlayerStop.Text = "Pause";
                playerPaused = false;
            }*/
        }

        private void togglePlayerPause()
        {
            if (player.IsPlaying)
            {
                player.Pause();
                buttonPlayerStop.Invoke((Action)delegate
                {
                    buttonPlayerStop.Text = "Resume";
                });
            }
            else
            {
                player.Resume();
                buttonPlayerStop.Invoke((Action)delegate
                {
                    buttonPlayerStop.Text = "Pause";
                });
            }
        }

        private void HScrollBarPlayer_Scroll(object sender, ScrollEventArgs e)
        {
            timerTrackBarUpdate.Stop();
            Console.WriteLine("scrolled");
           // player.SeekAsync(TimeSpan.FromMilliseconds(e.NewValue));
            timerTrackBarUpdate.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Keydown: {0}", e.KeyCode);
            switch (e.KeyCode)
            {
                case Keys.Left:
                    player.PreviousFrame();
                    e.Handled = true;
                    break;
                case Keys.Right:
                    player.NextFrame();
                    e.Handled = true;
                    break;
                case Keys.Space:
                    togglePlayerPause();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void TrackBarPlayer_Scroll(object sender, EventArgs e)
        {
            player.SeekAsync(TimeSpan.FromMilliseconds(trackBarPlayer.Value));
        }

        private void ButtonClipStart_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Clip start attempt: " + player.Position);
            if (clipEnd.TotalMilliseconds == 0)
            {
                Console.WriteLine("ClipEnd not set yet.");
                clipStart = player.Position;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMark1.Left = sliderposx+15;
            }
            if(player.Position > clipEnd)
            {
                Console.WriteLine("ERR: clip start Position is after clip end!!");
            }
            else
            {
                clipStart = player.Position;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMark1.Left = sliderposx + 15;
            }
            Console.WriteLine("Clip start: " + clipStart);
        }

        private void ButtonClipEnd_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Clip end attempt: " + player.Position);
            Console.WriteLine("start {0}, end {1}: ", clipStart, clipEnd);
            if (clipStart.TotalMilliseconds == 0)
            {
                Console.WriteLine("ClipStart not set yet.");
                clipEnd = player.Position;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMarkEnd.Left = sliderposx + 15;
            }
            if (player.Position < clipStart)
            {
                Console.WriteLine("ERR: clip start Position is after clip end!!");
            }
            else
            {
                clipEnd = player.Position;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMarkEnd.Left = sliderposx + 15;
            }
            Console.WriteLine("Clip end: " + clipEnd);
           // labelMark1.Text = String.Format("S: {0}, E: {1}", clipStart, clipEnd);
        }

        private void ButtonRender_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Encoding clip..");
            MediaFile inputFile = new MediaFile { Filename = clipPath };
            MediaFile outputFile = new MediaFile { Filename = "render\\"+ clipPath };
            TimeSpan clipLen = clipEnd - clipStart;
            Console.WriteLine("Input: {0}, output: {1}", inputFile.Filename, outputFile.Filename);
            using (Engine engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                ConversionOptions options = new ConversionOptions();
                options.CutMedia(clipStart, clipLen);
                Console.WriteLine("start at: {0}, end at {1}, duration: {2}", clipStart, clipEnd, clipLen);

                engine.ConvertProgressEvent += Engine_ConvertProgressEvent;
                engine.ConversionCompleteEvent += Engine_ConversionCompleteEvent;
                engine.Convert(inputFile, outputFile, options);
            }
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
