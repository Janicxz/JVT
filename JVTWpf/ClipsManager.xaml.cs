﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace JVTWpf
{
    /// <summary>
    /// Interaction logic for ClipsManager.xaml
    /// </summary>
    public partial class ClipsManager : Window
    {
        ObservableCollection<VideoClip> videoClips;
        FFmpegEncoder encoder;
        public event EventHandler OnEncodingBegin = delegate { };
        public ClipsManager(ObservableCollection<VideoClip> videoClipsList)
        {
            InitializeComponent();
            DataContext = videoClips;
            videoClips = videoClipsList;
            this.Loaded += ClipsManager_Loaded;
            dataGridClips.Unloaded += DataGridClips_Unloaded;
            // this.Closing += ClipsManager_Closing;
            //dataGridClips.DataContext = videoClips;
            dataGridClips.ItemsSource = videoClips;
            buttonEncode.Click += ButtonEncode_Click;
            dataGridContextDelete.Click += DataGridContextDelete_Click;
            buttonClearClips.Click += ButtonClearClips_Click;
            TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();
        }

        private void DataGridContextDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridClips.SelectedItem == null) return;

            videoClips.Remove((VideoClip)dataGridClips.SelectedItem);
        }

        private void DataGridClips_Unloaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.DataGrid grid = (System.Windows.Controls.DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        //public bool closingApplication = false;

        /*private void ClipsManager_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //dataGridClips.CommitEdit();
            
            if(!closingApplication)
            {
                this.Visibility = Visibility.Hidden;
                e.Cancel = true;
            }
            else
            {
                this.Loaded -= ClipsManager_Loaded;
                dataGridClips.Unloaded -= DataGridClips_Unloaded;
                this.Closing -= ClipsManager_Closing;
                buttonEncode.Click -= ButtonEncode_Click;
                buttonClearClips.Click -= ButtonClearClips_Click;
                base.Close();
            }
        }*/

        private void ButtonClearClips_Click(object sender, RoutedEventArgs e)
        {
            videoClips.Clear();
            RefreshDatagrid();
        }

        public void RefreshDatagrid()
        {
            // dataGridClips.ItemsSource = null;
            //dataGridClips.ItemsSource = videoClips;
            Console.WriteLine("Refreshing datagrid, clips: " + videoClips.Count);
            CollectionViewSource.GetDefaultView(dataGridClips.ItemsSource).Refresh();
           // dataGridClips.Items.Refresh();
        }

        private void ButtonEncode_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Clips " + videoClips);
            if(OnEncodingBegin != null)
            {
                OnEncodingBegin(this, EventArgs.Empty);
            }
            int resW, resH, bitrate, framerate, maxFileSize;
            try
            {
                resW = int.Parse(comboBoxResolution.Text.Split('x')[0]);
                resH = int.Parse(comboBoxResolution.Text.Split('x')[1]);
                bitrate = int.Parse(comboBoxBitrate.Text);
                framerate = int.Parse(comboBoxFPS.Text);
                maxFileSize = int.Parse(comboBoxFileSizeMax.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("buttonEncode: " + ex.Message);
                System.Windows.Forms.MessageBox.Show("Invalid encoder settings detected, canceling encoding.");
                return;
            }
            encoder = new FFmpegEncoder(videoClips);
            Console.WriteLine("Setting encoding values: {0}, {1}, {2}", resW + "x" + resH, bitrate, framerate);
            encoder.OnEncodingProgress += Encoder_OnEncodingProgress;
            encoder.OnEncodingFinished += Encoder_OnEncodingFinished;
            bool hwEncoding = false;
            this.Dispatcher.Invoke(() =>
            {
                hwEncoding = (bool)checkBoxHardwareAccel.IsChecked;
                this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                this.Opacity = 0.4;
                this.buttonEncode.IsEnabled = false;
            });
            if(maxFileSize > 0)
            {
                foreach(VideoClip clip in videoClips)
                {
                    clip.bitRate = (maxFileSize*8192) / (int)clip.Length.TotalSeconds;
                    clip.bitRate -= 384; // Take audio bitrate into account
                    clip.bitRate = clip.bitRate - ((clip.bitRate / 100) * 3);
                }
            }
            Task encodingTask = Task.Run(() => encoder.Encode(resW, resH, bitrate, framerate, hwEncoding));
            //encoder.Encode(resW, resH, bitrate, framerate, (bool)checkBoxHardwareAccel.IsChecked);

            //videoClips.Clear();
            //dataGridClips.Items.Refresh();
        }

        private void Encoder_OnEncodingFinished(object sender, EventArgs e)
        {
            Console.WriteLine("Event: Encoding finished.");
            //System.Windows.Forms.MessageBox.Show("Encoding finished! \n" +
            //    "Encoded clips can be found in " + Environment.CurrentDirectory + "\\videos");
            DialogResult result = System.Windows.Forms.MessageBox.Show("Encoding finished.\n Open the output folder?", "Encoder message", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo() { FileName = Environment.CurrentDirectory + "\\videos", UseShellExecute = true, Verb = "open" });
            }
            this.Dispatcher.Invoke(() =>
            {
                this.Title = "ClipsManager";
                this.Opacity = 1.0;
                this.buttonEncode.IsEnabled = true;
                this.encodingProgressBar.Value = 0.0;
                this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            });
            encoder.OnEncodingFinished -= Encoder_OnEncodingFinished;
            encoder.OnEncodingProgress -= Encoder_OnEncodingProgress;
        }

        private void Encoder_OnEncodingProgress(object sender, EncoderProgressEventArgs e)
        {
            Console.WriteLine("Event: Encoding progress: Current encode: {0}%. Clips encoded: {1}/{2}", e.CurrentClipProcess, e.ClipsEncoded, e.ClipsTotal);
            this.Dispatcher.Invoke(() =>
            {
                this.Title = string.Format("ClipsManager - Clips encoded: {1}/{2}", e.CurrentClipProcess, e.ClipsEncoded, e.ClipsTotal);
                this.encodingProgressBar.Value = (double)e.ClipsEncoded / (double)e.ClipsTotal;
                this.TaskbarItemInfo.ProgressValue = (double)e.ClipsEncoded / (double)e.ClipsTotal;
            });
        }

        private string GetGraphicsCardName()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");
            string graphicsCard = "";
            foreach(ManagementObject obj in searcher.Get())
            {
                foreach(PropertyData property in obj.Properties)
                {
                    if(property.Name == "Description")
                    {
                        graphicsCard = property.Value.ToString();
                    }
                }
            }
            Console.WriteLine("Found graphics card: " + graphicsCard);
            return graphicsCard;
        }

        private void ClipsManager_Loaded(object sender, RoutedEventArgs e)
        {
            string gfxCardName = GetGraphicsCardName();
            if (gfxCardName.ToLower().Contains("nvidia"))
            {
                checkBoxHardwareAccel.IsEnabled = true;
                checkBoxHardwareAccel.Content = string.Format("Use hardware encoding ({0})", gfxCardName);
            }

            RefreshDatagrid();
            /*foreach (VideoClip clip in videoClips)
            {
                Console.WriteLine("adding clip to datagrid");
                dataGridClips.Items.Add(clip);
            }*/
        }
    }
}
