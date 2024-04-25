using System.Reactive.Linq;

namespace FilConv.ViewModels;

public class DesignPreviewViewModel : PreviewViewModel
{
    public DesignPreviewViewModel()
        : base(Observable.Empty<AspectBitmapSource>(), Observable.Empty<double?>(), Observable.Empty<bool>())
    {
    }
}
