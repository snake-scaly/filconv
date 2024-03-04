namespace ImageLib.ColorManagement
{
    public static class RgbExtensions
    {
        public static XyzColor ToXyz(this Rgb rgb) => ColorSpace.Srgb.ToXyz(rgb);

        public static LuvColor ToLuv(this Rgb rgb) => ColorSpace.Luv.FromXyz(rgb.ToXyz());

        public static LabColor ToLab(this Rgb rgb) => ColorSpace.Lab.FromXyz(rgb.ToXyz());
    }
}
