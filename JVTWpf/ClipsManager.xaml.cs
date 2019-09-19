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
            encoder = new FFmpegEncoder(videoClips);
            encoder.Encode(1920, 1080, 12000, 60); // 1920x1080 12mbit 60 fps // TODO: Add UI for these
            System.Windows.Forms.MessageBox.Show("Encoding finished!");
            //videoClips.Clear();
            dataGridClips.Items.Refresh();
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
