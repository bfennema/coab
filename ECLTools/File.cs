using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public partial class File : Classes.File
{
    internal async static Task<FileStream?> InternalOpen(string filename, FileMode mode, FileAccess access)
    {
        return await Task.Run(() => new FileStream(filename, mode, access, FileShare.ReadWrite, bufferSize: 4096, useAsync: true));
    }
    public async IAsyncEnumerable<(string, Stream)> OpenAll(string path, string filter = "*")
    {
        foreach (var filePath in Directory.EnumerateFiles(path, filter))
        {
            var fs = await InternalOpen(filePath, FileMode.Open, FileAccess.Read);

            if (fs != null)
            {
                yield return (filePath, fs);
            }
        }
    }
    public async Task<Stream?> Open(string path, string filename)
    {
        if (!System.IO.File.Exists(path + "\\" + filename))
        {
            return null;
        }
        return await InternalOpen(path + "\\" + filename, FileMode.Open, FileAccess.Read);
    }
    public async Task<Stream?> Create(string path, string filename)
    {
        return await InternalOpen(path + "\\" + filename, FileMode.Create, FileAccess.Write);
    }
    public async Task<bool> Delete(string path, string filename)
    {
        await Task.Run(() => System.IO.File.Delete(path + "\\" + filename));

        return true;
    }
    public async Task<bool> Find(string path, string filename)
    {
        var fs = await InternalOpen(path + "\\" + filename, FileMode.Create, FileAccess.Write);

        if (fs == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
