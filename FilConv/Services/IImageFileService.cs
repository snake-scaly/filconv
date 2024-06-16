using System.Threading.Tasks;
using FilConv.Presenter;

namespace FilConv.Services;

public interface IImageFileService
{
    Task<IImagePresenter> LoadAsync(string fileName);
    Task<IImagePresenter> LoadRawAsync(string fileName);
}
