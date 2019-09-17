using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

using System.IO;
using Vlc.DotNet.Forms;
using System.Threading;

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
        // clean up, create load video function instead of this mess
        // https://blog.nytsoi.net/2017/12/31/ffmpeg-combining-audio-tracks add support for combining mic track
        // ffmpeg -i 'input.mkv' -filter_complex '[0:a:1]volume=0.1[l];[0:a:0][l]amerge=inputs=2[a]' -map '0:v:0' -map '[a]' -c:v copy -c:a libmp3lame -q:a 3 -ac 2 'output.mp4'
        // Add reordering clips on the clips manager

        List<VideoClip> clips = new List<VideoClip>();
        VlcControl vlcControlPlayer = new VlcControl();

        string clipPath; //= Path.GetFullPath("test.mp4");
        TimeSpan clipStart = TimeSpan.FromHours(1337); // Use 1337 hours as unset value.
        TimeSpan clipEnd = TimeSpan.FromHours(1337);
        System.Windows.Forms.Timer timerTrackBarUpdate;

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

            timerTrackBarUpdate = new System.Windows.Forms.Timer();
            timerTrackBarUpdate.Interval = 100;
            timerTrackBarUpdate.Tick += TimerTrackBarUpdate_Tick;
            timerTrackBarUpdate.Start();
        }

        //private bool firstLoad = true;
        private void initPlayer()
        {
            /*if(firstLoad)
            {
                if (player != null)
                    player.Dispose();
                player = new MpvPlayer(panelPlayer.Handle, dllPath);
                //player = new MpvPlayer(dllPath);
                //player.LogLevel = Mpv.NET.API.MpvLogLevel.Debug;
                player.MediaLoaded += Player_MediaLoaded;
                player.MediaPaused += Player_MediaPaused;
                player.MediaResumed += Player_MediaResumed;
                firstLoad = false;
            }
            //if (player != null)
            //    player.Dispose();

           // player.Load(clipPath);
            player.API.Command("loadfile",clipPath);
            Console.WriteLine("Property: " +player.API.GetPropertyLong("duration"));
            player.Loop = true;
            player.Resume();
            System.Threading.Thread.Sleep(150);*/
            vlcControlPlayer.BeginInit();
            DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "\\libvlc\\win-x86");
            vlcControlPlayer.VlcLibDirectory = di;
            //vlcControlPlayer.VlcMediaplayerOptions = new[] { "-vv"}; // verbose console output
            vlcControlPlayer.EndInit();
            vlcControlPlayer.Dock = DockStyle.Fill;
            vlcControlPlayer.AllowDrop = false;
            panelVlcPlayer.Controls.Add(vlcControlPlayer);
            vlcControlPlayer.VlcMediaPlayer.PositionChanged += VlcMediaPlayer_PositionChanged;

            //loadVideo(clipPath);
        }


        private void VlcMediaPlayer_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            float posperc =  e.NewPosition * 100;
            //trackBarPlayer.Maximum = (int)vlcControlPlayer.GetCurrentMedia().Duration.TotalMilliseconds;
            //Console.WriteLine("Max: " + (int)vlcControlPlayer.GetCurrentMedia().Duration.TotalMilliseconds);
            //Console.WriteLine("Pos chnged: " + e.NewPosition + " % " + posperc);
            float currentPositionMs = posperc * (trackBarPlayer.Maximum / 100);
            //Console.WriteLine("currentPositionMs: " + currentPositionMs);
            //Console.WriteLine("trackbar value: " + trackBarPlayer.Value);

            // Invoke or we crash on wrong thread
            trackBarPlayer.Invoke((Action)delegate
            {
                // This should be done in vlc video loaded event or something
                trackBarPlayer.Maximum = (int)vlcControlPlayer.GetCurrentMedia().Duration.TotalMilliseconds;
                //Console.WriteLine("Max: " + (int)vlcControlPlayer.GetCurrentMedia().Duration.TotalMilliseconds);

                if (currentPositionMs < 0)
                    currentPositionMs = 0;
                trackBarPlayer.Value = (int)currentPositionMs;
            });
        }

        private void loadVideo(string path)
        {
            Console.WriteLine("Loading video " + path);
           // vlcControlPlayer.SetMedia(new FileInfo(path), new[] { "input-repeat=65535" });
          //  vlcControlPlayer.Play(); // This needs to be done outside main thread or the program can freeze
            // https://github.com/ZeBobo5/Vlc.DotNet/wiki/Vlc.DotNet-freezes-(don't-call-Vlc.DotNet-from-a-Vlc.DotNet-callback)
            ThreadPool.QueueUserWorkItem(a => vlcControlPlayer.Play(new FileInfo(path), new[] { "input-repeat=65535" }));
            //System.Threading.Thread.Sleep(150); // wait for load or else it returns 0
            //trackBarPlayer.Maximum = (int)vlcControlPlayer.GetCurrentMedia().Duration.TotalMilliseconds;
            //Console.WriteLine("Video duration: " + (int)vlcControlPlayer.GetCurrentMedia().Duration.TotalMilliseconds);

            resetClipLabels();
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
           /* Console.WriteLine("timer Pos {0}, dur {1}", player.Position, player.Duration);
            if(player.Duration.TotalSeconds == 0)
            {
                Console.WriteLine("bugged shit");
               // firstLoad = true;
               // initPlayer();
            }
            trackBarPlayer.Invoke((Action)delegate
            {
                trackBarPlayer.Value = (int)player.Position.TotalMilliseconds;
            });*/
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if(player != null) player.Dispose();
            // Needs to be called from different thread or it may deadlock
            ThreadPool.QueueUserWorkItem(a => vlcControlPlayer.Stop());
        }

        //bool playerPaused = false;
        private void ButtonPlayerStop_Click(object sender, EventArgs e)
        {
            togglePlayerPause();
        }

        private void togglePlayerPause()
        {
            if(vlcControlPlayer.IsPlaying)
            {
                buttonPlayerStop.Invoke((Action)delegate
                {
                    buttonPlayerStop.Text = "Resume";
                });
            }
            else
            {
                buttonPlayerStop.Invoke((Action)delegate
                {
                    buttonPlayerStop.Text = "Pause";
                });
            }
            vlcControlPlayer.Pause();
            /*if (player.IsPlaying)
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
            }*/
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Keydown: {0}", e.KeyCode);
            if ((int)vlcControlPlayer.VlcMediaPlayer.FramesPerSecond == 0) //nothing loaded yet, return
                return;
            int frameTime = 1000 / (int)vlcControlPlayer.VlcMediaPlayer.FramesPerSecond;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    vlcControlPlayer.Time -= frameTime;
                    if ((trackBarPlayer.Value - frameTime) > 0)
                        trackBarPlayer.Value -= frameTime;
                    e.Handled = true;
                    break;
                case Keys.Right:
                    vlcControlPlayer.Time += frameTime;
                    if ((trackBarPlayer.Value + frameTime) < trackBarPlayer.Maximum)
                        trackBarPlayer.Value += frameTime;
                    //player.NextFrame();
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
            int posMs = trackBarPlayer.Value;
            float posPerc = ((float)trackBarPlayer.Value / (float)trackBarPlayer.Maximum);

            vlcControlPlayer.VlcMediaPlayer.Position = posPerc;
            //player.SeekAsync(TimeSpan.FromMilliseconds(trackBarPlayer.Value));
        }

        private void ButtonClipStart_Click(object sender, EventArgs e)
        {
            TimeSpan trackBarTimespan = TimeSpan.FromMilliseconds(trackBarPlayer.Value);
            Console.WriteLine("Clip start attempt: " + trackBarTimespan);
            /*if (clipEnd == TimeSpan.FromHours(1337))
            {
                Console.WriteLine("ClipEnd not set.");
                clipStart = trackBarTimespan;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMark1.Left = sliderposx+15;
            }*/
            if(trackBarTimespan > clipEnd)
            {
                Console.WriteLine("ERR: clip start Position is after clip end!!");
                resetClipLabels();
            }
            else
            {
                clipStart = trackBarTimespan;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMark1.Left = sliderposx + 15;
            }
            Console.WriteLine("Clip start: " + clipStart);
        }

        private void ButtonClipEnd_Click(object sender, EventArgs e)
        {
            TimeSpan trackBarTimespan = TimeSpan.FromMilliseconds(trackBarPlayer.Value);

            Console.WriteLine("Clip end attempt: " + trackBarTimespan);
            Console.WriteLine("start {0}, end {1}: ", clipStart, clipEnd);
            /*if (clipStart == TimeSpan.FromHours(1337))
            {
                Console.WriteLine("ClipStart not set.");
                clipEnd = trackBarTimespan;
                int sliderposx = trackBarPlayer.Value * trackBarPlayer.Width / trackBarPlayer.Maximum;
                Console.WriteLine("Slider pos: " + sliderposx);
                labelMarkEnd.Left = sliderposx + 15;
            }*/
            if (trackBarTimespan < clipStart)
            {
                Console.WriteLine("ERR: clip start Position is after clip end!!");
                resetClipLabels();
            }
            else
            {
                clipEnd = trackBarTimespan;
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
                MessageBox.Show("Add clips first to render.");
                return;
            }
            // TODO Move this clip name logic to clip encoder!!! DONT NEED IT HERE
            foreach(VideoClip clip in clips)
            {
                MediaFile inputFile = new MediaFile { Filename = clip.filePath };
                string outputFolder =  Application.StartupPath + "\\render\\";
                string tempPath = outputFolder + clip.OutputName;//clip.OutputName;

                // Generate unique name for each clip so we don't override if multiple clips from same video.
                // Its overriding to first free name found for all clips now..
                Console.WriteLine("Found existing clip with same filename, generating new name..");
                bool tryNextName = true;
                int counter = 1;
                tempPath = outputFolder + counter.ToString() + "_" + clip.OutputName;
                while(tryNextName)
                {
                    bool nameUsedbyOtherClip = false;
                    //Console.WriteLine("Trying name: " + outputFolder + tempPath);
                    if (File.Exists(tempPath))
                    {
                        counter++;
                        tempPath = outputFolder + counter.ToString() + "_" + clip.OutputName;
                    }
                    else
                    {
                        foreach (VideoClip clip2 in clips)
                        {
                            if (clip2.OutputName == counter.ToString() + "_" + clip.OutputName)
                            {
                                // Console.WriteLine("Clip name already taken by other clip on the list, trying next one..");
                                counter++;
                                tempPath = outputFolder + counter.ToString() + "_" + clip.OutputName;
                                nameUsedbyOtherClip = true;
                                break;
                            }
                        }
                        if(!nameUsedbyOtherClip)
                        {
                            //S Console.WriteLine("found free name: " + outputFolder + tempPath);
                            tryNextName = false;
                            clip.OutputName = counter.ToString() + "_" + clip.OutputName;
                            break;
                        }
                    }
                }
            }
            buttonPlayerStop.Text = "Resume";
            vlcControlPlayer.SetPause(true);
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
            loadVideo(clipPath);
            //initPlayer();
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
            labelMark1.Location = new Point(trackBarPlayer.Location.X, trackBarPlayer.Location.Y+20); // 12
            labelMarkEnd.Location = new Point(trackBarPlayer.Location.X + trackBarPlayer.Width-20, trackBarPlayer.Location.Y+20); // 771
            // Check if nothing is loaded into the player yet
            if ((int)vlcControlPlayer.VlcMediaPlayer.FramesPerSecond == 0)
                return;
            clipEnd = vlcControlPlayer.GetCurrentMedia().Duration;
            clipStart = TimeSpan.Zero;
        }

        private void ButtonAddClip_Click(object sender, EventArgs e)
        {
            /*if(clipEnd.TotalMilliseconds == 0)
            {
                // MessageBox.Show("Clip end or start not set! \n Cancelling clip add.");
                // return;
                // Add full video file?
                clipEnd = vlcControlPlayer.GetCurrentMedia().Duration;
            }
            if(clipStart.TotalMilliseconds == 0)
            {
                clipStart = TimeSpan.Zero;
            }*/
            if(vlcControlPlayer.GetCurrentMedia() == null)
            {
                Console.WriteLine("No video loaded yet! Can't add a clip.");
                MessageBox.Show("No video loaded yet! Adding clip cancelled.");
                return;
            }

            Console.WriteLine("Adding clip: {0}-{1} on {2}",clipStart,clipEnd, clipPath);
            Console.WriteLine("audio tracks count: " + vlcControlPlayer.GetCurrentMedia().Tracks.Length);
            bool multiTrackAudio = false;
            if (vlcControlPlayer.GetCurrentMedia().Tracks.Length > 2)
                multiTrackAudio = true;
            /*foreach(Vlc.DotNet.Core.Interops.MediaTrack track in vlcControlPlayer.GetCurrentMedia().Tracks)
            {
                Console.WriteLine(track);
                // Video
                // System sound
                // Microphone
            }*/
            clips.Add(new VideoClip() {Start = clipStart, End = clipEnd, filePath = clipPath, OutputName = Path.GetFileName(clipPath), MultiTrackAudio = multiTrackAudio, Encode = true, MergeAudioTracks = false, Volume = 100 });
            //Reset the clip stuff.
            resetClipLabels();
        }

        private void LoadVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open video file";
            //dialog.Filter = "Video files|*.*"
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                clipPath = dialog.FileName.ToString();
                loadVideo(clipPath);
                //initPlayer();
                resetClipLabels();
            }
        }

        private void ButtonMute_Click(object sender, EventArgs e)
        {
            if(vlcControlPlayer.Audio.Volume == 0)
            {
                vlcControlPlayer.Audio.Volume = 100;
                buttonMute.Text = "Mute";
            }
            else
            {
                vlcControlPlayer.Audio.Volume = 0;
                buttonMute.Text = "Unmute";
            }
        }

        private void TrackBarPlayer_SizeChanged(object sender, EventArgs e)
        {
            // TODO
            // Recalculate current clip location markers instead of resetting it
            resetClipLabels();
        }
    }
}
