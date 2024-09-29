using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GoldBoxPlayer
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
        public bool viewItemStats = false;
        public bool skipTitleScreen = false;
        public bool improvedAreaMap = false;
        public bool noRaceClassLimits = false;
        public bool soundOn = true;
        public bool noRaceLevelLimits = false;
        public bool sortTreasure = false;
        public bool pictureOn = true;
        public bool animationOn = true;
        public Logging.Game game = Logging.Game.None;
        public string poolOfRadianceDataPath = "";
        public string poolOfRadianceSavePath = "";
        public string curseOfTheAzureBondsDataPath = "";
        public string curseOfTheAzureBondsSavePath = "";
        public string secretOfTheSilverBladesDataPath = "";
        public string secretOfTheSilverBladesSavePath = "";

        static public Settings? LoadSettings(string appDataPath, string defaultDataPath, string defaultSavePath)
        {
            string configFile = Path.Combine(appDataPath, "Settings.xml");
            Settings? settings = null;

            if (System.IO.File.Exists(configFile))
            {
                FileStream fs = new FileStream(configFile, FileMode.Open);

                if (fs.Length > 0)
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(Settings));
                    try
                    {
                        settings = (Settings?)formatter.Deserialize(fs);
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
            //poolOfRadianceDataPath = Path.Combine(defaultDataPath, "POOLRAD");
            poolOfRadianceSavePath = Path.Combine(defaultSavePath, Enum.GetName<Logging.Game>(Logging.Game.PoolOfRadiance));
            //curseOfTheAzureBondsDataPath = Path.Combine(defaultDataPath, "CURSE");
            curseOfTheAzureBondsSavePath = Path.Combine(defaultSavePath, Enum.GetName<Logging.Game>(Logging.Game.CurseOfTheAzureBonds));
            //secretOfTheSilverBladesDataPath = Path.Combine(defaultDataPath, "SECRET");
            secretOfTheSilverBladesSavePath = Path.Combine(defaultSavePath, Enum.GetName<Logging.Game>(Logging.Game.SecretOfTheSilverBlades));
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
            Logging.Config.Game = game;
            Logging.Config.SavePathByGame[(int)Logging.Game.PoolOfRadiance] = poolOfRadianceSavePath;
            Logging.Config.SavePathByGame[(int)Logging.Game.CurseOfTheAzureBonds] = curseOfTheAzureBondsSavePath;
            Logging.Config.SavePathByGame[(int)Logging.Game.SecretOfTheSilverBlades] = secretOfTheSilverBladesDataPath;
            if (game == Logging.Game.PoolOfRadiance)
            {
                Logging.Config.DataPath = poolOfRadianceDataPath;
                Logging.Config.SavePath = poolOfRadianceSavePath;
            }
            else if (game == Logging.Game.CurseOfTheAzureBonds)
            {
                Logging.Config.DataPath = curseOfTheAzureBondsDataPath;
                Logging.Config.SavePath = curseOfTheAzureBondsSavePath;
            }
            else if (game == Logging.Game.SecretOfTheSilverBlades)
            {
                Logging.Config.DataPath = secretOfTheSilverBladesDataPath;
                Logging.Config.SavePath = secretOfTheSilverBladesSavePath;
            }
            else
            {
                Logging.Config.DataPath = "";
                Logging.Config.SavePath = "";
            }
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
            Classes.Cheats.ViewItemStatsSet(viewItemStats);
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
        public bool ViewItemStats
        {
            get => viewItemStats;
            set
            {
                viewItemStats = value;
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
        public Logging.Game Game
        {
            get => game;
            set
            {
                game = value;
                Save();
                UpdateGameDataSave();
            }
        }
        [XmlIgnore]
        public string PoolOfRadianceDataPath
        {
            get => poolOfRadianceDataPath;
            set
            {
                poolOfRadianceDataPath = value;
                Save();
                if (game == Logging.Game.PoolOfRadiance)
                {
                    Logging.Config.DataPath = poolOfRadianceDataPath;
                }
            }
        }
        [XmlIgnore]
        public string PoolOfRadianceSavePath
        {
            get => poolOfRadianceSavePath;
            set
            {
                poolOfRadianceSavePath = value;
                Save();
                if (game == Logging.Game.PoolOfRadiance)
                {
                    Logging.Config.SavePath = poolOfRadianceSavePath;
                }
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
                if (game == Logging.Game.CurseOfTheAzureBonds)
                {
                    Logging.Config.DataPath = curseOfTheAzureBondsDataPath;
                }
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
                if (game == Logging.Game.CurseOfTheAzureBonds)
                {
                    Logging.Config.SavePath = curseOfTheAzureBondsSavePath;
                }
            }
        }
        [XmlIgnore]
        public string SecretOfTheSilverBladesDataPath
        {
            get => secretOfTheSilverBladesDataPath;
            set
            {
                secretOfTheSilverBladesDataPath = value;
                Save();
                if (game == Logging.Game.SecretOfTheSilverBlades)
                {
                    Logging.Config.DataPath = secretOfTheSilverBladesDataPath;
                }
            }
        }
        [XmlIgnore]
        public string SecretOfTheSilverBladesSavePath
        {
            get => secretOfTheSilverBladesSavePath;
            set
            {
                secretOfTheSilverBladesSavePath = value;
                Save();
                if (game == Logging.Game.SecretOfTheSilverBlades)
                {
                    Logging.Config.SavePath = secretOfTheSilverBladesSavePath;
                }
            }
        }
    }
}
