using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        List<VideoClip> videoClips;
        FFmpegEncoder encoder;
        public ClipsManager(List<VideoClip> videoClipsList)
        {
            /*ObservableCollection<VideoClip> clipsList = new ObservableCollection<VideoClip>();
            foreach(VideoClip clip in videoClipsList )
            {
                clipsList.Add(clip);
            }*/

            InitializeComponent();
            videoClips = videoClipsList;
            this.Loaded += ClipsManager_Loaded;
            dataGridClips.Unloaded += DataGridClips_Unloaded;
           // this.Closing += ClipsManager_Closing;
            buttonEncode.Click += ButtonEncode_Click;
            buttonClearClips.Click += ButtonClearClips_Click;
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
            dataGridClips.ItemsSource = null;
            dataGridClips.ItemsSource = videoClips;
           // dataGridClips.Items.Refresh();
        }

        private void ButtonEncode_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Clips " + videoClips);
            int resW, resH, bitrate, framerate;
            try
            {
                resW = int.Parse(((ComboBoxItem)comboBoxResolution.SelectedItem).Content.ToString().Split('x')[0]);
                resH = int.Parse(((ComboBoxItem)comboBoxResolution.SelectedItem).Content.ToString().Split('x')[1]);
                bitrate = int.Parse(((ComboBoxItem)comboBoxBitrate.SelectedItem).Content.ToString());
                framerate = int.Parse(((ComboBoxItem)comboBoxFPS.SelectedItem).Content.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("buttonEncode: " + ex.Message);
                System.Windows.Forms.MessageBox.Show("Invalid encoder settings detected, canceling encoding.");
                return;
            }
            encoder = new FFmpegEncoder(videoClips);
            Console.WriteLine("Setting encoding values: {0}, {1}, {2}", resW + "x" + resH, bitrate, framerate);
            encoder.Encode(resW, resH, bitrate, framerate);
            System.Windows.Forms.MessageBox.Show("Encoding finished! \n" +
                "Encoded clips can be found in " + Environment.CurrentDirectory + "\\videos");
            //videoClips.Clear();
            //dataGridClips.Items.Refresh();
        }

        private void ClipsManager_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDatagrid();
            //dataGridClips.Items.Add(new object[] { });
            //dataGridClips.ItemsSource = videoClips;
            /*foreach (VideoClip clip in videoClips)
            {
                Console.WriteLine("adding clip to datagrid");
                dataGridClips.Items.Add(clip);
            }*/
        }
    }
}
