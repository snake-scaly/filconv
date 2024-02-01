using ImageLib.ColorManagement;

namespace ImageLib
{
    /// <summary>
    /// An interface for reading pixels of a bitmap.
    /// </summary>
    public interface IReadOnlyPixels
    {
        int Width { get; }
        int Height { get; }
        Rgb GetPixel(int x, int y);
    }
}
