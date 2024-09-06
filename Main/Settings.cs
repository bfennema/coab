using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main
{
    public class Settings
    {
        private string configFile = "";
        public bool allowPlayerModify = false;
        public bool allowGodsIntervene = false;
        public bool displayFullItemNames = false;
        public bool skipCopyProtection = true;
        public bool freeTraining = false;
        public bool alwaysShowAreaMap = false;
        public bool playerAlwaysSaves = false;
        public bool viewItemsStats = false;
        public bool skipTitleScreen = false;
        public bool improvedAreaMap = false;
        public bool noRaceClassLimits = false;
        public bool soundOn = true;
        public bool noRaceLevelLimits = false;
        public bool sortTreasure = false;
        public bool pictureOn = true;
        public bool animationOn = true;
        public string curseOfTheAzureBondsDataPath = "";
        public string curseOfTheAzureBondsSavePath = "";

        static public Settings LoadSettings(string appDataPath, string defaultDataPath, string defaultSavePath)
        {
            string configFile = Path.Combine(appDataPath, "Settings.xml");
            Settings settings = null;

            if (System.IO.File.Exists(configFile))
            {
                FileStream fs = new FileStream(configFile, FileMode.Open);

                if (fs.Length > 0)
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(Settings));
                    try
                    {
                        settings = (Settings)formatter.Deserialize(fs);
                    }
                    catch (SerializationException e)
                    {
                        throw;
                    }
                    finally
                    {
                        fs.Close();
                    }
                    settings.configFile = configFile;
                }
                else
                {
                    settings = new Settings(appDataPath, defaultDataPath, defaultSavePath);
                    settings.Save();
                }
            }
            else
            {
                settings = new Settings(appDataPath, defaultDataPath, defaultSavePath);
                settings.Save();
            }
            return settings;
        }

        public Settings()
        {
        }

        public Settings(string appDataPath, string defaultDataPath, string defaultSavePath)
        {
            configFile = Path.Combine(appDataPath, "Settings.xml");
            curseOfTheAzureBondsDataPath = defaultDataPath;
            curseOfTheAzureBondsSavePath = defaultSavePath;
        }

        private void Save()
        {
            FileStream fs = new FileStream(configFile, FileMode.Create);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            XmlSerializer formatter = new XmlSerializer(this.GetType());
            try
            {
                formatter.Serialize(fs, this);
            }
            catch (SerializationException e)
            {
                //Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
        private void UpdateGameDataSave()
        {
            Logging.Config.DataPath = curseOfTheAzureBondsDataPath;
            Logging.Config.SavePath = curseOfTheAzureBondsSavePath;
        }
        public void Set()
        {
            UpdateGameDataSave();

            engine.seg044.SetSound(soundOn);
            engine.seg044.SetPicture(pictureOn);
            engine.seg044.SetAnimation(animationOn);

            Classes.Cheats.PlayerAlwaysSavesSet(playerAlwaysSaves);
            Classes.Cheats.AlwaysShowAreaMapSet(alwaysShowAreaMap);
            Classes.Cheats.FreeTrainingSet(freeTraining);
            Classes.Cheats.SkipCopyProtectionSet(skipCopyProtection);
            Classes.Cheats.AllowPlayerModifySet(allowPlayerModify);
            Classes.Cheats.AllowGodsInterveneSet(allowGodsIntervene);
            Classes.Cheats.DisplayFullItemNamesSet(displayFullItemNames);
            Classes.Cheats.ViewItemStatsSet(viewItemsStats);
            Classes.Cheats.SkipTitleScreenSet(skipTitleScreen);
            Classes.Cheats.ImprovedAreaMapSet(improvedAreaMap);
            Classes.Cheats.NoRaceLevelLimits(noRaceClassLimits);
            Classes.Cheats.NoRaceClassRestrictions(noRaceClassLimits);
            Classes.Cheats.SortTreasureSet(sortTreasure);
        }
        [XmlIgnore]
        public bool AllowPlayerModify
        {
            get => allowPlayerModify;
            set
            {
                allowPlayerModify = value;
                Save();
                Classes.Cheats.AllowPlayerModifySet(value);
            }
        }
        [XmlIgnore]
        public bool AllowGodsIntervene
        {
            get => allowGodsIntervene;
            set
            {
                allowGodsIntervene = value;
                Save();
                Classes.Cheats.AllowGodsInterveneSet(value);
            }
        }
        [XmlIgnore]
        public bool DisplayFullItemNames
        {
            get => displayFullItemNames;
            set
            {
                displayFullItemNames = value;
                Save();
                Classes.Cheats.DisplayFullItemNamesSet(value);
            }
        }
        [XmlIgnore]
        public bool SkipCopyProtection
        {
            get => skipCopyProtection;
            set
            {
                skipCopyProtection = value;
                Save();
                Classes.Cheats.SkipCopyProtectionSet(value);
            }
        }
        [XmlIgnore]
        public bool FreeTraining
        {
            get => freeTraining;
            set
            {
                freeTraining = value;
                Save();
                Classes.Cheats.FreeTrainingSet(value);
            }
        }
        [XmlIgnore]
        public bool AlwaysShowAreaMap
        {
            get => alwaysShowAreaMap;
            set
            {
                alwaysShowAreaMap = value;
                Save();
                Classes.Cheats.AlwaysShowAreaMapSet(value);
            }
        }
        [XmlIgnore]
        public bool PlayerAlwaysSaves
        {
            get => playerAlwaysSaves;
            set
            {
                playerAlwaysSaves = value;
                Save();
                Classes.Cheats.PlayerAlwaysSavesSet(value);
            }
        }
        [XmlIgnore]
        public bool ViewItemsStats
        {
            get => viewItemsStats;
            set
            {
                viewItemsStats = value;
                Save();
                Classes.Cheats.ViewItemStatsSet(value);
            }
        }
        [XmlIgnore]
        public bool SkipTitleScreen
        {
            get => skipTitleScreen;
            set
            {
                skipTitleScreen = value;
                Save();
                Classes.Cheats.SkipTitleScreenSet(value);
            }
        }
        [XmlIgnore]
        public bool ImprovedAreaMap
        {
            get => improvedAreaMap;
            set
            {
                improvedAreaMap = value;
                Save();
                Classes.Cheats.ImprovedAreaMapSet(value);
            }
        }
        [XmlIgnore]
        public bool NoRaceClassLimits
        {
            get => noRaceClassLimits;
            set
            {
                noRaceClassLimits = value;
                Save();
                Classes.Cheats.NoRaceClassRestrictions(value);
            }
        }
        [XmlIgnore]
        public bool SoundOn
        {
            get => soundOn;
            set
            {
                soundOn = value;
                Save();
                engine.seg044.SetSound(value);
            }
        }
        [XmlIgnore]
        public bool NoRaceLevelLimits
        {
            get => noRaceLevelLimits;
            set
            {
                noRaceLevelLimits = value;
                Save();
                Classes.Cheats.NoRaceLevelLimits(value);
            }
        }
        [XmlIgnore]
        public bool SortTreasure
        {
            get => sortTreasure;
            set
            {
                sortTreasure = value;
                Save();
                Classes.Cheats.SortTreasureSet(value);
            }
        }
        [XmlIgnore]
        public bool PictureOn
        {
            get => pictureOn;
            set
            {
                pictureOn = value;
                Save();
                engine.seg044.SetPicture(value);
            }
        }
        [XmlIgnore]
        public bool AnimationOn
        {
            get => animationOn;
            set
            {
                animationOn = value;
                Save();
                engine.seg044.SetAnimation(value);
            }
        }
        [XmlIgnore]
        public string CurseOfTheAzureBondsDataPath
        {
            get => curseOfTheAzureBondsDataPath;
            set
            {
                curseOfTheAzureBondsDataPath = value;
                Save();
                Logging.Config.DataPath = curseOfTheAzureBondsDataPath;
            }
        }
        [XmlIgnore]
        public string CurseOfTheAzureBondsSavePath
        {
            get => curseOfTheAzureBondsSavePath;
            set
            {
                curseOfTheAzureBondsSavePath = value;
                Save();
                Logging.Config.SavePath = curseOfTheAzureBondsSavePath;
            }
        }
    }
}
