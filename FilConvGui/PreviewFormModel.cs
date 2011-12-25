using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FilLib;
using ImageLib;
using System.Drawing;

namespace FilConvGui
{
    class PreviewFormModel
    {
        PictureAspect _pictureAspect;
        PictureScale _pictureScale;

        public Fil FilPicture { get; set; }
        public AgatImageFormat FilPictureFormat { get; set; }
        public Bitmap BitmapPicture { get; set; }

        public PictureAspect Aspect
        {
            get
            {
                return _pictureAspect;
            }
            set
            {
                _pictureAspect = value;
                if (value != PictureAspect.Original && _pictureScale == PictureScale.Free)
                {
                    // PictureBox does not support scaling with arbitrary aspect
                    _pictureScale = PictureScale.Double;
                }
            }
        }

        public PictureScale Scale
        {
            get
            {
                return _pictureScale;
            }
            set
            {
                _pictureScale = value;
                if (value.ResizeToFit)
                {
                    // PictureBox does not support scaling with arbitrary aspect
                    Aspect = PictureAspect.Original;
                }
            }
        }
    }
}
