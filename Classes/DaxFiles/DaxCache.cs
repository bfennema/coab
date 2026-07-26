using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Classes.DaxFiles
{
    public class DaxCache
    {
        private readonly static Dictionary<string, DaxFileCache> fileCache = [];

        public static void ClearCache()
        {
            fileCache.Clear();
        }

        public static async Task<bool> PreloadDax(string filename, byte filenum)
        {
            if (!fileCache.TryGetValue(filename, out DaxFileCache? dfc))
            {
                dfc = new DaxFileCache();
                try
                {
                    var status = await dfc.Add(filename, filenum);
                    if (status == true)
                    {
                        fileCache.Add(filename, dfc);
                    }
                    return status;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    var status = await dfc.Add(filename, filenum);
                    if (status == true)
                    {
                        fileCache.Add(filename, dfc);
                    }
                    return status;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
            }
        }
        public static async Task<byte[]?> LoadDax(string filename, string filenum, int block_id)
        {
            if (!fileCache.TryGetValue(filename, out DaxFileCache? dfc))
            {
                dfc = new DaxFileCache();
                try
                {
                    var status = await dfc.Add(filename, filenum);
                    if (status == true)
                    {
                        fileCache.Add(filename, dfc);
                    }
                    else
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
            }

            var entry = dfc.GetData(block_id);
            if (entry == null)
            {
                try
                {
                    if (await dfc.Add(filename, filenum) == false)
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
        public static async Task<byte[]?> LoadDax(string filename, byte filenum, int block_id)
        {
            return await LoadDax(filename, filenum.ToString(), block_id);
        }
        public static async Task<byte[]?> LoadDax(string filename, int block_id)
        {
            return await LoadDax(filename, "", block_id);
        }
    }
}
