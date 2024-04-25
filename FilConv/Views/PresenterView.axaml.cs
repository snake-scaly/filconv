using Avalonia;
using Avalonia.Controls;
using FilConv.Presenter;
using FilConv.ViewModels;

namespace FilConv.Views;

public partial class PresenterView : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<PresenterView, string>(nameof(Title));

    public static readonly StyledProperty<IImagePresenter> ImagePresenterProperty =
        AvaloniaProperty.Register<PresenterView, IImagePresenter>(nameof(ImagePresenter));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public IImagePresenter ImagePresenter
    {
        get => GetValue(ImagePresenterProperty);
        set => SetValue(ImagePresenterProperty, value);
    }

    public PresenterView()
    {
        InitializeComponent();
        (Content as StyledElement)!.DataContext = new PresenterViewModel(this.GetObservable(ImagePresenterProperty));
    }
}
