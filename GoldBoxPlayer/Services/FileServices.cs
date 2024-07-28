using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace GoldBoxPlayer.Services;

public class FilesService : IFilesService
{
    private readonly TopLevel _target;

    public FilesService(Window target)
    {
        _target = target;
    }

    public FilesService(Control control)
    {
        _target = TopLevel.GetTopLevel(control);
    }

    public async Task<IStorageFolder?> OpenFolderAsync(string title, IStorageFolder? startLocation)
    {
        var folder = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = title,
            SuggestedStartLocation = startLocation,
            AllowMultiple = false
        });

        return folder.Count >= 1 ? folder[0] : null;
    }
}