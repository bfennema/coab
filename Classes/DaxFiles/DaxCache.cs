using System.Collections.Generic;
using System.IO;

namespace Classes.DaxFiles
{
    public class DaxCache
    {
        static Dictionary<string, DaxFileCache> fileCache = new Dictionary<string, DaxFileCache>();

        public static byte[] LoadDax(string file_name, int block_id)
        {
            DaxFileCache dfc;

            file_name = file_name.ToLower();

            if (!fileCache.TryGetValue(file_name, out dfc))
            {
                try
                {
                    dfc = new DaxFileCache(file_name);
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                fileCache.Add(file_name, dfc);
            }

            return dfc.GetData(block_id);
        }
    }
}
