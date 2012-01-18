namespace ImageLib
{
    public class HgrImageFormat : BwImageFormatAbstr
    {
        public override string Name
        {
            get { return "HGR"; }
        }

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
