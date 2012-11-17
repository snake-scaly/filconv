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

        BitmapSource _displayPicture;
        IImagePresenter _imagePresenter;

        public event EventHandler<EventArgs> DisplayPictureChange;

        public PreviewModel()
        {
            Scale = defaultScale;
        }

        public string Title { get; set; }

        public IImagePresenter ImagePresenter
        {
            get { return _imagePresenter; }
            set
            {
                if (!object.ReferenceEquals(value, _imagePresenter))
                {
                    if (_imagePresenter != null)
                    {
                        _imagePresenter.DisplayImageChanged -= image_DisplayImageChanged;
                        _imagePresenter.RevokeToolbarFragment();
                    }
                    Toolbar.Clear();

                    _imagePresenter = value;

                    if (_imagePresenter != null)
                    {
                        _imagePresenter.DisplayImageChanged += image_DisplayImageChanged;
                        _imagePresenter.GrantToolbarFragment(Toolbar);
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
                if (_displayPicture == null && _imagePresenter != null && _imagePresenter.DisplayImage != null)
                {
                    _displayPicture = _imagePresenter.DisplayImage.Bitmap;
                }
                return _displayPicture;
            }
        }

        public double Aspect
        {
            get
            {
                return _imagePresenter.DisplayImage.Aspect;
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
            Debug.Assert(object.ReferenceEquals(sender, _imagePresenter));
            _displayPicture = null;
            OnDisplayPictureChange();
        }
    }
}
