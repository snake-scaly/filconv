namespace ImageLib.ColorManagement
{
    public class ColorSpace
    {
        public static SrgbColorSpace Srgb { get; } = new SrgbColorSpace();

        public static LuvColorSpace Luv { get; } = new LuvColorSpace(Srgb.ToXyz(Rgb.FromRgb(255, 255, 255)));

        public static LabColorSpace Lab { get; } = new LabColorSpace(Srgb.ToXyz(Rgb.FromRgb(255, 255, 255)));
    }
}
