using System;
using System.Collections.Generic;
using FilConvWpf.UI;
using ImageLib;

namespace FilConvWpf
{
    interface IImagePresenter
    {
        event EventHandler<EventArgs> DisplayImageChanged;
        event EventHandler<EventArgs> ToolBarChanged;

        AspectBitmap DisplayImage { get; }

        /// <summary>
        /// Tools for manipulating the presenter.
        /// </summary>
        IEnumerable<ITool> Tools { get; }

        void StoreSettings(IDictionary<string, object> settings);
        void AdoptSettings(IDictionary<string, object> settings);
    }
}
