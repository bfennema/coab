using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Classes
{
    public static class MapTracker
    {
        public static Dictionary<string, bool[][]> ExploredMaps { get; set; } = new Dictionary<string, bool[][]>();

        public static void MarkExplored(int gameArea, int blockId, int x, int y)
        {
            string key = $"{gameArea}_{blockId}";
            if (!ExploredMaps.TryGetValue(key, out var grid))
            {
                grid = new bool[16][];
                for (int i = 0; i < 16; i++)
                {
                    grid[i] = new bool[16];
                }
                ExploredMaps[key] = grid;
            }

            if (x >= 0 && x < 16 && y >= 0 && y < 16)
            {
                grid[y][x] = true;
            }
        }

        public static bool IsExplored(int gameArea, int blockId, int x, int y)
        {
            string key = $"{gameArea}_{blockId}";
            if (ExploredMaps.TryGetValue(key, out var grid))
            {
                if (x >= 0 && x < 16 && y >= 0 && y < 16)
                {
                    return grid[y][x];
                }
            }
            return false;
        }

        public static void Reset()
        {
            ExploredMaps.Clear();
        }

        public static async Task Save(string savePath, string datFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(savePath)) return;
                string mapFileName = Path.ChangeExtension(datFileName, ".MAP");
                using (var stream = await gbl.file.Create(savePath, mapFileName))
                {
                    if (stream != null)
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(ExploredMaps, options);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                        stream.Close();
                    }
                }
            }
            catch (Exception)
            {
                // Ignore errors to prevent crash
            }
        }

        public static async Task Load(string savePath, string datFileName)
        {
            try
            {
                Reset();
                if (string.IsNullOrEmpty(savePath)) return;
                string mapFileName = Path.ChangeExtension(datFileName, ".MAP");
                if (await gbl.file.Find(savePath, mapFileName))
                {
                    using (var stream = await gbl.file.Open(savePath, mapFileName))
                    {
                        if (stream != null)
                        {
                            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, bool[][]>>(stream);
                            if (data != null)
                            {
                                ExploredMaps = data;
                            }
                            stream.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignore errors to prevent crash
            }
        }
    }
}
