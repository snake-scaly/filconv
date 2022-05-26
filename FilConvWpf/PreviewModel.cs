using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    class PreviewModel : INotifyPropertyChanged
    {
        private static readonly PictureScale defaultScale = PictureScale.Double;

        private BitmapSource _displayPicture;
        private IImagePresenter _imagePresenter;
        private bool _aspectToggleChecked;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> DisplayPictureChange;

        public PreviewModel()
        {
            Scale = defaultScale;
            _aspectToggleChecked = true;
        }

        public string Title { get; set; }

        public IImagePresenter ImagePresenter
        {
            get => _imagePresenter;

            set
            {
                if (!ReferenceEquals(value, _imagePresenter))
                {
                    if (_imagePresenter != null)
                    {
                        _imagePresenter.DisplayImageChanged -= imagePresenter_DisplayImageChanged;
                        _imagePresenter.ToolBarChanged -= ImagePresenter_ToolBarChanged;
                    }

                    _imagePresenter = value;

                    if (_imagePresenter != null)
                    {
                        _imagePresenter.DisplayImageChanged += imagePresenter_DisplayImageChanged;
                        _imagePresenter.ToolBarChanged += ImagePresenter_ToolBarChanged;
                    }

                    _displayPicture = null;
                    OnDisplayPictureChange();
                    OnPropertyChanged(nameof(ToolBarItems));
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

        public bool AspectToggleEnabled =>
            _imagePresenter?.DisplayImage != null && _imagePresenter.DisplayImage.Aspect != 1;

        public bool AspectToggleChecked
        {
            get => _aspectToggleChecked;
            set => _aspectToggleChecked = value;
        }

        public double Aspect => AspectToggleEnabled && _aspectToggleChecked ? _imagePresenter.DisplayImage.Aspect : 1;

        public PictureScale Scale { get; set; }

        public IEnumerable ToolBarItems => _imagePresenter?.Tools.Select(t => t.Element);

        protected virtual void OnDisplayPictureChange()
        {
            DisplayPictureChange?.Invoke(this, EventArgs.Empty);
        }

        private void imagePresenter_DisplayImageChanged(object sender, EventArgs e)
        {
            Debug.Assert(ReferenceEquals(sender, _imagePresenter));
            _displayPicture = null;
            OnDisplayPictureChange();
        }

        private void ImagePresenter_ToolBarChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(ToolBarItems));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
