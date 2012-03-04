using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;
using ImageLib;

namespace FilConvWpf
{
    class PreviewModel
    {
        static readonly PictureScale defaultScale = PictureScale.Double;
        const bool defaultTvAspect = true;

        BitmapSource _displayPicture;
        bool _tvAspect;
        IImageDisplayAdapter _image;

        public event EventHandler<EventArgs> DisplayPictureChange;

        public PreviewModel()
        {
            Scale = defaultScale;
            TvAspect = defaultTvAspect;
        }

        public string Title { get; set; }

        public IImageDisplayAdapter Image
        {
            get { return _image; }
            set
            {
                if (!object.ReferenceEquals(value, _image))
                {
                    if (_image != null)
                    {
                        _image.DisplayImageChanged -= image_DisplayImageChanged;
                        _image.RevokeToolbarFragment();
                    }
                    Toolbar.Clear();

                    _image = value;

                    if (_image != null)
                    {
                        _image.DisplayImageChanged += image_DisplayImageChanged;
                        _image.GrantToolbarFragment(Toolbar);
                    }

                    _displayPicture = null;
                    OnDisplayPictureChange();
                }
            }
        }

        public BitmapSource DisplayPicture
        {
            get
            {
                if (_displayPicture == null)
                {
                    if (_image != null)
                    {
                        _displayPicture = _image.DisplayImage.Bitmap;
                    }
                }
                return _displayPicture;
            }
        }

        public bool TvAspect
        {
            get { return TvAspectEnabled && _tvAspect; }
            set { _tvAspect = value; }
        }

        public bool TvAspectEnabled
        {
            get { return _image != null && _image.EnableAspectCorrection; }
        }

        public double Aspect
        {
            get
            {
                if (TvAspect && TvAspectEnabled)
                {
                    return _image.DisplayImage.Aspect;
                }
                return 1;
            }
        }

        public PictureScale Scale { get; set; }

        public ToolbarFragment Toolbar { get; set; }

        protected virtual void OnDisplayPictureChange()
        {
            if (DisplayPictureChange != null)
            {
                DisplayPictureChange(this, EventArgs.Empty);
            }
        }

        private void image_DisplayImageChanged(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, _image));
            _displayPicture = null;
            OnDisplayPictureChange();
        }
    }
}
