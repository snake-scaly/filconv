namespace ImageLib.Apple
{
    class Apple2Utils
    {
        public static int GetHiResLineOffset(int lineIndex)
        {
            const int bytesPerSuperblock = 1024;
            const int bytesPerBlock = 128;
            const int bytesPerLine = 40;

            int superblock = lineIndex & 7;
            int block = (lineIndex >> 3) & 7;
            int lineInBlock = (lineIndex >> 6) & 3;
            return bytesPerSuperblock * superblock + bytesPerBlock * block + bytesPerLine * lineInBlock;
        }

        public static int GetTextLineOffset(int lineIndex)
        {
            const int bytesPerBlock = 128;
            const int bytesPerLine = 40;

            int block = lineIndex & 7;
            int lineInBlock = (lineIndex >> 3) & 3;
            return bytesPerBlock * block + bytesPerLine * lineInBlock;
        }
    }
}
