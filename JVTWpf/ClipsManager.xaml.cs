using System;
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
            int resW, resH, bitrate, framerate;
            try
            {
                resW = int.Parse(comboBoxResolution.Text.Split('x')[0]);
                resH = int.Parse(comboBoxResolution.Text.Split('x')[1]);
                bitrate = int.Parse(comboBoxBitrate.Text);
                framerate = int.Parse(comboBoxFPS.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("buttonEncode: " + ex.Message);
                System.Windows.Forms.MessageBox.Show("Invalid encoder settings detected, canceling encoding.");
                return;
            }
            encoder = new FFmpegEncoder(videoClips);
            Console.WriteLine("Setting encoding values: {0}, {1}, {2}", resW + "x" + resH, bitrate, framerate);
            encoder.Encode(resW, resH, bitrate, framerate, (bool)checkBoxHardwareAccel.IsChecked);
            //System.Windows.Forms.MessageBox.Show("Encoding finished! \n" +
            //    "Encoded clips can be found in " + Environment.CurrentDirectory + "\\videos");
            DialogResult result = System.Windows.Forms.MessageBox.Show("Encoding finished.\n Open the output folder?", "Encoder message", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if(result == System.Windows.Forms.DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo() {FileName = Environment.CurrentDirectory + "\\videos", UseShellExecute = true, Verb = "open" });
            }
            //videoClips.Clear();
            //dataGridClips.Items.Refresh();
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
