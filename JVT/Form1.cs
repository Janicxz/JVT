using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Mpv.NET.Player;

namespace JVT
{
    public partial class Form1 : Form
    {
        string dllPath = "lib\\mpv-1.dll";
        MpvPlayer player;
        Timer timerScrollBarUpdate;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Init
            player = new MpvPlayer(panelPlayer.Handle, dllPath);
            player.Load("test.mp4");
            player.Loop = true;
            player.Resume();
            player.MediaLoaded += Player_MediaLoaded;
            player.MediaPaused += Player_MediaPaused;
            player.MediaResumed += Player_MediaResumed;

            timerScrollBarUpdate = new Timer();
            timerScrollBarUpdate.Interval = 100;
            timerScrollBarUpdate.Tick += TimerScrollBarUpdate_Tick;
            timerScrollBarUpdate.Start();
        }

        private void Player_MediaResumed(object sender, EventArgs e)
        {
            Console.WriteLine("playback resumed");
            timerScrollBarUpdate.Start();
        }

        private void Player_MediaPaused(object sender, EventArgs e)
        {
            Console.WriteLine("Playback paused.");
            timerScrollBarUpdate.Stop();
        }

        private void TimerScrollBarUpdate_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("timer Pos {0}, dur {1}", player.Position, player.Duration);
            hScrollBarPlayer.Invoke((Action)delegate
            {
                hScrollBarPlayer.Value = (int)player.Position.TotalMilliseconds;
                //Application.DoEvents();
            });
        }

        private void Player_PositionChanged(object sender, MpvPlayerPositionChangedEventArgs e)
        {
            hScrollBarPlayer.Invoke((Action)delegate
            {
                hScrollBarPlayer.Value = (int)player.Position.TotalMilliseconds;
                //Application.DoEvents();
            });
        }


        private void Player_MediaLoaded(object sender, EventArgs e)
        {
            Console.WriteLine("loaded media Dur:" + player.Duration.TotalMilliseconds);
            hScrollBarPlayer.Invoke((Action)delegate
            {
                hScrollBarPlayer.Maximum = (int)player.Duration.TotalMilliseconds;
                hScrollBarPlayer.Minimum = 0;
            });

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
             if(player != null) player.Dispose();
        }

        bool playerPaused = false;
        private void ButtonPlayerStop_Click(object sender, EventArgs e)
        {
            togglePlayerPause();
           // Console.WriteLine("Playing {0}", player.IsPlaying);
           /* if (!playerPaused)
            {
                player.Pause();
                buttonPlayerStop.Text = "Resume";
                playerPaused = true;
            }
            else
            {
                player.Resume();
                buttonPlayerStop.Text = "Pause";
                playerPaused = false;
            }*/
        }

        private void togglePlayerPause()
        {
            if (player.IsPlaying)
            {
                player.Pause();
                buttonPlayerStop.Invoke((Action)delegate
                {
                    buttonPlayerStop.Text = "Resume";
                });
            }
            else
            {
                player.Resume();
                buttonPlayerStop.Invoke((Action)delegate
                {
                    buttonPlayerStop.Text = "Pause";
                });
            }
        }

        private void HScrollBarPlayer_Scroll(object sender, ScrollEventArgs e)
        {
            timerScrollBarUpdate.Stop();
            Console.WriteLine("scrolled");
           // player.SeekAsync(TimeSpan.FromMilliseconds(e.NewValue));
            timerScrollBarUpdate.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Keydown: {0}", e.KeyCode);
            switch (e.KeyCode)
            {
                case Keys.Left:
                    player.PreviousFrame();
                    e.Handled = true;
                    break;
                case Keys.Right:
                    player.NextFrame();
                    e.Handled = true;
                    break;
                case Keys.Space:
                    togglePlayerPause();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }
    }
}
