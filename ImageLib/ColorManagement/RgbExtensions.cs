namespace ImageLib.ColorManagement
{
    public static class RgbExtensions
    {
        public static XyzColor ToXyz(this Rgb rgb) => ColorSpace.Srgb.ToXyz(rgb);
    }
}
