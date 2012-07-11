using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace RPGAsci
{
	public partial class AudioForm : Form
	{
		static private SoundPlayer soundPlayer;
		public AudioForm()
		{
			InitializeComponent();
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.Load += new EventHandler(AudioForm_Load);
		}
		void AudioForm_Load(object sender, EventArgs e)
		{
			this.Size = new Size(0, 0);
		}

		static public void PlayAudio(string file)
		{
			soundPlayer.SoundLocation = file;
			soundPlayer.Play();
		}


	}
}
