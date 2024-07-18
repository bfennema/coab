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
        MaxGames,
    }

    public static class Config
    {
        static string basePath;
        static string logPath;
        static string[] savePath = new string[(int)Game.MaxGames];
        static string[] dataPath = new string[(int)Game.MaxGames];
        static Game game;

        static public void Setup()
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
        }

        public static string GetLogPath() { return logPath; }
        public static void SetSavePath(Game which, string path) { savePath[(int)which] = path; }
        public static void SetDataPath(Game which, string path) { dataPath[(int)which] = path; }
        public static string GetSavePath(Game which) { return savePath[(int)which]; }
        public static string GetDataPath(Game which) { return dataPath[(int)which]; }
        public static string GetBasePath() { return basePath; }
        public static void SetGame(int which) { game = (Game)which; }
        public static void SetGame(Game which) { game = which; }
        public static Game GetGame() { return game; }
    }
}
