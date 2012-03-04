namespace ImageLib.Agat
{
    public class HgrImageFormat : BwImageFormatAbstr
    {
        protected override int Width
        {
            get { return 256; }
        }

        protected override int Height
        {
            get { return 256; }
        }
    }
}
