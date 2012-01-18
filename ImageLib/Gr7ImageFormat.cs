namespace ImageLib
{
    public class Gr7ImageFormat : C16ImageFormatAbstr
    {
        public override string Name
        {
            get { return "GR7"; }
        }

        protected override int Width
        {
            get { return 64; }
        }

        protected override int Height
        {
            get { return 64; }
        }
    }
}
