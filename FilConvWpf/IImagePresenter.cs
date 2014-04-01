using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FilConvWpf
{
    interface IImagePresenter
    {
        event EventHandler<EventArgs> DisplayImageChanged;

        /// <summary>
        /// Names of supported preview modes.
        /// </summary>
        /// If PreviewModes is empty or null, the mode combo-box is not displayed.
        string[] SupportedPreviewModes { get; }

        /// <summary>
        /// Get or set the current preview mode.
        /// </summary>
        /// PreviewMode is an index into the SupportedPreviewModes array.
        /// Changing PreviewMode will usually fire the DisplayImageChanged event.
        int PreviewMode { get; set; }

        AspectBitmap DisplayImage { get; }

        /// <summary>
        /// Toolbar with additional controls for a preview mode.
        /// </summary>
        /// Toolbar can change when the DisplayImage changes. ToolBar can be null
        /// in which case it is not displayed.
        ToolBar ToolBar { get; }

        void StoreSettings(IDictionary<string, object> settings);
        void AdoptSettings(IDictionary<string, object> settings);
    }
}
