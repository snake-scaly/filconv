namespace ImageLib
{
    public interface IWriteablePixels<in T>
    {
        int Width { get; }
        int Height { get; }
        void SetPixel(int x, int y, T pixel);
    }
}
