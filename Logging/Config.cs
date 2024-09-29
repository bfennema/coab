using System;
using System.IO;

namespace Logging
{
    public enum Game
    {
        None = 0,
        PoolOfRadiance,
        CurseOfTheAzureBonds,
        SecretOfTheSilverBlades,
        PoolsOfDarkness,
        ChampionsOfKrynn,
        DeathKnightsOfKrynn,
        TheDarkQueenOfKrynn,
        GatewayToTheSavageFrontier,
        TreasuresOfTheSavageFrontier,
        UnlimitedAdventures,
        CountdownToDoomsday,
        MatrixCubed,
        MaxGames,
    }

    public static class Config
    {
        static string basePath;
        static string appDataPath;
        static string logPath;
        static string saveBasePath;
        static string savePath;
        static string[] savePathByGame = new string[(int)Game.MaxGames];
        static string dataPath;
        static Game game;

        static Config()
        {
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Curse of the Azure Bonds");

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

            saveBasePath = Path.Combine(basePath, "Save");

            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CotAB");

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }

            savePath = "";
            dataPath = "";
            game = Game.None;
        }
        public static string BasePath
        {
            get => basePath;
        }
        public static string LogPath
        {
            get => logPath;
        }
        public static string AppDataPath
        {
            get => appDataPath;
        }
        public static string SaveBasePath
        {
            get => saveBasePath;
        }
        public static string SavePath
        {
            get => savePath;
            set => savePath = value;
        }
        public static string[] SavePathByGame
        {
            get => savePathByGame;
        }
        public static string DataPath
        {
            get => dataPath;
            set => dataPath = value;
        }
        public static Game Game
        {
            get => game;
            set => game = value;
        }
    }
}
