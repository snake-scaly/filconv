﻿using System;
using System.Windows.Media;

namespace ImageLib
{
    public class Mgr9ImageFormat : AgatImageFormatAbstr
    {
        protected override int Width
        {
            get { return 256; }
        }

        protected override int Height
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
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(255, 0, 0),
            Color.FromRgb(0, 255, 0),
            Color.FromRgb(0, 0, 255),
        };
    }
}
