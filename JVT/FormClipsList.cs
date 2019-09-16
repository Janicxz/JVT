using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JVT
{
    public partial class FormClipList : Form
    {
        private List<VideoClip> clips;
        ClipEncoder encoder;
        public FormClipList(List<VideoClip> clipsList)
        {
            InitializeComponent();
            clips = clipsList;
            encoder = new ClipEncoder();
            encoder.EncodingStatusChanged += Encoder_EncodingStatusChanged;
            progressBarEncoder.Maximum = clips.Count()+1;

            comboBoxResolution.SelectedIndex = 0;
            comboBoxBitrate.SelectedIndex = 0;
            comboBoxFps.SelectedIndex = 0;
        }

        private void Encoder_EncodingStatusChanged(int ClipsEncoded, bool Finished)
        {
            progressBarEncoder.Value = ClipsEncoded;
            if(Finished)
            {
                MessageBox.Show("Encoding finished!");
                clips.Clear();
                this.Close();
            }
        }

        private void FormClipList_Load(object sender, EventArgs e)
        {
            foreach(VideoClip clip in clips)
            {
                dataGridViewClips.Rows.Add(new object[] {true,false,clip.OutputName,clip.Start,clip.End});
            }
        }

        private void ButtonEncode_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dataGridViewClips.Rows)
            {
                if((bool)row.Cells["Merge"].Value)
                {
                    foreach (VideoClip clip in clips)
                    {
                        if(clip.OutputName == (string)row.Cells["outputFilename"].Value)
                        {
                            clip.Merge = true;
                        }
                    }
                }
            }
            EncoderSettings cfg = new EncoderSettings();
            try
            {
                cfg.Width = Int32.Parse(comboBoxResolution.SelectedItem.ToString().Split('x')[0]);
                cfg.Height = Int32.Parse(comboBoxResolution.SelectedItem.ToString().Split('x')[1]);
                cfg.Bitrate = Int32.Parse(comboBoxBitrate.SelectedItem.ToString());
                cfg.FPS = Int32.Parse(comboBoxFps.SelectedItem.ToString());
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Bad values inserted in encoder settings!\n Encoding cancelled.");
                return;
            }
            Console.WriteLine("Passing settings: {0}x{1} {2}kbps {3}fps", cfg.Width, cfg.Height, cfg.Bitrate, cfg.FPS);
            encoder.Encode(clips, cfg);
        }
    }
}
