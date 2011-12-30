using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public class Mgr9ImageFormat : AgatImageFormatAbstr
    {
        public override string Name
        {
            get { return "MGR9"; }
        }

        public override int Width
        {
            get { return 256; }
        }

        public override int Height
        {
            get { return 256; }
        }

        protected override int BitsPerPixel
        {
            get { return 2; }
        }

        protected override Color[] Palette
        {
            get { return _colorPalette; }
        }

        protected override int GetLineOffset(int y)
        {
            int bank;
            int line = Math.DivRem(y, 2, out bank);
            return (line + Height / 2 * bank) * BytesPerScanline;
        }

        private static readonly Color[] _colorPalette =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(0, 0, 255),
        };
    }
}
