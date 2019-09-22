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

        private string currentStartThumbFilename = "";
        private BitmapSource currentStartThumbnail;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*
#if RELEASE
            FileStream fs = new FileStream("consolelog.log", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.AutoFlush = true;
            Console.SetOut(sw);
            Console.SetError(sw);
#endif
*/
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
                    togglePlayerPause(false);
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
                togglePlayerPause(true);

            managerWindow = new ClipsManager(videoClips);
            managerWindow.Closed += ManagerWindow_Closed;
            managerWindow.dataGridClips.SelectionChanged += DataGridClips_SelectionChanged;
            managerWindow.buttonEncode.Click += ButtonEncode_Click1;
            managerWindow.Show();
        }

        private void ButtonEncode_Click1(object sender, RoutedEventArgs e)
        {
            ffmePlayer.Pause();
        }

        private bool requestClipLoad = false;
        private void DataGridClips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(VideoClip selectedClip in e.AddedItems)
            {
                // FIXME: Clicking on same clip that we added and adding new identical clip causes duplicate file check to get stuck in infinite loop
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
            managerWindow.buttonEncode.Click -= ButtonEncode_Click1;
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
            // Create new reference so we don't compare to our current clip already on the list
            VideoClip newClip = new VideoClip();
            newClip.Encode = currentClip.Encode;
            newClip.End = currentClip.End;
            newClip.filePath = currentClip.filePath;
            newClip.Length = currentClip.Length;
            newClip.Merge = currentClip.Merge;
            newClip.MergeAudioTracks = currentClip.MergeAudioTracks;
            newClip.MultiTrackAudio = currentClip.MultiTrackAudio;
            newClip.HasAudio = currentClip.HasAudio;
            newClip.forceOverwrite = currentClip.forceOverwrite;
            newClip.inputFileCodec = currentClip.inputFileCodec;
            newClip.OutputName = currentClip.OutputName;
            newClip.Start = currentClip.Start;
            newClip.thumbnail = currentClip.thumbnail;
            newClip.Volume = currentClip.Volume;
            currentClip = null;
            currentClip = newClip;

            Console.WriteLine("Adding clip {0}-{1} ", currentClip.Start, currentClip.End);
            checkFileForDuplicate();
            currentClip.Length = currentClip.End - currentClip.Start;
            // Add the first thumbnail we generated on load if we didn't grab a thumbnail at starting frame
            if(currentClip.thumbnail == null)
            {
                Console.WriteLine("No thumbnail detected on current clip, trying to add thumbnail from initial load");
                if(currentClip.filePath == currentStartThumbFilename) // Only add thumbnail from start if we're still dealing with same file
                {
                    currentClip.thumbnail = currentStartThumbnail;
                }
            }
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
            // generate thumbnail for manager window
            AddThumbnailToCurrentClip(false);

            currentClip.Start = ffmePlayer.Position;
            currentClip.Length = currentClip.End - currentClip.Start;
            Console.WriteLine("Selecting clip start at " + currentClip.Start);
            playerTimeSliderCustom.StartSlider.Value = currentClip.Start.TotalMilliseconds;
            //Console.WriteLine("startslider value: " + playerTimeSliderCustom.StartSlider.Value);
        }

        private void AddThumbnailToCurrentClip(bool startThumbnail)
        {
            Size playerSize = ffmePlayer.RenderSize;
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)playerSize.Width, (int)playerSize.Height, 96, 96, PixelFormats.Default);
            ffmePlayer.Measure(ffmePlayer.RenderSize);
            ffmePlayer.Arrange(new Rect(new Point(0, 0), ffmePlayer.RenderSize));
            ffmePlayer.UpdateLayout();

            bmp.Render(ffmePlayer);
            PngBitmapEncoder img = new PngBitmapEncoder();
            img.Frames.Add(BitmapFrame.Create(bmp));
            BitmapImage bmpImg = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);

                bmpImg.BeginInit();
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.StreamSource = ms;
                bmpImg.EndInit();
            }
            if(startThumbnail)
            {
                currentStartThumbFilename = currentClip.filePath;
                Console.WriteLine("Adding start thumbnail for file " + currentStartThumbFilename);
                currentStartThumbnail = bmpImg;
            }
            else
                currentClip.thumbnail = bmpImg;
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
            togglePlayerPause(false);
        }

        private void togglePlayerPause(bool forcePause)
        {
            if(forcePause)
            {
                buttonPauseToggle.Content = "▶️ Resume";
                ffmePlayer.Pause();
                return;
            }

            buttonPauseToggle.Content = (ffmePlayer.IsPaused) ? "⏸ Pause" : "▶️ Resume";
            if(ffmePlayer.IsPaused)
                ffmePlayer.Play();
            else
                ffmePlayer.Pause();
        }

        private void FfmePlayer_PositionChanged(object sender, Unosquare.FFME.Common.PositionChangedEventArgs e)
        {
            //Console.WriteLine("Current pos: " + e.Position.TotalMilliseconds);
            double currentPosition = 0;
            //Console.WriteLine("Media position: " + e.Position);
            if (e.Position.TotalMilliseconds > playerTimeSliderCustom.CurrentSlider.Maximum)
                currentPosition = playerTimeSliderCustom.CurrentSlider.Maximum;
            else if (e.Position.TotalMilliseconds < 0)
                currentPosition = 0;
            else
                currentPosition = e.Position.TotalMilliseconds;

            // Add a thumbnail from start of the file if we don't have a thumbnail yet
            // FIXME: Find a way to check if the player is showing any video frames yet, this method is unreliable. (results in gray/empty previews sometimes)
            //  within 10-100 ms
            if(currentPosition >= 10 && currentPosition < 100 && ffmePlayer.RenderSize.Width > 0 && ffmePlayer.RenderSize.Height > 0)
            {
                if(currentStartThumbnail == null)
                {
                    AddThumbnailToCurrentClip(true);
                }
            }

            // ignore valuechanged event for player position update
            playerTimeSliderCustom.CurrentSlider.ValueChanged -= CurrentSlider_ValueChanged;
            playerTimeSliderCustom.CurrentSlider.Value = currentPosition;
            playerTimeSliderCustom.CurrentSlider.ValueChanged += CurrentSlider_ValueChanged;
        }

        private void CurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Console.WriteLine("Slider click at {0}, changing player position..", e.NewValue);
            ffmePlayer.Position = TimeSpan.FromMilliseconds(e.NewValue);
        }
        private void FfmePlayer_MediaReady(object sender, EventArgs e)
        {
            Console.WriteLine("Media ready to play");
            mediaReadyToPlay = true;

            if (requestClipLoad)
            {
                UpdateInterface();
                ffmePlayer.Position = currentClip.Start;
            }
            else
            {
                resetInterface();
                // Console.WriteLine("Codec: " + ffmePlayer.VideoCodec);
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

            /*if(requestClipLoad)
            {
                currentClip.Start = TimeSpan.Zero;
                currentClip.End = ffmePlayer.MediaInfo.Duration;
                currentClip.Length = ffmePlayer.MediaInfo.Duration;
                Console.WriteLine("Setting clip reset len: " + currentClip.Length);
                return;
            }*/
            if(!requestClipLoad)
                currentClip = new VideoClip();
            currentClip.filePath = ffmePlayer.MediaInfo.MediaSource;
            currentClip.OutputName = System.IO.Path.GetFileName(currentClip.filePath);
            currentClip.Volume = 100;
            currentClip.Encode = true;
            currentClip.Start = TimeSpan.Zero;
            currentClip.End = ffmePlayer.MediaInfo.Duration;
            currentClip.Length = ffmePlayer.MediaInfo.Duration;

            currentClip.inputFileCodec = ffmePlayer.VideoCodec;
            Console.WriteLine("has audio : " + ffmePlayer.HasAudio);
            currentClip.HasAudio = ffmePlayer.HasAudio;
            if (ffmePlayer.MediaInfo.Streams.Count >= 3) // Has extra audio tracks/streams
            {
                currentClip.MultiTrackAudio = true;
                currentClip.MergeAudioTracks = true;
                Console.WriteLine("Multiple audio tracks detected");
            }
        }

        private void checkFileForDuplicate()
        {
            // Force mp4 output for now
            // TODO: Add proper format selection
            string outputFormat = ".mp4";

            if (System.IO.Path.GetExtension(currentClip.OutputName) != outputFormat)
            {
                Console.WriteLine("File extension not {1}, changing.. ({0})", currentClip.OutputName, outputFormat);
                currentClip.OutputName = System.IO.Path.GetFileNameWithoutExtension(currentClip.OutputName); // This does not check the path properly, should be directoryName + "\\" + currentClip.OutputName?
                currentClip.OutputName += outputFormat;
            }
            Console.WriteLine("Checking name for duplicate {0}", currentClip.OutputName);

            string directoryName = Environment.CurrentDirectory; //System.IO.Path.GetDirectoryName(currentClip.filePath);
            directoryName += "\\videos";

            int fileCount = 1;
            while(File.Exists(directoryName + "\\" + currentClip.OutputName))
            {
                Console.WriteLine("File exists: {0}, renaming to {1}", currentClip.OutputName, fileCount + "_" + System.IO.Path.GetFileNameWithoutExtension(currentClip.filePath) + outputFormat);
                currentClip.OutputName = fileCount + "_" + System.IO.Path.GetFileNameWithoutExtension(currentClip.filePath) + outputFormat;
                fileCount++;
            }
            while(videoClips.Any(item => item.OutputName == currentClip.OutputName))
            {
                Console.WriteLine("Name used in list: {0}, renaming to {1}", currentClip.OutputName, fileCount + "_" + System.IO.Path.GetFileNameWithoutExtension(currentClip.filePath) + outputFormat);
                currentClip.OutputName = fileCount + "_" + System.IO.Path.GetFileNameWithoutExtension(currentClip.filePath) + outputFormat;
                fileCount++;
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
            currentStartThumbnail = null;

            ffmePlayer.Source = new Uri(videoPath);
            ffmePlayer.Play();
        }
    }
}
