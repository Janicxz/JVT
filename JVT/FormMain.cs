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
using System.IO;

namespace JVT
{
    public partial class FormMain : Form
    {
        // TODO:
        // Add file selection
        // Add new page/window for render queue which lets you select the clips you want merged into single output video like simple video editor.
        // Add encoding options
        // Add multi clip support
        // Add support to merging multiple videos into single one.
        // Add volume button
        // Multi channel audio merging? (if separate mic and game audio for example.)
        // Youtube upload support with network interface select option?
        // integration with OBS?
        // Drag & Drop gets bugged after 3rd drag? something todo with form handle breaking or smth from MPV init?
        // Add 3rd form for rendering settings?
        // Add encoding status to clipencoder class so we can update UI in formclipslist when one of the encodes finishes.( ie, 1/3 done)
        // Finish merging clips functionality
        List<VideoClip> clips = new List<VideoClip>();
        string dllPath = "lib\\mpv-1.dll";
        string clipPath = Path.GetFullPath("test.mp4");
        TimeSpan clipStart = TimeSpan.FromHours(1337); // Use 1337 hours as unset value.
        TimeSpan clipEnd = TimeSpan.FromHours(1337);
        MpvPlayer player;
        Timer timerTrackBarUpdate;

        public FormMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Init
            /*player = new MpvPlayer(panelPlayer.Handle, dllPath);
            player.Load(clipPath);
            player.Loop = true;
            player.Resume();
            player.MediaLoaded += Player_MediaLoaded;
            player.MediaPaused += Player_MediaPaused;
            player.MediaResumed += Player_MediaResumed;*/
            initPlayer();

            labelMark1.BackColor = Color.Transparent;
            labelMarkEnd.BackColor = Color.Transparent;

            timerTrackBarUpdate = new Timer();
            timerTrackBarUpdate.Interval = 100;
            timerTrackBarUpdate.Tick += TimerTrackBarUpdate_Tick;
            timerTrackBarUpdate.Start();
        }

        private void initPlayer()
        {
            if (player != null)
                player.Dispose();
            player = new MpvPlayer(panelPlayer.Handle, dllPath);
            player.Load(clipPath);
            player.MediaLoaded += Player_MediaLoaded;
            player.MediaPaused += Player_MediaPaused;
            player.MediaResumed += Player_MediaResumed;
            player.Loop = true;
            player.Resume();
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
            buttonPlayerStop.Invoke((Action)delegate
            {
                buttonPlayerStop.Text = "Resume";
            });
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
                trackBarPlayer.Value = 0;
                trackBarPlayer.Maximum = (int)player.Duration.TotalMilliseconds;
                trackBarPlayer.Minimum = 0;
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
             if(player != null) player.Dispose();
        }

        //bool playerPaused = false;
        private void ButtonPlayerStop_Click(object sender, EventArgs e)
        {
            togglePlayerPause();
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

        // TODO: Make dragging scrollbar not jump around on timer ticks?
        private void TrackBarPlayer_Scroll(object sender, EventArgs e)
        {
            player.SeekAsync(TimeSpan.FromMilliseconds(trackBarPlayer.Value));
        }

        private void ButtonClipStart_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Clip start attempt: " + player.Position);
            if (clipEnd == TimeSpan.FromHours(1337))
            {
                Console.WriteLine("ClipEnd not set.");
                clipStart = player.Position;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMark1.Left = sliderposx+15;
            }
            if(player.Position > clipEnd)
            {
                Console.WriteLine("ERR: clip start Position is after clip end!!");
                resetClipLabels();
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
            if (clipStart == TimeSpan.FromHours(1337))
            {
                Console.WriteLine("ClipStart not set.");
                clipEnd = player.Position;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMarkEnd.Left = sliderposx + 15;
            }
            if (player.Position < clipStart)
            {
                Console.WriteLine("ERR: clip start Position is after clip end!!");
                resetClipLabels();
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
            if(clips.Count == 0)
            {
                Console.WriteLine("No clips to encode, count: " + clips.Count);
                return;
            }
            // TODO Move this clip name logic to clip encoder!!! DONT NEED IT HERE
            foreach(VideoClip clip in clips)
            {
                MediaFile inputFile = new MediaFile { Filename = clip.filePath };
                string outputFolder =  Application.StartupPath + "\\render\\";
                string tempPath = clip.OutputName;

                // Generate unique name for each clip so we don't override if multiple clips from same video.
                // Its overriding to first free name found for all clips now..
                if (File.Exists(outputFolder + clip.OutputName))
                {
                    Console.WriteLine("Found existing clip with same filename, generating new name..");
                    bool tryNextName = true;
                    int counter = 1;
                    tempPath = counter.ToString() + "_" + clip.filePath;
                    while(tryNextName)
                    {
                        bool nameUsedbyOtherClip = false;
                        //Console.WriteLine("Trying name: " + outputFolder + tempPath);
                        if (File.Exists(outputFolder + tempPath))
                        {
                            counter++;
                            tempPath = counter.ToString() + "_" + clip.filePath;
                        }
                        else
                        {
                            foreach (VideoClip clip2 in clips)
                            {
                                if (clip2.OutputName == tempPath)
                                {
                                   // Console.WriteLine("Clip name already taken by other clip on the list, trying next one..");
                                    counter++;
                                    tempPath = counter.ToString() + "_" + clip.filePath;
                                    nameUsedbyOtherClip = true;
                                    break;
                                }
                            }
                            if(!nameUsedbyOtherClip)
                            {
                               //S Console.WriteLine("found free name: " + outputFolder + tempPath);
                                tryNextName = false;
                                break;
                            }
                        }
                    }
                }
                clip.OutputName = tempPath;


                // Move this to clips list form for final confirm
                /*
                MediaFile outputFile = new MediaFile { Filename = outputFolder + clip.filePath };
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
                }
                */
            }
            FormClipList formClipList = new FormClipList(clips);
            formClipList.Show();
            // Gets passed by reference, clearing here clears the list passed to encoder too.
            //clips.Clear();
        }



        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            //Console.WriteLine("Dragged file " + filePaths[0]);
            clipPath = @filePaths[0];
            initPlayer();
            /*player.Dispose();

            player = new MpvPlayer(panelPlayer.Handle, dllPath);
            player.Load(@filePaths[0]);
            player.Loop = true;
            player.MediaLoaded += Player_MediaLoaded;
            player.MediaPaused += Player_MediaPaused;
            player.MediaResumed += Player_MediaResumed;
            player.Resume();
            clipPath = @filePaths[0];*/
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            //Console.WriteLine("Got drag enter ");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void resetClipLabels()
        {
            labelMark1.Location = new Point(12, 384);
            labelMarkEnd.Location = new Point(771, 384);
            clipEnd = TimeSpan.FromHours(1337);
            clipStart = TimeSpan.FromHours(1337);
        }

        private void ButtonAddClip_Click(object sender, EventArgs e)
        {
            if(clipEnd.TotalMilliseconds == 0  || clipStart.TotalMilliseconds == 0)
            {
                MessageBox.Show("Clip end or start not set! \n Cancelling clip add.");
                return;
            }
            clips.Add(new VideoClip() {Start = clipStart, End = clipEnd, filePath = clipPath, OutputName = Path.GetFileName(clipPath) });
            //Reset the clip stuff.
            resetClipLabels();
        }
    }
}
