using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Apple
{
    class AppleScreenLoRes : AppleScreen
    {
        public int Width
        {
            get { return 40; }
        }

        public int Height
        {
            get { return 48; }
        }

        public int ByteWidth
        {
            get { return 40; }
        }

        public byte[] Pixels
        {
            get { throw new NotImplementedException(); }
        }

        public int GetLineOffset(int lineIndex)
        {
            throw new NotImplementedException();
        }
    }
}
