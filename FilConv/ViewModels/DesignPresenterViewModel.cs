using System.Reactive.Linq;
using FilConv.Presenter;

namespace FilConv.ViewModels;

public class DesignPresenterViewModel : PresenterViewModel
{
    public DesignPresenterViewModel()
        : base(Observable.Empty<IImagePresenter>())
    {
    }
}
