﻿using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    /// <summary>
    /// An image to be displayed.
    /// </summary>
    class AspectBitmap
    {
        public BitmapSource Bitmap { get; private set; }

        /// <summary>
        /// Gets aspect ratio of the image pixels.
        /// </summary>
        /// <remarks>
        /// Aspect value is calculated as physical pixel width divided by physical
        /// pixel height as displayed by the original hardware.  For instance,
        /// aspect of 1.0 corresponds to square original pixels.
        /// </remarks>
        public double Aspect { get; private set; }

        public AspectBitmap(BitmapSource bitmap, double aspect)
        {
            Bitmap = bitmap;
            Aspect = aspect;
        }
    }
}