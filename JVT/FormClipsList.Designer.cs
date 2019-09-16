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
            this.Encode = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Merge = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.outputFilename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clipStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clipEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonEncode = new System.Windows.Forms.Button();
            this.progressBarEncoder = new System.Windows.Forms.ProgressBar();
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
            // Encode
            // 
            this.Encode.HeaderText = "Encode";
            this.Encode.Name = "Encode";
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
            // buttonEncode
            // 
            this.buttonEncode.Location = new System.Drawing.Point(328, 387);
            this.buttonEncode.Name = "buttonEncode";
            this.buttonEncode.Size = new System.Drawing.Size(75, 23);
            this.buttonEncode.TabIndex = 1;
            this.buttonEncode.Text = "Encode";
            this.buttonEncode.UseVisualStyleBackColor = true;
            this.buttonEncode.Click += new System.EventHandler(this.ButtonEncode_Click);
            // 
            // progressBarEncoder
            // 
            this.progressBarEncoder.Location = new System.Drawing.Point(316, 415);
            this.progressBarEncoder.Name = "progressBarEncoder";
            this.progressBarEncoder.Size = new System.Drawing.Size(100, 23);
            this.progressBarEncoder.TabIndex = 2;
            // 
            // FormClipList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.progressBarEncoder);
            this.Controls.Add(this.buttonEncode);
            this.Controls.Add(this.dataGridViewClips);
            this.Name = "FormClipList";
            this.Text = "Clips";
            this.Load += new System.EventHandler(this.FormClipList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClips)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewClips;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Encode;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Merge;
        private System.Windows.Forms.DataGridViewTextBoxColumn outputFilename;
        private System.Windows.Forms.DataGridViewTextBoxColumn clipStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn clipEnd;
        private System.Windows.Forms.Button buttonEncode;
        private System.Windows.Forms.ProgressBar progressBarEncoder;
    }
}