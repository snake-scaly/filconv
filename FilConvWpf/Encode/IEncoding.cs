using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FilConvWpf.Encode
{
    interface IEncoding
    {
        event EventHandler<EventArgs> EncodingChanged;

        string Name { get; }
        ToolBar ToolBar { get; }

        AspectBitmap Preview(BitmapSource original);

        /// <summary>
        /// Check whether this encoding can supply data for the given
        /// container, for instance, <see cref="FilLib.Fil"/>.
        /// </summary>
        /// <param name="type">CLR type of the container</param>
        /// <returns><code>true</code> if data can be supplied.</returns>
        bool IsContainerSupported(Type type);

        /// <summary>
        /// Encode a bitmap into the given container.
        /// </summary>
        /// <remarks>
        /// The runtime type of the container must be supported
        /// by this encoding.  To check if the container is supported,
        /// pass <code>container.GetType()</code> to the
        /// <see cref="#IsContainerSupported()"/> method.
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// if runtime type of the given container is not supported
        /// </exception>
        void Encode(BitmapSource original, object container);

        void StoreSettings(IDictionary<string, object> settings);
        void AdoptSettings(IDictionary<string, object> settings);
    }
}
