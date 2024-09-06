using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Main.Properties;
using System.Drawing.Imaging;
using System.Runtime;

namespace Main
{
	public partial class MainForm : Form
	{
		Bitmap bm;
		Rectangle rect;
		int outputWidth;
		int outputHeight;
		Settings settings;

		public MainForm()
		{
			outputHeight = 200;
			outputWidth = 320;

			settings = Settings.LoadSettings(Logging.Config.AppDataPath, Directory.GetCurrentDirectory(), Logging.Config.SaveBasePath);
			settings.Set();

			InitializeComponent();

			if (settings.CurseOfTheAzureBondsDataPath.Length > 0)
			{
				this.dataToolStripMenuItem1.Text += " - " + settings.CurseOfTheAzureBondsDataPath;
			}

			if (settings.CurseOfTheAzureBondsSavePath.Length > 0)
			{
				this.saveToolStripMenuItem1.Text += " - " + settings.CurseOfTheAzureBondsSavePath;
			}

			bm = new Bitmap(outputWidth, outputHeight, PixelFormat.Format24bppRgb);
			rect = new Rectangle(0, 0, outputWidth, outputHeight);

			Classes.Display.UpdateCallback = UpdateDisplayCallback;
		}

		object obj = new object();

		public void UpdateDisplayCallback(byte[] videoRam, int videoRamSize)
		{
			BitmapData bmpData =
				bm.LockBits(rect, ImageLockMode.WriteOnly,
				PixelFormat.Format24bppRgb);

			IntPtr ptr = bmpData.Scan0;

			System.Runtime.InteropServices.Marshal.Copy(videoRam, 0, ptr, videoRamSize);

			bm.UnlockBits(bmpData);

			displayArea.Invoke(new MethodInvoker(UpdateDisplayCallback));
		}

		void UpdateDisplayCallback()
		{
			displayArea.Image = (Image)bm.Clone();
		}

		private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.F5)
			{
				Classes.Display.ForceUpdate();
			}

			engine.seg049.AddKey(Keyboard.KeyToIBMKey(e.KeyCode));
		}

		private void commandDebugToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			engine.seg043.ToggleCommandDebugging();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			engine.seg043.print_and_exit();
		}

		private void dumpPlayerAffectsToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			engine.seg043.DumpPlayerAffects();
		}

		private void commandDebuggingToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			engine.seg043.ToggleCommandDebugging();
		}

		string Picture_Prefix = "Curse - ";

		private void screenCaptureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			int largest = 0;
			foreach (string filename in Directory.GetFiles(path, Picture_Prefix + "*.png", SearchOption.TopDirectoryOnly))
			{
				int num;
				string substr = Path.GetFileNameWithoutExtension(filename).Substring(Picture_Prefix.Length);
				if (Int32.TryParse(substr, out num))
				{
					largest = Math.Max(num, largest);
				}
			}
			largest++;

			string newfilepath = Path.Combine(path, Picture_Prefix + largest.ToString("D4") + ".png");
			displayArea.Image.Save(newfilepath, System.Drawing.Imaging.ImageFormat.Png);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			lock (obj)
			{
				base.OnPaint(e);
			}
		}

		private void playersAlwayMakeSavingThrowToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			bool flipped = !settings.PlayerAlwaysSaves;
			settings.PlayerAlwaysSaves = flipped;
		}

		private void alwayAllowAreaMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settings.AlwaysShowAreaMap = !settings.AlwaysShowAreaMap;
		}

		private void freeTrainingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.FreeTraining;
			settings.FreeTraining = flipped;
		}

		private void skipCopyProtectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.SkipCopyProtection;
			settings.SkipCopyProtection = flipped;
		}

		private void skipTitleScreenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.SkipTitleScreen;
			settings.SkipTitleScreen = flipped;
		}

		private void allowPlayerModifyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.AllowPlayerModify;
			settings.AllowPlayerModify = flipped;
		}

		private void allowGodsInterveneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.AllowGodsIntervene;
			settings.AllowGodsIntervene = flipped;
		}

		private void displayItemsFullNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.DisplayFullItemNames;
			settings.DisplayFullItemNames = flipped;
		}

		private void viewItemsStatsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.ViewItemsStats;
			settings.ViewItemsStats = flipped;
		}

		private void improvedAreaMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.ImprovedAreaMap;
			settings.ImprovedAreaMap = flipped;
		}

		private void noRaceLevelLimitsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.NoRaceLevelLimits;
			settings.NoRaceLevelLimits = flipped;
		}

		private void noRaceClassLimitsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.NoRaceClassLimits;
			settings.NoRaceClassLimits = flipped;
		}

		private void dumpMonstersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			engine.seg043.DumpMonsters();
		}

		private void dumpTreasureItemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			engine.seg043.DumpTreasureItems();
		}

		private void soundOnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.SoundOn;
			settings.SoundOn = flipped;
		}

		private void PictureOnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.PictureOn;
			settings.PictureOn = flipped;
			if (flipped == false)
			{
				settings.AnimationOn = false;
			}
		}

		private void AnimationOnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.AnimationOn;
			settings.AnimationOn = flipped;
		}

		private void sortTreasureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !settings.SortTreasure;
			settings.SortTreasure = flipped;
		}

		private void DataToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = settings.CurseOfTheAzureBondsDataPath;
			var result = folderBrowserDialog1.ShowDialog();
			if (!result.Equals(DialogResult.OK)) return;
			settings.CurseOfTheAzureBondsDataPath = folderBrowserDialog1.SelectedPath;
			dataToolStripMenuItem1.Text = "Data - " + settings.CurseOfTheAzureBondsDataPath;
		}

		private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = settings.CurseOfTheAzureBondsSavePath;
			var result = folderBrowserDialog1.ShowDialog();
			if (!result.Equals(DialogResult.OK)) return;
			settings.CurseOfTheAzureBondsSavePath = folderBrowserDialog1.SelectedPath;
			saveToolStripMenuItem1.Text = "Save - " + settings.CurseOfTheAzureBondsSavePath;
		}
	}
}