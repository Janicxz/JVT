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
            encoder.Encode(clips);
        }
    }
}
