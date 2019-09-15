namespace JVT
{
    partial class Form1
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
            this.panelPlayer = new System.Windows.Forms.Panel();
            this.buttonPlayerStop = new System.Windows.Forms.Button();
            this.hScrollBarPlayer = new System.Windows.Forms.HScrollBar();
            this.trackBarPlayer = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // panelPlayer
            // 
            this.panelPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPlayer.Location = new System.Drawing.Point(0, 0);
            this.panelPlayer.Name = "panelPlayer";
            this.panelPlayer.Size = new System.Drawing.Size(800, 387);
            this.panelPlayer.TabIndex = 0;
            // 
            // buttonPlayerStop
            // 
            this.buttonPlayerStop.Location = new System.Drawing.Point(344, 421);
            this.buttonPlayerStop.Name = "buttonPlayerStop";
            this.buttonPlayerStop.Size = new System.Drawing.Size(75, 23);
            this.buttonPlayerStop.TabIndex = 1;
            this.buttonPlayerStop.Text = "Pause";
            this.buttonPlayerStop.UseVisualStyleBackColor = true;
            this.buttonPlayerStop.Click += new System.EventHandler(this.ButtonPlayerStop_Click);
            // 
            // hScrollBarPlayer
            // 
            this.hScrollBarPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBarPlayer.LargeChange = 2;
            this.hScrollBarPlayer.Location = new System.Drawing.Point(9, 390);
            this.hScrollBarPlayer.Name = "hScrollBarPlayer";
            this.hScrollBarPlayer.Size = new System.Drawing.Size(782, 19);
            this.hScrollBarPlayer.TabIndex = 2;
            this.hScrollBarPlayer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HScrollBarPlayer_Scroll);
            // 
            // trackBarPlayer
            // 
            this.trackBarPlayer.Location = new System.Drawing.Point(476, 398);
            this.trackBarPlayer.Maximum = 100;
            this.trackBarPlayer.Name = "trackBarPlayer";
            this.trackBarPlayer.Size = new System.Drawing.Size(104, 45);
            this.trackBarPlayer.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 441);
            this.Controls.Add(this.trackBarPlayer);
            this.Controls.Add(this.hScrollBarPlayer);
            this.Controls.Add(this.buttonPlayerStop);
            this.Controls.Add(this.panelPlayer);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPlayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelPlayer;
        private System.Windows.Forms.Button buttonPlayerStop;
        private System.Windows.Forms.HScrollBar hScrollBarPlayer;
        private System.Windows.Forms.TrackBar trackBarPlayer;
    }
}

