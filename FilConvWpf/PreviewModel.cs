using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FilLib;
using ImageLib;
using System.Drawing;
using System.Diagnostics;

namespace FilConvWpf
{
    class PreviewModel
    {
        static readonly PictureScale defaultScale = PictureScale.Double;
        const bool defaultTvAspect = true;
        const bool defaultDither = false;

        Fil _filPicture;
        Bitmap _bitmapPicture;
        AgatImageFormat _format;
        Bitmap _displayPicture;
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

        public Fil FilPicture
        {
            get { return _filPicture; }
            set
            {
                if (value != _filPicture)
                {
                    _filPicture = value;
                    _displayPicture = null;
                    if (value != null)
                    {
                        _bitmapPicture = null;
                    }
                    OnDisplayPictureChange();
                }
            }
        }

        public Bitmap BitmapPicture
        {
            get { return _bitmapPicture; }
            set
            {
                if (value != _bitmapPicture)
                {
                    _bitmapPicture = value;
                    if (value != null)
                    {
                        _filPicture = null;
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
                AgatImageFormat newFormat = null;
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

        public IList<AgatImageFormat> FormatList
        {
            get
            {
                var formats = new List<AgatImageFormat>();
                return new AgatImageFormat[] { null, null };
            }
        }

        public AgatImageFormat Format
        {
            get { return _format; }
        }

        public Bitmap DisplayPicture
        {
            get
            {
                if (_displayPicture == null)
                {
                    if (_bitmapPicture != null)
                    {
                        if (Encode && _format != null)
                        {
                            _displayPicture = AgatImageConverter.GetBitmap(
                                AgatImageConverter.GetBytes(_bitmapPicture, _format, _dither),
                                _format);
                        }
                        else
                        {
                            _displayPicture = _bitmapPicture;
                        }
                    }
                    else if (_filPicture != null)
                    {
                        Debug.Assert(_format != null);
                        _displayPicture = AgatImageConverter.GetBitmap(_filPicture.Data, _format);
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
            get { return _format != null && (Encode || _filPicture != null); }
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
            get { return _filPicture != null || Encode; }
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

        static readonly AgatImageFormat[] _formats =
        {
            new Gr7ImageFormat(),
            new MgrImageFormat(),
            new HgrImageFormat(),
            new Mgr9ImageFormat(),
            new Hgr9ImageFormat(),
        };
    }
}
