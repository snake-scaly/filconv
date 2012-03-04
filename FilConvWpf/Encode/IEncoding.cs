using System;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    interface IEncoding : IToolbarClient
    {
        event EventHandler<EventArgs> EncodingChanged;
        string Name { get; }
        DisplayImage Encode(BitmapSource original);

        /// <summary>
        /// Check whether this encoding can supply data for the given
        /// container, for instance, <see cref="FilLib.Fil"/>.
        /// </summary>
        /// <param name="type">CLR type of the container</param>
        /// <returns><code>true</code> if data can be supplied.</returns>
        bool IsContainerSupported(Type type);

        /// <summary>
        /// Supply encoded data for the given container.
        /// </summary>
        /// <remarks>
        /// The runtime type of the container must be supported
        /// by this encoding.  To check if the container is supported,
        /// pass container.GetType() to the <see cref="#IsContainerSupported()"/>
        /// method.
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// if runtime type of the given container is not supported
        /// </exception>
        void FillContainerData(object container, BitmapSource original);
    }
}
