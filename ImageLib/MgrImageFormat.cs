namespace ImageLib
{
    public class MgrImageFormat : C16ImageFormatAbstr
    {
        public override string Name
        {
            get { return "MGR"; }
        }

        protected override int Width
        {
            get { return 128; }
        }

        protected override int Height
        {
            get { return 128; }
        }
    }
}
