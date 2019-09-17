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
        private int clipsEncodeCount;
        ClipEncoder encoder;
        public FormClipList(List<VideoClip> clipsList)
        {
            InitializeComponent();
            clips = clipsList;
            encoder = new ClipEncoder();
            encoder.EncodingStatusChanged += Encoder_EncodingStatusChanged;
            //progressBarEncoder.Maximum = clips.Count()+1;

            comboBoxResolution.SelectedIndex = 0;
            comboBoxBitrate.SelectedIndex = 0;
            comboBoxFps.SelectedIndex = 0;
        }

        private void Encoder_EncodingStatusChanged(int ClipsEncoded, bool Finished)
        {
            progressBarEncoder.Invoke((Action)delegate
            {
                progressBarEncoder.Value = ClipsEncoded;

            });
            labelProgress.Invoke((Action)delegate
            {
                labelProgress.Text = String.Format("Progress: {0}/{1}", ClipsEncoded, clipsEncodeCount);
            });
            
            //progressBarEncoder.Value = ClipsEncoded;
            if (Finished)
            {
                MessageBox.Show("Encoding finished!");
                this.Invoke((Action)delegate
                {
                    clips.Clear();
                    this.Close();
                });
            }
        }

        private void FormClipList_Load(object sender, EventArgs e)
        {
            foreach(VideoClip clip in clips)
            {
                dataGridViewClips.Rows.Add(new object[] {true,clip.Volume,clip.MultiTrackAudio,false,clip.OutputName,clip.Start,clip.End});
                if (!clip.MultiTrackAudio)
                    dataGridViewClips.Rows[dataGridViewClips.Rows.Count-1].Cells["ColumnMicTrack"].ReadOnly = true;
            }
        }

        private void ButtonEncode_Click(object sender, EventArgs e)
        {
            bool addedMergeClip = false;
            foreach(DataGridViewRow row in dataGridViewClips.Rows)
            {
                foreach (VideoClip clip in clips)
                {
                    if ((bool)row.Cells["Merge"].Value)
                    {
                        if (clip.OutputName == (string)row.Cells["outputFilename"].Value)
                        {
                            clip.Merge = true;
                            if(!addedMergeClip)
                            {
                                clipsEncodeCount++;
                                addedMergeClip = true;
                            }
                        }
                    }
                    if ((bool)row.Cells["ColumnMicTrack"].Value)
                    {
                        if (clip.OutputName == (string)row.Cells["outputFilename"].Value)
                        {
                            clip.MergeAudioTracks = true;
                        }
                    }
                    if (clip.OutputName == (string)row.Cells["outputFilename"].Value)
                    {
                        clip.Volume = Int32.Parse(row.Cells["ColumnVolume"].Value.ToString());
                    }
                    if (clip.OutputName == (string)row.Cells["outputFilename"].Value)
                    {
                        clip.Encode = (bool)row.Cells["Encode"].Value;
                        clipsEncodeCount++;
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
            // run this in a separate thread so we don't freeze the UI
            buttonEncode.Enabled = false;
            buttonEncode.Text = "Encoding..";
            progressBarEncoder.Maximum = clipsEncodeCount;
            Task.Run(() => encoder.Encode(clips, cfg));
            //encoder.Encode(clips, cfg);
        }
    }
}
