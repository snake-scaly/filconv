using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using FilConv.Presenter;
using FilConv.UI;

namespace FilConv.ViewModels;

public class PresenterViewModel : IDisposable
{
    private readonly ObservableCollection<Control> _items = [];
    private readonly IDisposable _toolsSubscription;

    public IObservable<bool> AspectToggleVisible { get; }
    public ReadOnlyObservableCollection<Control> Tools { get; }
    public IObservable<AspectBitmapSource?> AspectBitmap { get; }

    public PresenterViewModel(IObservable<IImagePresenter?> presenters)
    {
        AspectBitmap = presenters
            .SelectMany(
                IObservable<EventPattern<object?, EventArgs>> (presenter) =>
                {
                    var newPresenter = Observable.Return(
                        new EventPattern<object?, EventArgs>(presenter, EventArgs.Empty));
                    if (presenter == null)
                        return newPresenter;
                    return newPresenter.Concat(
                        Observable.FromEventPattern<object?, EventArgs>(
                            presenter, nameof(IImagePresenter.DisplayImageChanged)));
                })
            .Select(x => (x.Sender as IImagePresenter)?.DisplayImage);

        AspectToggleVisible = AspectBitmap.Select(x => x is { Bitmap: not null } && x.PixelAspect != 1);

        _toolsSubscription = presenters
            .SelectMany(
                IObservable<EventPattern<object?, EventArgs>> (presenter) =>
                {
                    var newPresenter = Observable.Return(
                        new EventPattern<object?, EventArgs>(presenter, EventArgs.Empty));
                    if (presenter == null)
                        return newPresenter;
                    return newPresenter.Concat(
                        Observable.FromEventPattern<object?, EventArgs>(
                            presenter, nameof(IImagePresenter.ToolBarChanged)));
                })
            .Select(x => (x.Sender as IImagePresenter)?.Tools)
            .Subscribe(OnToolsReplaced);

        Tools = new ReadOnlyObservableCollection<Control>(_items);
    }

    public void Dispose()
    {
        _toolsSubscription.Dispose();
    }

    private void OnToolsReplaced(IEnumerable<ITool>? tools)
    {
        if (tools == null)
        {
            _items.Clear();
            return;
        }

        var i = 0;
        foreach (var tool in tools)
        {
            var j = IndexOf(tool.Element, i);

            if (j == -1)
                _items.Insert(i, tool.Element);
            else if (j != i)
                _items.Move(j, i);

            i++;
        }

        for (var j = _items.Count - 1; j >= i; j--)
            _items.RemoveAt(j);
    }

    private int IndexOf(object item, int startIndex)
    {
        for (var i = startIndex; i < _items.Count; i++)
            if (ReferenceEquals(_items[i], item))
                return i;
        return -1;
    }
}
