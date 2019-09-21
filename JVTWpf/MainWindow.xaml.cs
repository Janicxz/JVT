using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        // TODO: FIXME: random crashing on close due to child window (ClipsManager)
        // Add encoder progress bar instead of freezing the UI while ffmpeg is busy.
        // Add support for adding custom audio tracks into the clips (based on audio merging feature)
        // Check if we're rendering same files with same settings again in clipmanager, skip unnecessary encoding jobs.
        // Add passing selected current clip from manager to mainwindow so user can preview which clip is which and change in/out position if needed instead of manual edit
        public MainWindow()
        {
            Unosquare.FFME.Library.FFmpegDirectory = Environment.CurrentDirectory + @"\ffmpeg";
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private bool mediaReadyToPlay = false;
        private VideoClip currentClip;
        ObservableCollection<VideoClip> videoClips;
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
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.Closing += MainWindow_Closing;
            this.AllowDrop = true;
            //playerTimeSlider.IsMoveToPointEnabled = true;
            ffmePlayer.LoopingBehavior = Unosquare.FFME.Common.MediaPlaybackState.Play;
            videoClips = new ObservableCollection<VideoClip>();

            //managerWindow = new ClipsManager(videoClips);


#if DEBUG
            loadVideo(Environment.CurrentDirectory + @"\test2.webm");
#endif
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Keydown: {0}", e.Key);
            if(!ffmePlayer.IsOpen)
            {
                Console.WriteLine("No file loaded yet.");
                return;
            }

            int frameTime = 1000 / (int)ffmePlayer.VideoFrameRate;
            switch(e.Key)
            {
                case Key.Left:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        ffmePlayer.Position = currentClip.Start;
                    else if (Keyboard.Modifiers == ModifierKeys.Control)
                        SetClipStart();
                    else
                        ffmePlayer.Position -= TimeSpan.FromMilliseconds(frameTime);
                    e.Handled = true;
                    break;
                case Key.Right:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        ffmePlayer.Position = currentClip.End;
                    else if (Keyboard.Modifiers == ModifierKeys.Control)
                        SetClipEnd();
                    else
                        ffmePlayer.Position += TimeSpan.FromMilliseconds(frameTime);
                    e.Handled = true;
                    break;
                case Key.I: // Premiere hotkeys
                    SetClipStart();
                    e.Handled = true;
                    break;
                case Key.O:
                    SetClipEnd();
                    e.Handled = true;
                    break;
                case Key.X:
                    AddClip();
                    e.Handled = true;
                    break;
                case Key.Up:
                    ffmePlayer.Position += TimeSpan.FromMilliseconds(1000);
                    e.Handled = true;
                    break;
                case Key.Down:
                    ffmePlayer.Position -= TimeSpan.FromMilliseconds(1000);
                    e.Handled = true;
                    break;
                case Key.Space:
                    togglePlayerPause();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
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
            if(!ffmePlayer.IsPaused)
                togglePlayerPause();

            managerWindow = new ClipsManager(videoClips);
            managerWindow.Closed += ManagerWindow_Closed;
            managerWindow.dataGridClips.SelectionChanged += DataGridClips_SelectionChanged;
            managerWindow.Show();
        }

        private bool requestClipLoad = false;
        private void DataGridClips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(VideoClip selectedClip in e.AddedItems)
            {
                Console.WriteLine("Selected: " + selectedClip.OutputName);
                loadVideo(selectedClip.filePath);
                requestClipLoad = true;
                currentClip = selectedClip;
                // Update UI here for the selected clip
                //currentClip = selectedClip;
                //loadvid
            }
        }

        private void ManagerWindow_Closed(object sender, EventArgs e)
        {
            managerWindow.Closed -= ManagerWindow_Closed;
            managerWindow.dataGridClips.SelectionChanged -= DataGridClips_SelectionChanged;
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
            AddClip();
        }

        private void AddClip()
        {
            if (!mediaReadyToPlay)
            {
                Console.WriteLine("No media loaded yet. Can't add clip");
                return;
            }
            Console.WriteLine("Adding clip {0}-{1} ", currentClip.Start, currentClip.End);
            checkFileForDuplicate();
            currentClip.Length = currentClip.End - currentClip.Start;
            videoClips.Add(currentClip);
            if (managerWindow != null)
                managerWindow.RefreshDatagrid();
            // we're adding new clips, not editing current one anymore
            requestClipLoad = false;
            resetInterface();
        }

        private void ButtonClipEnd_Click(object sender, RoutedEventArgs e)
        {
            SetClipEnd();
            //Console.WriteLine("endslider value: " + playerTimeSliderCustom.EndSlider.Value);
        }

        private void SetClipEnd()
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
            currentClip.Length = currentClip.End - currentClip.Start;
            Console.WriteLine("Selecting clip end at " + currentClip.End);
            playerTimeSliderCustom.EndSlider.Value = currentClip.End.TotalMilliseconds;
        }

        private void ButtonClipStart_Click(object sender, RoutedEventArgs e)
        {
            SetClipStart();
        }
        private void SetClipStart()
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
                // add check if our reposition is valid
                //ffmePlayer.Position = currentClip.End - TimeSpan.FromSeconds(1);
                resetInterface();
                return;
            }
            currentClip.Start = ffmePlayer.Position;
            currentClip.Length = currentClip.End - currentClip.Start;
            Console.WriteLine("Selecting clip start at " + currentClip.Start);
            playerTimeSliderCustom.StartSlider.Value = currentClip.Start.TotalMilliseconds;
            //Console.WriteLine("startslider value: " + playerTimeSliderCustom.StartSlider.Value);
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                Console.WriteLine("Got drag & drop: " + files[0]);
                loadVideo(files[0]);
            }
        }

        private void ButtonPauseToggle_Click(object sender, RoutedEventArgs e)
        {
            if (!mediaReadyToPlay)
            {
                Console.WriteLine("No media loaded yet. Can't toggle pause");
                return;
            }
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
            if (e.Position.TotalMilliseconds > playerTimeSliderCustom.CurrentSlider.Maximum)
                currentPosition = playerTimeSliderCustom.CurrentSlider.Maximum;
            else if (e.Position.TotalMilliseconds < 0)
                currentPosition = 0;
            else
                currentPosition = e.Position.TotalMilliseconds;
            //Console.WriteLine("Setting slider pos: " + currentPosition);
            // ignore valuechanged event for automatic playback slider changes.
            //playerTimeSlider.ValueChanged -= PlayerTimeSlider_ValueChanged;
            //playerTimeSlider.Value = currentPosition;
            playerTimeSliderCustom.CurrentSlider.ValueChanged -= CurrentSlider_ValueChanged;
            playerTimeSliderCustom.CurrentSlider.Value = currentPosition;
            playerTimeSliderCustom.CurrentSlider.ValueChanged += CurrentSlider_ValueChanged;
            //playerTimeSlider.ValueChanged += PlayerTimeSlider_ValueChanged;
        }

        private void CurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Console.WriteLine("Slider click at {0}, changing player position..", e.NewValue);
            ffmePlayer.Position = TimeSpan.FromMilliseconds(e.NewValue);
        }
        /*
        private void PlayerTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Console.WriteLine("Slider click at {0}, changing player position..", playerTimeSlider.Value);
            ffmePlayer.Position = TimeSpan.FromMilliseconds(playerTimeSlider.Value);
        }
        */
        private void FfmePlayer_MediaReady(object sender, EventArgs e)
        {
            Console.WriteLine("Media ready to play");
            mediaReadyToPlay = true;
            if(requestClipLoad)
            {
                UpdateInterface();
                ffmePlayer.Position = currentClip.Start;
            }
            else
            {
                resetInterface();
                // Console.WriteLine("Codec: " + ffmePlayer.VideoCodec);
                currentClip.inputFileCodec = ffmePlayer.VideoCodec;
                Console.WriteLine("has audio : " + ffmePlayer.HasAudio);
                currentClip.HasAudio = ffmePlayer.HasAudio;
            }
        }

        private void UpdateInterface()
        {
            playerTimeSliderCustom.CurrentSlider.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            playerTimeSliderCustom.CurrentSlider.Minimum = 0;
            playerTimeSliderCustom.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            playerTimeSliderCustom.Minimum = 0;
            playerTimeSliderCustom.EndSlider.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            playerTimeSliderCustom.EndSlider.Minimum = 0;
            playerTimeSliderCustom.StartSlider.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            playerTimeSliderCustom.StartSlider.Minimum = 0;
            playerTimeSliderCustom.EndSlider.Value = currentClip.End.TotalMilliseconds;
            playerTimeSliderCustom.StartSlider.Value = currentClip.Start.TotalMilliseconds;
        }
        // Reset clip UI and prepare clip info
        private void resetInterface()
        {
            //playerTimeSlider.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
           

            playerTimeSliderCustom.CurrentSlider.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            playerTimeSliderCustom.CurrentSlider.Minimum = 0;
            playerTimeSliderCustom.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            playerTimeSliderCustom.Minimum = 0;
            playerTimeSliderCustom.EndSlider.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            playerTimeSliderCustom.EndSlider.Minimum = 0;
            playerTimeSliderCustom.StartSlider.Maximum = ffmePlayer.MediaInfo.Duration.TotalMilliseconds;
            playerTimeSliderCustom.StartSlider.Minimum = 0;
            playerTimeSliderCustom.EndSlider.Value = playerTimeSliderCustom.Maximum;
            playerTimeSliderCustom.StartSlider.Value = playerTimeSliderCustom.Minimum;
            //Console.WriteLine("Resetting slider max: " + playerTimeSliderCustom.CurrentSlider.Maximum);

            if(requestClipLoad)
            {
                currentClip.Start = TimeSpan.Zero;
                currentClip.End = ffmePlayer.MediaInfo.Duration;
                currentClip.Length = ffmePlayer.MediaInfo.Duration;
                Console.WriteLine("Setting clip reset len: " + currentClip.Length);
                return;
            }

            currentClip = new VideoClip();
            currentClip.filePath = ffmePlayer.MediaInfo.MediaSource;
            currentClip.OutputName = System.IO.Path.GetFileName(currentClip.filePath);
            currentClip.Volume = 100;
            currentClip.Encode = true;
            currentClip.Start = TimeSpan.Zero;
            currentClip.End = ffmePlayer.MediaInfo.Duration;
            currentClip.Length = ffmePlayer.MediaInfo.Duration;
            // Console.WriteLine("Stream count: " + ffmePlayer.MediaInfo.Streams.Count);
            if (ffmePlayer.MediaInfo.Streams.Count >= 3) // Has extra audio tracks/streams
            {
                currentClip.MultiTrackAudio = true;
                currentClip.MergeAudioTracks = true;
                Console.WriteLine("Multiple audio tracks detected");
            }
            // FIXME: finish this slider fuckery
            /*Point sliderPos = playerTimeSlider.PointToScreen(new Point(0d, 0d));
            Console.WriteLine("sliderpos:" + sliderPos);
            Console.WriteLine("Slider width " + playerTimeSlider.ActualWidth + " final: " + (sliderPos.X - playerTimeSlider.ActualWidth));
            Console.WriteLine("label: " + labelStart.Margin);
            labelStart.Margin = new Thickness(sliderPos.X, sliderPos.Y, 0, 0);
            Console.WriteLine("after: " + labelStart.Margin);*/
            //labelMark1.Location = new Point(trackBarPlayer.Location.X, trackBarPlayer.Location.Y + 20); // 12
            //labelMarkEnd.Location = new Point(trackBarPlayer.Location.X + trackBarPlayer.Width - 20, trackBarPlayer.Location.Y + 20); // 771
        }

        private void checkFileForDuplicate()
        {
            // Force mp4 output for now
            // TODO: Add proper format selection
            if (System.IO.Path.GetExtension(currentClip.OutputName) != ".mp4")
            {
                Console.WriteLine("File extension not .mp4, changing.. ({0})", currentClip.OutputName);
                currentClip.OutputName = System.IO.Path.GetFileNameWithoutExtension(currentClip.OutputName);
                currentClip.OutputName += ".mp4";
            }
            Console.WriteLine("Checking name for duplicate {0}", currentClip.OutputName);

            string directoryName = Environment.CurrentDirectory; //System.IO.Path.GetDirectoryName(currentClip.filePath);
            directoryName += "\\videos";

            // make sure file doesnt exist on disk AND we don't have other clip in queue for same name
            bool nameExists = true;
            bool nameUsedInList = false;
            string outNameOriginal = currentClip.OutputName;
            int nameCount = 1;
            
            // Loop through names until filename doesn't exist on disk and it's not used by any clip in cliplist.
            while (nameExists || nameUsedInList)
            {
                // Don't loop through this again if we found a free name
                if (File.Exists(directoryName + "\\" + currentClip.OutputName) && nameExists)
                {
                    Console.WriteLine("File name already used: " + directoryName + "\\" + currentClip.OutputName);
                    if(!File.Exists(directoryName + "\\" + nameCount + "_" + currentClip.OutputName))
                    {
                        nameExists = false;
                        currentClip.OutputName = nameCount + "_" + currentClip.OutputName;
                        Console.WriteLine("Found free name not on disk: " + directoryName + "\\" + currentClip.OutputName);
                        //outNameOriginal = nameCount + "_" + outNameOriginal;
                    }
                    else
                    {
                        Console.WriteLine("found file on disk: " + directoryName + "\\" + currentClip.OutputName);
                        nameCount++;
                    }
                }
                else
                {
                    //Console.WriteLine("Filename is free: " + directoryName + "\\" + currentClip.OutputName);
                    nameExists = false;
                }
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
            requestClipLoad = false;

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
