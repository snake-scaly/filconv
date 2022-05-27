using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    class PreviewModel : INotifyPropertyChanged
    {
        private string _title;
        private IImagePresenter _imagePresenter;
        private bool _aspectToggleChecked;
        private double? _scale;

        public event PropertyChangedEventHandler PropertyChanged;

        public PreviewModel()
        {
            _aspectToggleChecked = true;
        }

        public string Title
        {
            get => _title;

            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

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

                    OnDisplayPictureChange();
                    OnPropertyChanged(nameof(ToolBarItems));
                }
            }
        }

        public BitmapSource DisplayPicture => _imagePresenter?.DisplayImage?.Bitmap;

        public Visibility AspectToggleVisibility =>
            _imagePresenter?.DisplayImage?.Aspect != 1 ? Visibility.Visible : Visibility.Collapsed;

        public bool AspectToggleChecked
        {
            get => _aspectToggleChecked;

            set
            {
                if (value != _aspectToggleChecked)
                {
                    _aspectToggleChecked = value;
                    OnPropertyChanged(nameof(AspectToggleChecked));
                    OnPropertyChanged(nameof(Aspect));
                }
            }
        }

        public double Aspect => _aspectToggleChecked ? _imagePresenter?.DisplayImage?.Aspect ?? 1 : 1;

        public double? Scale
        {
            get => _scale;

            set
            {
                if (!Equals(value, _scale))
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                    OnPropertyChanged(nameof(ScrollBarVisibility));
                    OnPropertyChanged(nameof(BitmapCursor));
                }
            }
        }

        public ScrollBarVisibility ScrollBarVisibility =>
            Scale == null ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;

        public Cursor BitmapCursor => ScrollBarVisibility == ScrollBarVisibility.Auto ? Cursors.Hand : Cursors.Arrow;

        public IEnumerable ToolBarItems => _imagePresenter?.Tools.Select(t => t.Element);

        protected virtual void OnDisplayPictureChange()
        {
            OnPropertyChanged(nameof(DisplayPicture));
            OnPropertyChanged(nameof(Aspect));
            OnPropertyChanged(nameof(AspectToggleVisibility));
        }

        private void imagePresenter_DisplayImageChanged(object sender, EventArgs e)
        {
            Debug.Assert(ReferenceEquals(sender, _imagePresenter));
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
