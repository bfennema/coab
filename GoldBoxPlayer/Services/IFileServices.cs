using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace GoldBoxPlayer.Services;

public interface IFilesService
{
    public Task<IStorageFolder?> OpenFolderAsync(string title, IStorageFolder? startLocation);
}