using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FilConv.ViewModels;

public partial class PreviewViewModel : ObservableObject
{
    private static readonly Cursor _handCursor = new(StandardCursorType.Hand);

    [NotifyPropertyChangedFor(nameof(ContentCursor))]
    [ObservableProperty] private Size _scrollerViewport;

    [NotifyPropertyChangedFor(nameof(ContentCursor))]
    [ObservableProperty] private Size _contentExtent;

    public IObservable<Bitmap?> Bitmap { get; }
    public IObservable<double?> Scale { get; }
    public IObservable<double> Aspect { get; }

    public PreviewViewModel(
        IObservable<AspectBitmapSource?> bitmapObservable,
        IObservable<double?> scaleObservable,
        IObservable<bool> nativeAspectObservable)
    {
        Bitmap = bitmapObservable.Select(x => x?.Bitmap);
        Scale = scaleObservable;
        Aspect = bitmapObservable.Select(x => x?.PixelAspect)
            .CombineLatest(nativeAspectObservable)
            .Select(ab => ab.Second ? ab.First ?? 1 : 1);
    }

    public Cursor ContentCursor
    {
        get
        {
            bool canScroll = ContentExtent.Width > ScrollerViewport.Width ||
                ContentExtent.Height > ScrollerViewport.Height;
            return canScroll ? _handCursor : Cursor.Default;
        }
    }
}
