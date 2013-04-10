using System.Windows.Media;

namespace ImageLib.Gamut
{
    public interface IGamut
    {
        /// <summary>
        /// Convert from sRGB into this gamut.
        /// </summary>
        /// <remarks>
        /// Conversion may include clamping, scaling and other color transformations.
        /// </remarks>
        /// <param name="color">sRGB color to convert</param>
        /// <returns>Color inside this gamut.</returns>
        Color FromSrgb(Color c);
    }
}
