using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace JVTWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Unosquare.FFME.Library.FFmpegDirectory = Environment.CurrentDirectory + @"\ffmpeg";
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private bool mediaReadyToPlay = false;
        private VideoClip currentClip;
        List<VideoClip> videoClips;
        ClipsManager managerWindow;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ffmePlayer.MediaOpening += FfmePlayer_MediaOpening;
            ffmePlayer.MediaReady += FfmePlayer_MediaReady;
            ffmePlayer.PositionChanged += FfmePlayer_PositionChanged;
            buttonPauseToggle.Click += ButtonPauseToggle_Click;
            this.Drop += MainWindow_Drop;
            buttonClipStart.Click += ButtonClipStart_Click;
            buttonClipEnd.Click += ButtonClipEnd_Click;
            buttonClipAdd.Click += ButtonClipAdd_Click;
            buttonMuteToggle.Click += ButtonMuteToggle_Click;
            buttonEncode.Click += ButtonEncode_Click;
            this.Closing += MainWindow_Closing;
            this.AllowDrop = true;
            playerTimeSlider.IsMoveToPointEnabled = true;
            ffmePlayer.LoopingBehavior = Unosquare.FFME.Common.MediaPlaybackState.Play;
            videoClips = new List<VideoClip>();
            //managerWindow = new ClipsManager(videoClips);

            loadVideo(Environment.CurrentDirectory + @"\test2.webm");
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //managerWindow.closingApplication = true;
            ffmePlayer.Close();
            ffmePlayer.Dispose();
           // managerWindow.Close();
        }

        private void ButtonEncode_Click(object sender, RoutedEventArgs e)
        {
            //if(managerWindow == null)
            managerWindow = new ClipsManager(videoClips);
            managerWindow.Closed += ManagerWindow_Closed;
            managerWindow.Show();
        }

        private void ManagerWindow_Closed(object sender, EventArgs e)
        {
            managerWindow.Closed -= ManagerWindow_Closed;
            managerWindow = null;
        }

        private void ButtonMuteToggle_Click(object sender, RoutedEventArgs e)
        {
            if(ffmePlayer.Volume == 1)
            {
                buttonMuteToggle.Content = "🔊 Unmute";
                ffmePlayer.Volume = 0;
            }
            else
            {
                buttonMuteToggle.Content = "🔇 Mute";
                ffmePlayer.Volume = 1;
            }
        }

        private void FfmePlayer_MediaOpening(object sender, Unosquare.FFME.Common.MediaOpeningEventArgs e)
        {
            Console.WriteLine("Opening media");
            mediaReadyToPlay = false;
        }

        private void ButtonClipAdd_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Adding clip {0}-{1} ", currentClip.Start, currentClip.End);
            if(!mediaReadyToPlay)
            {
                Console.WriteLine("No media loaded yet. Can't add clip");
                return;
            }
            checkFileForDuplicate();
            currentClip.Length = currentClip.End-currentClip.Start;
            videoClips.Add(currentClip);
            if(managerWindow != null)
                managerWindow.RefreshDatagrid();
            resetInterface();
        }

        private void ButtonClipEnd_Click(object sender, RoutedEventArgs e)
        {
            if (!mediaReadyToPlay)
            {
                Console.WriteLine("No media loaded yet. Can't select clip end");
                return;
            }
            // Check for clip end sanity
            if (ffmePlayer.Position <= currentClip.Start)
            {
                Console.WriteLine("ERROR: Clip is set to end before it starts");
                resetInterface();
                return;
            }

            currentClip.End = ffmePlayer.Position;
            Console.WriteLine("Selecting clip end at " + currentClip.End);
        }

        private void ButtonClipStart_Click(object sender, RoutedEventArgs e)
        {
            if (!mediaReadyToPlay)
            {
                Console.WriteLine("No media loaded yet. Can't select clip start");
                return;
            }
            // Check for clip start sanity
            if (ffmePlayer.Position >= currentClip.End)
            {
                Console.WriteLine("ERROR: Clip is set to start after it ends");
                resetInterface();
                return;
            }
            currentClip.Start = ffmePlayer.Position;
            Console.WriteLine("Selecting clip start at " + currentClip.Start);
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                Console.WriteLine("Got drag & drop: " + files);
                loadVideo(files[0]);
            }
        }

        private void ButtonPauseToggle_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Pause toggle clicked!");
            togglePlayerPause();
        }

        private void togglePlayerPause()
        {
            buttonPauseToggle.Content = (ffmePlayer.IsPaused) ? "⏸ Pause" : "▶️ Resume";
            if(ffmePlayer.IsPaused)
                ffmePlayer.Play();
            else
                ffmePlayer.Pause();
        }

        private void FfmePlayer_PositionChanged(object sender, Unosquare.FFME.Common.PositionChangedEventArgs e)
        {
            double currentPosition = 0;
            //Console.WriteLine("Media position: " + e.Position);
            if (e.Position.TotalMilliseconds > playerTimeSlider.Maximum)
                currentPosition = playerTimeSlider.Maximum;
            else if (e.Position.TotalMilliseconds < 0)
                currentPosition = 0;
            else
                currentPosition = e.Position.TotalMilliseconds;
            //Console.WriteLine("Setting slider pos: " + currentPosition);
            // ignore valuechanged event for automatic playback slider changes.
            playerTimeSlider.ValueChanged -= PlayerTimeSlider_ValueChanged;
            playerTimeSlider.Value = currentPosition;
            playerTimeSlider.ValueChanged += PlayerTimeSlider_ValueChanged;
        }

        private void PlayerTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Console.WriteLine("Slider click at {0}, changing player position..", playerTimeSlider.Value);
            ffmePlayer.Position = TimeSpan.FromMilliseconds(playerTimeSlider.Value);
        }

        private void FfmePlayer_MediaReady(object sender, EventArgs e)
        {
            Console.WriteLine("Media ready to play");
            mediaReadyToPlay = true;
            resetInterface();
        }

       /* private void resetCurrentClip()
        {
            currentClip = new VideoClip();
            currentClip.filePath = ffmePlayer.MediaInfo.MediaSource;
            currentClip.OutputName = System.IO.Path.GetFileName(currentClip.filePath);
            currentClip.Volume = 100;
            currentClip.Encode = true;
            currentClip.Start = TimeSpan.Zero;
            currentClip.End = ffmePlayer.MediaInfo.Duration;
            if (ffmePlayer.MediaInfo.Streams.Count >= 3) // Has extra audio tracks/streams
            {
                currentClip.MultiTrackAudio = true;
                currentClip.MergeAudioTracks = true;
                //Console.WriteLine("Multiple audio tracks detected");
            }
        }*/

        // Reset clip UI and prepare clip info
        private void resetInterface()
        {
            playerTimeSlider.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            Console.WriteLine("Resetting slider max: " + playerTimeSlider.Maximum);

            currentClip = new VideoClip();
            currentClip.filePath = ffmePlayer.MediaInfo.MediaSource;
            currentClip.OutputName = System.IO.Path.GetFileName(currentClip.filePath);
            currentClip.Volume = 100;
            currentClip.Encode = true;
            currentClip.Start = TimeSpan.Zero;
            currentClip.End = ffmePlayer.MediaInfo.Duration;
            // Console.WriteLine("Stream count: " + ffmePlayer.MediaInfo.Streams.Count);
            if (ffmePlayer.MediaInfo.Streams.Count >= 3) // Has extra audio tracks/streams
            {
                currentClip.MultiTrackAudio = true;
                currentClip.MergeAudioTracks = true;
                Console.WriteLine("Multiple audio tracks detected");
            }
        }

        private void checkFileForDuplicate()
        {
            Console.WriteLine("Checking name for duplicate {0}", currentClip.OutputName);

            string directoryName = System.IO.Path.GetDirectoryName(currentClip.filePath);
            directoryName += "\\videos";

            // make sure file doesnt exist on disk AND we don't have other clip in queue for same name
            bool nameExists = true;
            bool nameUsedInList = false;
            string outNameOriginal = currentClip.OutputName;
            int nameCount = 1;
            
            while (nameExists || nameUsedInList)
            {
                // Don't loop through this again if we found a free name
                if (File.Exists(directoryName + "\\" + currentClip.OutputName) && nameExists)
                {
                    if(!File.Exists(directoryName + "\\" + nameCount + "_" + currentClip.OutputName))
                    {
                        nameExists = false;
                        currentClip.OutputName = nameCount + "_" + currentClip.OutputName;
                        //outNameOriginal = nameCount + "_" + outNameOriginal;
                    }
                    else
                    {
                        nameCount++;
                    }
                }
                else
                    nameExists = false;
                // We found free name, now check if its used in list
                if (!nameExists)
                {
                    // Reset and try again with new name
                    nameUsedInList = false;
                    foreach (VideoClip clip in videoClips)
                    {
                        if(clip.OutputName == currentClip.OutputName)
                        {
                            // Found a clip of same name in list, rename and try again
                            nameUsedInList = true;
                            currentClip.OutputName = nameCount + "_" + outNameOriginal;
                            nameCount++;
                            break;
                        }
                    }
                }
            }
            Console.WriteLine("Clip new name {0}", currentClip.OutputName);
        }

        private void loadVideo(string videoPath)
        {
            Console.WriteLine("Loading video: " + videoPath);

            currentClip = new VideoClip();
            currentClip.filePath = videoPath;
            currentClip.OutputName = System.IO.Path.GetFileName(videoPath);
            currentClip.Volume = 100;
            currentClip.Encode = true;

            ffmePlayer.Source = new Uri(videoPath);
            ffmePlayer.Play();
        }
    }
}
