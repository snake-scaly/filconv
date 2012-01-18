using System.Windows.Media.Imaging;

namespace ImageLib
{
    /// <summary>
    /// Image converter interface.
    /// </summary>
    /// <para>This interface provides routines to convert between
    /// standard raster image representation and bytes in a particular
    /// native format.</para>
    public interface NativeImageFormat
    {
        /// <summary>
        /// Format name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Native aspect ratio of the image pixels.
        /// </summary>
        double Aspect { get; }

        BitmapSource FromNative(NativeImage native);
        NativeImage ToNative(BitmapSource bitmap, EncodingOptions options);
    }
}
