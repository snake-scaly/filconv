using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;
using ImageLib;
using System.Windows.Controls;

namespace FilConvWpf
{
    class PreviewModel
    {
        static readonly PictureScale defaultScale = PictureScale.Double;

        BitmapSource _displayPicture;
        IImagePresenter _imagePresenter;
        bool _aspectToggleChecked;

        public PreviewModel()
        {
            Scale = defaultScale;
            _aspectToggleChecked = true;
        }

        public event EventHandler<EventArgs> DisplayPictureChange;

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
                    }

                    _imagePresenter = value;

                    if (_imagePresenter != null)
                    {
                        _imagePresenter.DisplayImageChanged += image_DisplayImageChanged;
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

        public bool AspectToggleEnabled
        {
            get
            {
                return _imagePresenter != null && _imagePresenter.DisplayImage != null && _imagePresenter.DisplayImage.Aspect != 1;
            }
        }

        public bool AspectToggleChecked
        {
            get
            {
                return _aspectToggleChecked;
            }
            set
            {
                _aspectToggleChecked = value;
            }
        }

        public double Aspect
        {
            get
            {
                return AspectToggleEnabled && _aspectToggleChecked ? _imagePresenter.DisplayImage.Aspect : 1;
            }
        }

        public PictureScale Scale { get; set; }

        public string[] SupportedPreviewModes
        {
            get
            {
                return _imagePresenter != null ? _imagePresenter.SupportedPreviewModes : null;
            }
        }

        public int PreviewMode
        {
            get
            {
                return _imagePresenter.PreviewMode;
            }
            set
            {
                _imagePresenter.PreviewMode = value;
            }
        }

        public ToolBar ToolBar
        {
            get
            {
                return _imagePresenter != null ? _imagePresenter.ToolBar : null;
            }
        }

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
