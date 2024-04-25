using System;
using System.Collections.Generic;
using FilConv.UI;

namespace FilConv.Presenter;

public interface IImagePresenter : IDisposable
{
    event EventHandler<EventArgs> DisplayImageChanged;
    event EventHandler<EventArgs> ToolBarChanged;

    AspectBitmapSource? DisplayImage { get; }

    /// <summary>
    /// Tools for manipulating the presenter.
    /// </summary>
    IEnumerable<ITool> Tools { get; }

    void StoreSettings(IDictionary<string, object> settings);
    void AdoptSettings(IDictionary<string, object> settings);
}
