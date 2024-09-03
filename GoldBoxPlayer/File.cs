using Avalonia.Platform.Storage;
using Classes;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GoldBoxPlayer;

public partial class File : Classes.File
{
    internal async static Task<IStorageFile?> InternalOpen(string path, string filename)
    {
        IStorageFile? storageFile = null;
        var folder = await gbl.StorageProvider.OpenFolderBookmarkAsync(path);
        await foreach (var file in folder.GetItemsAsync())
        {
            if (string.Equals(file.Name, filename, System.StringComparison.OrdinalIgnoreCase))
            {
                storageFile = file as IStorageFile;
                if (storageFile != null)
                {
                    break;
                }
            }
        }
        return storageFile;
    }
    public async IAsyncEnumerable<(string,Stream)> OpenAll(string path, string filter = "*")
    {
        IStorageFile? storageFile = null;
        var folder = await gbl.StorageProvider.OpenFolderBookmarkAsync(path);
        await foreach (var file in folder.GetItemsAsync())
        {
            bool match = false;
            if (filter.Equals("*"))
            {
                match = true;
            }
            else if (filter.StartsWith("*"))
            {
                match = file.Name.EndsWith(filter.Substring(1), System.StringComparison.OrdinalIgnoreCase);
            }
            else if (filter.EndsWith("*"))
            {
                match = file.Name.StartsWith(filter.TrimEnd('*'), System.StringComparison.OrdinalIgnoreCase);
            }
            if (match)
            {
                storageFile = file as IStorageFile;
                if (storageFile != null)
                {
                    yield return (file.Name, await storageFile.OpenReadAsync());
                }
            }
        }
    }
    public async Task<Stream?> Open(string path, string filename)
    {
        IStorageFile? storageFile = await InternalOpen(path, filename);

        if (storageFile == null)
        {
            return null;
        }

        var stream = await storageFile.OpenReadAsync();

        return stream;
    }
    public async void Delete(string path, string filename)
    {
        IStorageFile? storageFile = await InternalOpen(path, filename);

        if (storageFile == null)
        {
            return;
        }
        await storageFile.DeleteAsync();
    }
    public async Task<bool> Find(string path, string filename)
    {
        IStorageFile? storageFile = await InternalOpen(path, filename);

        if (storageFile == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
