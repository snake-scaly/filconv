using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Apple
{
    class Apple2Utils
    {
        public static int GetHiResLineOffset(int lineIndex)
        {
            int block = lineIndex & 7;
            int subBlock = (lineIndex >> 3) & 7;
            int line = (lineIndex >> 6) & 3;
            return block * 1024 + subBlock * 128 + line * 40;
        }

        public static int GetLoResLineOffset(int lineIndex)
        {
            const int _bytesPerByteLine = 40;
            const int _bytesPerBlock = 128;

            int block = (lineIndex >> 1) & 7;
            int lineInBlock = (lineIndex >> 4) & 3;
            return _bytesPerBlock * block + _bytesPerByteLine * lineInBlock;
        }
    }
}
