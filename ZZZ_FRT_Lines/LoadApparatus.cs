using AxWMPLib;
using System;
using System.Windows.Forms;

namespace ZZZ_FRT_Lines
{
    public partial class LoadApparatus : Form
    {
        private AxWindowsMediaPlayer axWindowsMediaPlayer1;
        public LoadApparatus()
        {
            InitializeComponent();
            this.CenterToScreen();
            //InitializeMediaPlayer();
        }

        private void InitializeMediaPlayer()
        {
            this.axWindowsMediaPlayer1 = new AxWindowsMediaPlayer();
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";

            // Set up an event handler to configure the media player once it's created
            this.axWindowsMediaPlayer1.CreateControl(); // Ensure the control is created
            this.axWindowsMediaPlayer1.HandleCreated += new EventHandler(AxWindowsMediaPlayer1_HandleCreated);
        }

        private void AxWindowsMediaPlayer1_HandleCreated(object sender, EventArgs e)
        {
            // Configure the media player
            axWindowsMediaPlayer1.uiMode = "none"; // Hide controls
            axWindowsMediaPlayer1.settings.setMode("loop", true); // Enable looping
            axWindowsMediaPlayer1.URL = "C:\\Users\\adodge\\Documents\\Visual Studio 2022\\The Autopart Apparatus\\ZZZ_FRT_Lines\\Apparatus_Load.mp4"; // Set the video file path
            axWindowsMediaPlayer1.settings.volume = 0; // Mute the sound
            axWindowsMediaPlayer1.Ctlcontrols.play(); // Start playing the video
            axWindowsMediaPlayer1.stretchToFit = true; // Stretch video to fit the form
            axWindowsMediaPlayer1.SendToBack(); // Send the media player to the back
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Ensure the media player control is behind all other controls
            axWindowsMediaPlayer1.SendToBack();
        }
    }
}
