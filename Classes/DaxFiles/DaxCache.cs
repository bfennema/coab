using System.Collections.Generic;
using System.IO;

namespace Classes.DaxFiles
{
    public class DaxCache
    {
        private readonly static Dictionary<string, DaxFileCache> fileCache = [];

        public static void PreloadDax(string filename, byte filenum)
        {
            if (!fileCache.TryGetValue(filename, out DaxFileCache? dfc))
            {
                try
                {
                    dfc = new DaxFileCache(filename, filenum);
                }
                catch (FileNotFoundException)
                {
                    return;
                }
                catch (DirectoryNotFoundException)
                {
                    return;
                }
                fileCache.Add(filename, dfc);
            }
            else
            {
                try
                {
                    dfc.Add(filename, filenum);
                }
                catch (FileNotFoundException)
                {
                    return;
                }
                catch (DirectoryNotFoundException)
                {
                    return;
                }
            }
        }
        public static byte[]? LoadDax(string filename, string filenum, int block_id)
        {
            if (!fileCache.TryGetValue(filename, out DaxFileCache? dfc))
            {
                try
                {
                    dfc = new DaxFileCache(filename, filenum);
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (DirectoryNotFoundException)
                {
                    return null;
                }
                fileCache.Add(filename, dfc);
            }

            var entry = dfc.GetData(block_id);
            if (entry == null)
            {
                try
                {
                    if (dfc.Add(filename, filenum) == false)
                    {
                        return null;
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (DirectoryNotFoundException)
                {
                    return null;
                }
                entry = dfc.GetData(block_id);
            }

            return entry;
        }
        public static byte[]? LoadDax(string filename, byte filenum, int block_id)
        {
            return LoadDax(filename, filenum.ToString(), block_id);
        }
        public static byte[]? LoadDax(string filename, int block_id)
        {
            return LoadDax(filename, "", block_id);
        }
    }
}
