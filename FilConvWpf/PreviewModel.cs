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

        NativeImage _nativeImage;
        BitmapSource _bitmapPicture;
        NativeImageFormat _format;
        BitmapSource _displayPicture;
        bool _tvAspect;
        bool _dither;

        public event EventHandler<EventArgs> DisplayPictureChange;

        public PreviewModel()
        {
            _format = _formats[1];
            Scale = defaultScale;
            TvAspect = defaultTvAspect;
            _dither = defaultDither;
        }

        public string Title { get; set; }

        public NativeImage NativeImage
        {
            get { return _nativeImage; }
            set
            {
                if (value != _nativeImage)
                {
                    _nativeImage = value;
                    _displayPicture = null;
                    if (value != null)
                    {
                        _bitmapPicture = null;
                    }
                    OnDisplayPictureChange();
                }
            }
        }

        public BitmapSource BitmapPicture
        {
            get { return _bitmapPicture; }
            set
            {
                if (value != _bitmapPicture)
                {
                    _bitmapPicture = value;
                    if (value != null)
                    {
                        _nativeImage = null;
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
                modes.AddRange(_formats.Select(f => f.Name));
                return modes;
            }
        }

        public int CurrentPreviewMode
        {
            get
            {
                if (_format == null)
                {
                    Debug.Assert(Encode);
                    return 0;
                }

                int i = Array.IndexOf(_formats, _format);
                Debug.Assert(i != -1);
                return i + (Encode ? 1 : 0);
            }

            set
            {
                NativeImageFormat newFormat = null;
                if (Encode)
                {
                    --value;
                }
                if (value >= 0)
                {
                    newFormat = _formats[value];
                }
                if (newFormat != _format)
                {
                    _format = newFormat;
                    _displayPicture = null;
                    OnDisplayPictureChange();
                }
            }
        }

        public NativeImageFormat Format
        {
            get { return _format; }
        }

        public BitmapSource DisplayPicture
        {
            get
            {
                if (_displayPicture == null)
                {
                    if (_bitmapPicture != null)
                    {
                        if (Encode && _format != null)
                        {
                            EncodingOptions options = new EncodingOptions();
                            options.Dither = Dither;
                            _displayPicture = _format.FromNative(_format.ToNative(_bitmapPicture, options));
                        }
                        else
                        {
                            _displayPicture = _bitmapPicture;
                        }
                    }
                    else if (_nativeImage != null)
                    {
                        Debug.Assert(_format != null);
                        _displayPicture = _format.FromNative(_nativeImage);
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
            get { return _format != null && (Encode || _nativeImage != null); }
        }

        public double Aspect
        {
            get
            {
                if (TvAspect && TvAspectEnabled)
                {
                    return _format.Aspect;
                }
                return 1;
            }
        }

        public PictureScale Scale { get; set; }

        public bool DisplayFormatBox
        {
            get { return _nativeImage != null || Encode; }
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
            get { return _format != null; }
        }

        public bool DitherVisible
        {
            get { return Encode; }
        }

        protected void OnDisplayPictureChange()
        {
            if (DisplayPictureChange != null)
            {
                DisplayPictureChange(this, EventArgs.Empty);
            }
        }

        static readonly NativeImageFormat[] _formats =
        {
            new Gr7ImageFormat(),
            new MgrImageFormat(),
            new HgrImageFormat(),
            new Mgr9ImageFormat(),
            new Hgr9ImageFormat(),
        };
    }
}
