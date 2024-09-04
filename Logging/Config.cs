using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Logging
{
    public enum Game
    {
        None = 0,
        PoolOfRadiance,
        CurseOfTheAzureBonds,
        SecretOfTheSilverBlades,
        MaxGames,
    }

    public static class Config
    {
        static string basePath;
        static string appDataPath;
        static string logPath;
        static Settings settings;

        public class Settings
        {
            private string configFile;
            public string[] savePath = new string[(int)Game.MaxGames];
            public string[] dataPath = new string[(int)Game.MaxGames];
            public Game game = Game.None;


            public Settings()
            {
                configFile = "";
            }

            public Settings(string appDataPath, string defaultSavePath)
            {
                configFile = configFile = Path.Combine(appDataPath, "Settings.xml");
                for (Game i = Game.None; i < Game.MaxGames; i++)
                {
                    savePath[(int)i] = Path.Combine(defaultSavePath, Enum.GetName<Game>(i));
                    dataPath[(int)i] = "";
                }
                if (File.Exists(configFile))
                {
                    FileStream fs = new FileStream(configFile, FileMode.Open);

                    if (fs.Length > 0)
                    {
                        Settings? saved = null;
                        XmlSerializer formatter = new XmlSerializer(this.GetType());
                        try
                        {
                            saved = (Settings?)formatter.Deserialize(fs);
                        }
                        catch (SerializationException e)
                        {
                            throw;
                        }
                        finally
                        {
                            fs.Close();
                        }

                        if (saved != null)
                        {
                            for (int i = (int)Game.None; i < (int)Game.MaxGames; i++)
                            {
                                savePath[i] = saved.savePath[i];
                                dataPath[i] = saved.dataPath[i];
                            }
                            game = saved.game;
                        }
                    }
                }
            }

            public void Save()
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
        }

        static Config()
        {
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Gold Box Player");

            if (Directory.Exists(basePath) == false)
            {
                Directory.CreateDirectory(basePath);
            }

            logPath = Path.Combine(basePath, "Logs");
            if (Directory.Exists(logPath) == false)
            {
                Directory.CreateDirectory(logPath);
            }

            Logger.Setup(logPath);

            string saveBasePath = Path.Combine(basePath, "Save");

            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Gold Box Player");

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }

            settings = new Settings(appDataPath, saveBasePath);
        }

        public static string GetLogPath() { return logPath; }
        public static string GetAppDataPath() { return appDataPath; }
        public static void SetSavePath(Game which, string path) { settings.savePath[(int)which] = path; settings.Save(); }
        public static void SetDataPath(Game which, string path) { settings.dataPath[(int)which] = path; settings.Save(); }
        public static string GetSavePath(Game which) { return settings.savePath[(int)which]; }
        public static string GetDataPath(Game which) { return settings.dataPath[(int)which]; }
        public static string GetBasePath() { return basePath; }
        public static void SetGame(int which) { settings.game = (Game)which; settings.Save(); }
        public static void SetGame(Game which) { settings.game = which; settings.Save(); }
        public static Game GetGame() { return settings.game; }


    }
}
