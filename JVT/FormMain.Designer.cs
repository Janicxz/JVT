namespace JVT
{
    partial class FormMain
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
            this.buttonPlayerStop = new System.Windows.Forms.Button();
            this.trackBarPlayer = new System.Windows.Forms.TrackBar();
            this.labelMark1 = new System.Windows.Forms.Label();
            this.buttonClipStart = new System.Windows.Forms.Button();
            this.buttonClipEnd = new System.Windows.Forms.Button();
            this.buttonRender = new System.Windows.Forms.Button();
            this.labelMarkEnd = new System.Windows.Forms.Label();
            this.buttonAddClip = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelVlcPlayer = new System.Windows.Forms.Panel();
            this.buttonMute = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPlayer)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPlayerStop
            // 
            this.buttonPlayerStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlayerStop.Location = new System.Drawing.Point(343, 415);
            this.buttonPlayerStop.Name = "buttonPlayerStop";
            this.buttonPlayerStop.Size = new System.Drawing.Size(75, 23);
            this.buttonPlayerStop.TabIndex = 1;
            this.buttonPlayerStop.TabStop = false;
            this.buttonPlayerStop.Text = "Pause";
            this.buttonPlayerStop.UseVisualStyleBackColor = true;
            this.buttonPlayerStop.Click += new System.EventHandler(this.ButtonPlayerStop_Click);
            // 
            // trackBarPlayer
            // 
            this.trackBarPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarPlayer.Location = new System.Drawing.Point(12, 364);
            this.trackBarPlayer.Maximum = 100;
            this.trackBarPlayer.Name = "trackBarPlayer";
            this.trackBarPlayer.Size = new System.Drawing.Size(776, 45);
            this.trackBarPlayer.TabIndex = 3;
            this.trackBarPlayer.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarPlayer.Scroll += new System.EventHandler(this.TrackBarPlayer_Scroll);
            this.trackBarPlayer.SizeChanged += new System.EventHandler(this.TrackBarPlayer_SizeChanged);
            // 
            // labelMark1
            // 
            this.labelMark1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMark1.AutoSize = true;
            this.labelMark1.BackColor = System.Drawing.Color.Transparent;
            this.labelMark1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMark1.ForeColor = System.Drawing.Color.DarkRed;
            this.labelMark1.Location = new System.Drawing.Point(12, 384);
            this.labelMark1.Name = "labelMark1";
            this.labelMark1.Size = new System.Drawing.Size(17, 25);
            this.labelMark1.TabIndex = 4;
            this.labelMark1.Text = "I";
            // 
            // buttonClipStart
            // 
            this.buttonClipStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClipStart.Location = new System.Drawing.Point(27, 415);
            this.buttonClipStart.Name = "buttonClipStart";
            this.buttonClipStart.Size = new System.Drawing.Size(75, 23);
            this.buttonClipStart.TabIndex = 5;
            this.buttonClipStart.TabStop = false;
            this.buttonClipStart.Text = "Clip Start";
            this.buttonClipStart.UseVisualStyleBackColor = true;
            this.buttonClipStart.Click += new System.EventHandler(this.ButtonClipStart_Click);
            // 
            // buttonClipEnd
            // 
            this.buttonClipEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClipEnd.Location = new System.Drawing.Point(108, 415);
            this.buttonClipEnd.Name = "buttonClipEnd";
            this.buttonClipEnd.Size = new System.Drawing.Size(75, 23);
            this.buttonClipEnd.TabIndex = 6;
            this.buttonClipEnd.TabStop = false;
            this.buttonClipEnd.Text = "Clip End";
            this.buttonClipEnd.UseVisualStyleBackColor = true;
            this.buttonClipEnd.Click += new System.EventHandler(this.ButtonClipEnd_Click);
            // 
            // buttonRender
            // 
            this.buttonRender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRender.Location = new System.Drawing.Point(635, 415);
            this.buttonRender.Name = "buttonRender";
            this.buttonRender.Size = new System.Drawing.Size(94, 24);
            this.buttonRender.TabIndex = 7;
            this.buttonRender.TabStop = false;
            this.buttonRender.Text = "Encoder";
            this.buttonRender.UseVisualStyleBackColor = true;
            this.buttonRender.Click += new System.EventHandler(this.ButtonRender_Click);
            // 
            // labelMarkEnd
            // 
            this.labelMarkEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMarkEnd.AutoSize = true;
            this.labelMarkEnd.BackColor = System.Drawing.Color.Transparent;
            this.labelMarkEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMarkEnd.Location = new System.Drawing.Point(771, 384);
            this.labelMarkEnd.Name = "labelMarkEnd";
            this.labelMarkEnd.Size = new System.Drawing.Size(17, 25);
            this.labelMarkEnd.TabIndex = 8;
            this.labelMarkEnd.Text = "I";
            // 
            // buttonAddClip
            // 
            this.buttonAddClip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddClip.Location = new System.Drawing.Point(189, 415);
            this.buttonAddClip.Name = "buttonAddClip";
            this.buttonAddClip.Size = new System.Drawing.Size(75, 23);
            this.buttonAddClip.TabIndex = 9;
            this.buttonAddClip.Text = "Add Clip";
            this.buttonAddClip.UseVisualStyleBackColor = true;
            this.buttonAddClip.Click += new System.EventHandler(this.ButtonAddClip_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadVideoToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadVideoToolStripMenuItem
            // 
            this.loadVideoToolStripMenuItem.Name = "loadVideoToolStripMenuItem";
            this.loadVideoToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.loadVideoToolStripMenuItem.Text = "Load Video";
            this.loadVideoToolStripMenuItem.Click += new System.EventHandler(this.LoadVideoToolStripMenuItem_Click);
            // 
            // panelVlcPlayer
            // 
            this.panelVlcPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVlcPlayer.Location = new System.Drawing.Point(12, 27);
            this.panelVlcPlayer.Name = "panelVlcPlayer";
            this.panelVlcPlayer.Size = new System.Drawing.Size(776, 331);
            this.panelVlcPlayer.TabIndex = 12;
            // 
            // buttonMute
            // 
            this.buttonMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMute.Location = new System.Drawing.Point(540, 416);
            this.buttonMute.Name = "buttonMute";
            this.buttonMute.Size = new System.Drawing.Size(75, 23);
            this.buttonMute.TabIndex = 13;
            this.buttonMute.Text = "Mute audio";
            this.buttonMute.UseVisualStyleBackColor = true;
            this.buttonMute.Click += new System.EventHandler(this.ButtonMute_Click);
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 453);
            this.Controls.Add(this.buttonMute);
            this.Controls.Add(this.panelVlcPlayer);
            this.Controls.Add(this.buttonAddClip);
            this.Controls.Add(this.labelMarkEnd);
            this.Controls.Add(this.buttonRender);
            this.Controls.Add(this.buttonClipEnd);
            this.Controls.Add(this.buttonClipStart);
            this.Controls.Add(this.labelMark1);
            this.Controls.Add(this.trackBarPlayer);
            this.Controls.Add(this.buttonPlayerStop);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "JVT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPlayer)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonPlayerStop;
        private System.Windows.Forms.TrackBar trackBarPlayer;
        private System.Windows.Forms.Label labelMark1;
        private System.Windows.Forms.Button buttonClipStart;
        private System.Windows.Forms.Button buttonClipEnd;
        private System.Windows.Forms.Button buttonRender;
        private System.Windows.Forms.Label labelMarkEnd;
        private System.Windows.Forms.Button buttonAddClip;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadVideoToolStripMenuItem;
        private System.Windows.Forms.Panel panelVlcPlayer;
        private System.Windows.Forms.Button buttonMute;
    }
}

