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
        const bool defaultDither = false;

        BitmapSource _bitmapPicture;
        int _format;
        BitmapSource _displayPicture;
        bool _tvAspect;
        bool _dither;
        IImageDisplayAdapter _image;

        public event EventHandler<EventArgs> DisplayPictureChange;

        public PreviewModel()
        {
            _format = 1;
            Scale = defaultScale;
            TvAspect = defaultTvAspect;
            _dither = defaultDither;
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

        public BitmapSource BitmapPicture
        {
            get { return _bitmapPicture; }

            set
            {
                if (!object.ReferenceEquals(value, _bitmapPicture))
                {
                    _bitmapPicture = value;
                    if (value != null)
                    {
                        _image = null;
                        _displayPicture = null;
                    }
                    OnDisplayPictureChange();
                }
            }
        }

        public IList<String> PreviewModes
        {
            get 
            {
                var modes = new List<String>();
                if (Encode)
                {
                    modes.Add("Оригинал");
                }
                modes.AddRange(_formats.Select(f => f.Item2));
                return modes;
            }
        }

        public int CurrentPreviewMode
        {
            get { return _format + (Encode ? 1 : 0); }

            set
            {
                if (Encode)
                {
                    --value;
                }
                if (value != _format)
                {
                    _format = value;
                    _displayPicture = null;
                    OnDisplayPictureChange();
                }
            }
        }

        public NativeImageFormat Format
        {
            get { return _formats[_format].Item1; }
        }

        public BitmapSource DisplayPicture
        {
            get
            {
                if (_displayPicture == null)
                {
                    if (_bitmapPicture != null)
                    {
                        if (Encode && _format != -1)
                        {
                            NativeImageFormat format = _formats[_format].Item1;
                            EncodingOptions options = new EncodingOptions();
                            options.Dither = Dither;
                            _displayPicture = format.FromNative(format.ToNative(_bitmapPicture, options));
                        }
                        else
                        {
                            _displayPicture = _bitmapPicture;
                        }
                    }
                    else if (_image != null)
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

        public bool DisplayFormatBox
        {
            get { return false; }
        }

        public bool Encode { get; set; }

        public bool Dither
        {
            get { return _dither; }
            set
            {
                if (value != _dither)
                {
                    _dither = value;
                    _displayPicture = null;
                    OnDisplayPictureChange();
                }
            }
        }

        public bool DitherEnabled
        {
            get { return _format != -1; }
        }

        public bool DitherVisible
        {
            get { return Encode; }
        }

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

        static readonly Tuple<NativeImageFormat, String>[] _formats =
        {
            Tuple.Create((NativeImageFormat)new Gr7ImageFormat(), "GR7"),
            Tuple.Create((NativeImageFormat)new MgrImageFormat(), "MGR"),
            Tuple.Create((NativeImageFormat)new HgrImageFormat(), "HGR"),
            Tuple.Create((NativeImageFormat)new Mgr9ImageFormat(), "MGR9"),
            Tuple.Create((NativeImageFormat)new Hgr9ImageFormat(), "HGR9"),
            Tuple.Create((NativeImageFormat)new Apple2ImageFormat(new Apple2SimpleTv(Apple2Palettes.European)), "Apple Simple EU"),
            Tuple.Create((NativeImageFormat)new Apple2ImageFormat(new Apple2SimpleTv(Apple2Palettes.American)), "Apple Simple US"),
            Tuple.Create((NativeImageFormat)new Apple2ImageFormat(new Apple2FillTv(Apple2Palettes.European)), "Apple Fill EU"),
            Tuple.Create((NativeImageFormat)new Apple2ImageFormat(new Apple2FillTv(Apple2Palettes.American)), "Apple Fill US"),
            Tuple.Create((NativeImageFormat)new Apple2ImageFormat(new Apple2DoublePixelTv(Apple2Palettes.European)), "Apple Double Pixel EU"),
            Tuple.Create((NativeImageFormat)new Apple2ImageFormat(new Apple2DoublePixelTv(Apple2Palettes.American)), "Apple Double Pixel US"),
            Tuple.Create((NativeImageFormat)new Apple2ImageFormat(new Apple2NtscTv(Apple2Palettes.American)), "Apple NTSC"),
        };
    }
}
