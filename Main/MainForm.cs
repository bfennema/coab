using Logging;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Main.Properties;

namespace Main
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			setSettings();

			InitializeComponent();

			Classes.Display.UpdateCallback = UpdateDisplayCallback;

			if (Settings.Default.PoolradData.Length > 0)
			{
				this.dataToolStripMenuItem1.Text += " - " + Settings.Default.PoolradData;
			}
			if (Settings.Default.PoolradSave.Length > 0)
			{
				this.saveToolStripMenuItem1.Text += " - " + Settings.Default.PoolradSave;
			}
			if (Settings.Default.CurseData.Length > 0)
			{
				this.dataToolStripMenuItem2.Text += " - " + Settings.Default.CurseData;
			}
			if (Settings.Default.CurseSave.Length > 0)
			{
				this.saveToolStripMenuItem2.Text += " - " + Settings.Default.CurseSave;
			}
			if (Settings.Default.SecretData.Length > 0)
			{
				this.dataToolStripMenuItem3.Text += " - " + Settings.Default.SecretData;
			}
			if (Settings.Default.SecretSave.Length > 0)
			{
				this.saveToolStripMenuItem3.Text += " - " + Settings.Default.SecretSave;
			}
			if ((Game)Settings.Default.Game == Game.PoolOfRadiance)
			{
				poolOfRadianceToolStripMenuItem.Checked = true;
				curseOfTheAzureBondsToolStripMenuItem.Checked = false;
				secretOfTheSilverBladesToolStripMenuItem.Checked = false;
			}
			else if ((Game)Settings.Default.Game == Game.CurseOfTheAzureBonds)
			{
				poolOfRadianceToolStripMenuItem.Checked = false;
				curseOfTheAzureBondsToolStripMenuItem.Checked = true;
				secretOfTheSilverBladesToolStripMenuItem.Checked = false;
			}
			else if ((Game)Settings.Default.Game == Game.SecretOfTheSilverBlades)
			{
				poolOfRadianceToolStripMenuItem.Checked = false;
				curseOfTheAzureBondsToolStripMenuItem.Checked = false;
				secretOfTheSilverBladesToolStripMenuItem.Checked = true;
			}
		}

		object obj = new object();

		public void UpdateDisplayCallback()
		{
			if (displayArea.InvokeRequired)
			{
				displayArea.Invoke(new MethodInvoker(UpdateDisplayCallback));
			}
			else
			{
				displayArea.Image = (Image)Classes.Display.bm.Clone();
			}
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

		private void setSettings()
		{
			Settings.Default.Upgrade();
			Settings.Default.Save();

			Classes.Cheats.PlayerAlwaysSavesSet(Settings.Default.PlayerAlwaysSaves);
			Classes.Cheats.AlwayShowAreaMapSet(Settings.Default.AlwayShowAreaMap);
			Classes.Cheats.FreeTrainingSet(Settings.Default.FreeTraining);
			Classes.Cheats.SkipCopyProtectionSet(Settings.Default.SkipCopyProtection);
			Classes.Cheats.AllowPlayerModifySet(Settings.Default.AllowPlayerModify);
			Classes.Cheats.AllowGodsInterveneSet(Settings.Default.AllowGodsIntervene);
			Classes.Cheats.DisplayFullItemNamesSet(Settings.Default.DisplayFullItemNames);
			Classes.Cheats.ViewItemStatsSet(Settings.Default.ViewItemsStats);
			Classes.Cheats.SkipTitleScreenSet(Settings.Default.SkipTitleScreen);
			Classes.Cheats.ImprovedAreaMapSet(Settings.Default.ImprovedAreaMap);
			Classes.Cheats.NoRaceLevelLimits(Settings.Default.NoRaceClassLimits);
			Classes.Cheats.NoRaceClassRestrictions(Settings.Default.NoRaceClassLimits);
			Classes.Cheats.SortTreasureSet(Settings.Default.SortTreasure);

			Config.SetDataPath(Logging.Game.PoolOfRadiance, Settings.Default.PoolradData);
			Config.SetSavePath(Logging.Game.PoolOfRadiance, Settings.Default.PoolradSave);
			Config.SetDataPath(Logging.Game.CurseOfTheAzureBonds, Settings.Default.CurseData);
			Config.SetSavePath(Logging.Game.CurseOfTheAzureBonds, Settings.Default.CurseSave);
			Config.SetDataPath(Logging.Game.SecretOfTheSilverBlades, Settings.Default.SecretData);
			Config.SetSavePath(Logging.Game.SecretOfTheSilverBlades, Settings.Default.SecretSave);
			Config.SetGame(Settings.Default.Game);

			engine.seg044.SetSound(Settings.Default.SoundOn);
			engine.seg044.SetPicture(Settings.Default.PictureOn);
			engine.seg044.SetAnimation(Settings.Default.AnimationOn);
		}

		private void playersAlwayMakeSavingThrowToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.PlayerAlwaysSaves;
			Settings.Default.PlayerAlwaysSaves = flipped;
			Settings.Default.Save();

			Classes.Cheats.PlayerAlwaysSavesSet(flipped);
		}

		private void alwayAllowAreaMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.Default.AlwayShowAreaMap = !Settings.Default.AlwayShowAreaMap;
			Settings.Default.Save();

			Classes.Cheats.AlwayShowAreaMapSet(Settings.Default.AlwayShowAreaMap);
		}

		private void freeTrainingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.FreeTraining;
			Settings.Default.FreeTraining = flipped;
			Settings.Default.Save();

			Classes.Cheats.FreeTrainingSet(flipped);
		}

		private void skipCopyProtectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.SkipCopyProtection;
			Settings.Default.SkipCopyProtection = flipped;
			Settings.Default.Save();

			Classes.Cheats.SkipCopyProtectionSet(flipped);
		}

		private void skipTitleScreenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.SkipTitleScreen;
			Settings.Default.SkipTitleScreen = flipped;
			Settings.Default.Save();

			Classes.Cheats.SkipTitleScreenSet(flipped);
		}

		private void allowPlayerModifyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.AllowPlayerModify;
			Settings.Default.AllowPlayerModify = flipped;
			Settings.Default.Save();

			Classes.Cheats.AllowPlayerModifySet(flipped);
		}

		private void allowGodsInterveneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.AllowGodsIntervene;
			Settings.Default.AllowGodsIntervene = flipped;
			Settings.Default.Save();

			Classes.Cheats.AllowGodsInterveneSet(flipped);
		}

		private void displayItemsFullNameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.DisplayFullItemNames;
			Settings.Default.DisplayFullItemNames = flipped;
			Settings.Default.Save();

			Classes.Cheats.DisplayFullItemNamesSet(flipped);
		}

		private void viewItemsStatsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.ViewItemsStats;
			Settings.Default.ViewItemsStats = flipped;
			Settings.Default.Save();

			Classes.Cheats.ViewItemStatsSet(flipped);
		}

		private void improvedAreaMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.ImprovedAreaMap;
			Settings.Default.ImprovedAreaMap = flipped;
			Settings.Default.Save();

			Classes.Cheats.ImprovedAreaMapSet(flipped);
		}

		private void noRaceLevelLimitsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.NoRaceLevelLimits;
			Settings.Default.NoRaceLevelLimits = flipped;
			Settings.Default.Save();

			Classes.Cheats.NoRaceLevelLimits(flipped);
		}

		private void noRaceClassLimitsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.NoRaceClassLimits;
			Settings.Default.NoRaceClassLimits = flipped;
			Settings.Default.Save();

			Classes.Cheats.NoRaceClassRestrictions(flipped);
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
			bool flipped = !Settings.Default.SoundOn;
			Settings.Default.SoundOn = flipped;
			Settings.Default.Save();

			engine.seg044.SetSound(flipped);
		}

        private void PictureOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flipped = !Settings.Default.PictureOn;
            Settings.Default.PictureOn = flipped;
            if (flipped == false)
            {
                Settings.Default.AnimationOn = false;
                engine.seg044.SetAnimation(false);
            }
            Settings.Default.Save();

            engine.seg044.SetPicture(flipped);

        }

        private void AnimationOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flipped = !Settings.Default.AnimationOn;
            Settings.Default.AnimationOn = flipped;
            Settings.Default.Save();

            engine.seg044.SetAnimation(flipped);
        }

		private void sortTreasureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool flipped = !Settings.Default.SortTreasure;
			Settings.Default.SortTreasure = flipped;
			Settings.Default.Save();

			Classes.Cheats.SortTreasureSet(flipped);
		}

		private void DataToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = Settings.Default.PoolradData;
			var result = folderBrowserDialog1.ShowDialog();
			if (!result.Equals(DialogResult.OK)) return;
			Settings.Default.PoolradData = folderBrowserDialog1.SelectedPath;
			Settings.Default.Save();
			dataToolStripMenuItem1.Text = "Data - " + Settings.Default.PoolradData;

			Logging.Config.SetSavePath(Logging.Game.PoolOfRadiance, Settings.Default.PoolradData);
		}

		private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = Settings.Default.PoolradSave;
			var result = folderBrowserDialog1.ShowDialog();
			if (!result.Equals(DialogResult.OK)) return;
			Settings.Default.PoolradSave = folderBrowserDialog1.SelectedPath;
			Settings.Default.Save();
			saveToolStripMenuItem1.Text = "Save - " + Settings.Default.PoolradSave;

			Logging.Config.SetSavePath(Logging.Game.PoolOfRadiance, Settings.Default.PoolradSave);
		}

		private void DataToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = Settings.Default.CurseData;
			var result = folderBrowserDialog1.ShowDialog();
			if (!result.Equals(DialogResult.OK)) return;
			Settings.Default.CurseData = folderBrowserDialog1.SelectedPath;
			Settings.Default.Save();
			dataToolStripMenuItem2.Text = "Data - " + Settings.Default.CurseData;

			Logging.Config.SetSavePath(Logging.Game.CurseOfTheAzureBonds, Settings.Default.CurseData);
		}

		private void SaveToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = Settings.Default.CurseSave;
			var result = folderBrowserDialog1.ShowDialog();
			if (!result.Equals(DialogResult.OK)) return;
			Settings.Default.CurseSave = folderBrowserDialog1.SelectedPath;
			Settings.Default.Save();
			saveToolStripMenuItem2.Text = "Save - " + Settings.Default.CurseSave;

			Logging.Config.SetSavePath(Logging.Game.CurseOfTheAzureBonds, Settings.Default.CurseSave);
		}

		private void DataToolStripMenuItem3_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = Settings.Default.SecretData;
			var result = folderBrowserDialog1.ShowDialog();
			if (!result.Equals(DialogResult.OK)) return;
			Settings.Default.SecretData = folderBrowserDialog1.SelectedPath;
			Settings.Default.Save();
			dataToolStripMenuItem3.Text = "Data - " + Settings.Default.SecretData;

			Logging.Config.SetSavePath(Logging.Game.SecretOfTheSilverBlades, Settings.Default.SecretData);
		}

		private void SaveToolStripMenuItem3_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = Settings.Default.SecretSave;
			var result = folderBrowserDialog1.ShowDialog();
			if (!result.Equals(DialogResult.OK)) return;
			Settings.Default.SecretSave = folderBrowserDialog1.SelectedPath;
			Settings.Default.Save();
			saveToolStripMenuItem3.Text = "Save - " + Settings.Default.SecretSave;

			Logging.Config.SetSavePath(Logging.Game.SecretOfTheSilverBlades, Settings.Default.SecretSave);
		}

		private void poolOfRadianceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (poolOfRadianceToolStripMenuItem.Checked == false)
			{
				Settings.Default.Game = (int)Game.PoolOfRadiance;
				Settings.Default.Save();
				Config.SetGame(Settings.Default.Game);
				poolOfRadianceToolStripMenuItem.Checked = true;
				curseOfTheAzureBondsToolStripMenuItem.Checked = false;
				secretOfTheSilverBladesToolStripMenuItem.Checked = false;
			}
		}

		private void curseOfTheAzureBondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (curseOfTheAzureBondsToolStripMenuItem.Checked == false)
			{
				Settings.Default.Game = (int)Game.CurseOfTheAzureBonds;
				Settings.Default.Save();
				Config.SetGame(Settings.Default.Game);
				poolOfRadianceToolStripMenuItem.Checked = false;
				curseOfTheAzureBondsToolStripMenuItem.Checked = true;
				secretOfTheSilverBladesToolStripMenuItem.Checked = false;
			}
		}

		private void secretOfTheSilverBladesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (secretOfTheSilverBladesToolStripMenuItem.Checked == false)
			{
				Settings.Default.Game = (int)Game.SecretOfTheSilverBlades;
				Settings.Default.Save();
				Config.SetGame(Settings.Default.Game);
				poolOfRadianceToolStripMenuItem.Checked = false;
				curseOfTheAzureBondsToolStripMenuItem.Checked = false;
				secretOfTheSilverBladesToolStripMenuItem.Checked = true;
			}
		}
    }
}