namespace JVT
{
    partial class FormClipList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridViewClips = new System.Windows.Forms.DataGridView();
            this.buttonEncode = new System.Windows.Forms.Button();
            this.progressBarEncoder = new System.Windows.Forms.ProgressBar();
            this.comboBoxResolution = new System.Windows.Forms.ComboBox();
            this.comboBoxBitrate = new System.Windows.Forms.ComboBox();
            this.comboBoxFps = new System.Windows.Forms.ComboBox();
            this.labelRes = new System.Windows.Forms.Label();
            this.labelBitrate = new System.Windows.Forms.Label();
            this.labelFps = new System.Windows.Forms.Label();
            this.Encode = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnMicTrack = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Merge = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.outputFilename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clipStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clipEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClips)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewClips
            // 
            this.dataGridViewClips.AllowUserToAddRows = false;
            this.dataGridViewClips.AllowUserToDeleteRows = false;
            this.dataGridViewClips.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewClips.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClips.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Encode,
            this.ColumnVolume,
            this.ColumnMicTrack,
            this.Merge,
            this.outputFilename,
            this.clipStart,
            this.clipEnd});
            this.dataGridViewClips.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewClips.Name = "dataGridViewClips";
            this.dataGridViewClips.RowHeadersVisible = false;
            this.dataGridViewClips.Size = new System.Drawing.Size(776, 341);
            this.dataGridViewClips.TabIndex = 0;
            // 
            // buttonEncode
            // 
            this.buttonEncode.Location = new System.Drawing.Point(329, 387);
            this.buttonEncode.Name = "buttonEncode";
            this.buttonEncode.Size = new System.Drawing.Size(75, 23);
            this.buttonEncode.TabIndex = 1;
            this.buttonEncode.Text = "Encode";
            this.buttonEncode.UseVisualStyleBackColor = true;
            this.buttonEncode.Click += new System.EventHandler(this.ButtonEncode_Click);
            // 
            // progressBarEncoder
            // 
            this.progressBarEncoder.Location = new System.Drawing.Point(318, 416);
            this.progressBarEncoder.Name = "progressBarEncoder";
            this.progressBarEncoder.Size = new System.Drawing.Size(100, 23);
            this.progressBarEncoder.TabIndex = 2;
            // 
            // comboBoxResolution
            // 
            this.comboBoxResolution.FormattingEnabled = true;
            this.comboBoxResolution.Items.AddRange(new object[] {
            "1920x1080",
            "1280x720"});
            this.comboBoxResolution.Location = new System.Drawing.Point(12, 372);
            this.comboBoxResolution.Name = "comboBoxResolution";
            this.comboBoxResolution.Size = new System.Drawing.Size(121, 21);
            this.comboBoxResolution.TabIndex = 3;
            // 
            // comboBoxBitrate
            // 
            this.comboBoxBitrate.FormattingEnabled = true;
            this.comboBoxBitrate.Items.AddRange(new object[] {
            "12000",
            "7500"});
            this.comboBoxBitrate.Location = new System.Drawing.Point(139, 372);
            this.comboBoxBitrate.Name = "comboBoxBitrate";
            this.comboBoxBitrate.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBitrate.TabIndex = 4;
            // 
            // comboBoxFps
            // 
            this.comboBoxFps.FormattingEnabled = true;
            this.comboBoxFps.Items.AddRange(new object[] {
            "60",
            "30"});
            this.comboBoxFps.Location = new System.Drawing.Point(269, 372);
            this.comboBoxFps.Name = "comboBoxFps";
            this.comboBoxFps.Size = new System.Drawing.Size(37, 21);
            this.comboBoxFps.TabIndex = 5;
            // 
            // labelRes
            // 
            this.labelRes.AutoSize = true;
            this.labelRes.Location = new System.Drawing.Point(12, 356);
            this.labelRes.Name = "labelRes";
            this.labelRes.Size = new System.Drawing.Size(57, 13);
            this.labelRes.TabIndex = 6;
            this.labelRes.Text = "Resolution";
            // 
            // labelBitrate
            // 
            this.labelBitrate.AutoSize = true;
            this.labelBitrate.Location = new System.Drawing.Point(139, 355);
            this.labelBitrate.Name = "labelBitrate";
            this.labelBitrate.Size = new System.Drawing.Size(70, 13);
            this.labelBitrate.TabIndex = 7;
            this.labelBitrate.Text = "Bitrate (Kbps)";
            // 
            // labelFps
            // 
            this.labelFps.AutoSize = true;
            this.labelFps.Location = new System.Drawing.Point(266, 355);
            this.labelFps.Name = "labelFps";
            this.labelFps.Size = new System.Drawing.Size(54, 13);
            this.labelFps.TabIndex = 8;
            this.labelFps.Text = "Framerate";
            // 
            // Encode
            // 
            this.Encode.HeaderText = "Encode";
            this.Encode.Name = "Encode";
            // 
            // ColumnVolume
            // 
            this.ColumnVolume.HeaderText = "Clip Volume (0-250)";
            this.ColumnVolume.Name = "ColumnVolume";
            // 
            // ColumnMicTrack
            // 
            this.ColumnMicTrack.HeaderText = "Include mic audio track";
            this.ColumnMicTrack.Name = "ColumnMicTrack";
            // 
            // Merge
            // 
            this.Merge.HeaderText = "Merge to single video";
            this.Merge.Name = "Merge";
            // 
            // outputFilename
            // 
            this.outputFilename.HeaderText = "Clip filename";
            this.outputFilename.Name = "outputFilename";
            // 
            // clipStart
            // 
            this.clipStart.HeaderText = "Clip start";
            this.clipStart.Name = "clipStart";
            this.clipStart.ReadOnly = true;
            // 
            // clipEnd
            // 
            this.clipEnd.HeaderText = "Clip end";
            this.clipEnd.Name = "clipEnd";
            this.clipEnd.ReadOnly = true;
            // 
            // FormClipList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelFps);
            this.Controls.Add(this.labelBitrate);
            this.Controls.Add(this.labelRes);
            this.Controls.Add(this.comboBoxFps);
            this.Controls.Add(this.comboBoxBitrate);
            this.Controls.Add(this.comboBoxResolution);
            this.Controls.Add(this.progressBarEncoder);
            this.Controls.Add(this.buttonEncode);
            this.Controls.Add(this.dataGridViewClips);
            this.Name = "FormClipList";
            this.Text = "Clips";
            this.Load += new System.EventHandler(this.FormClipList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClips)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewClips;
        private System.Windows.Forms.Button buttonEncode;
        private System.Windows.Forms.ProgressBar progressBarEncoder;
        private System.Windows.Forms.ComboBox comboBoxResolution;
        private System.Windows.Forms.ComboBox comboBoxBitrate;
        private System.Windows.Forms.ComboBox comboBoxFps;
        private System.Windows.Forms.Label labelRes;
        private System.Windows.Forms.Label labelBitrate;
        private System.Windows.Forms.Label labelFps;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Encode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVolume;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnMicTrack;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Merge;
        private System.Windows.Forms.DataGridViewTextBoxColumn outputFilename;
        private System.Windows.Forms.DataGridViewTextBoxColumn clipStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn clipEnd;
    }
}