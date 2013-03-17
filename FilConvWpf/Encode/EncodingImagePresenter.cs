using System;
using System.Diagnostics;
using System.Windows.Controls;
using ImageLib.Agat;
using System.Collections.Generic;

namespace FilConvWpf.Encode
{
    class EncodingImagePresenter : IImagePresenter
    {
        private const int _defaultEncoding = 2;

        private Preview _sourcePreview;
        private IEncoding _currentEncoding;
        private Dictionary<string, object> _settings;

        public event EventHandler<EventArgs> DisplayImageChanged;

        public EncodingImagePresenter(Preview sourcePreview)
        {
            _sourcePreview = sourcePreview;
            _sourcePreview.DisplayPictureChange += sourcePreview_DisplayPictureChange;

            SupportedPreviewModes = new string[_encodings.Length];
            int i = 0;
            foreach (IEncoding e in _encodings)
            {
                SupportedPreviewModes[i] = e.Name;
                i++;
            }

            _settings = new Dictionary<string, object>();

            PreviewMode = _defaultEncoding;
        }

        public AspectBitmap DisplayImage { get; private set; }

        public string[] SupportedPreviewModes { get; private set; }

        public int PreviewMode
        {
            get
            {
                return Array.IndexOf(_encodings, _currentEncoding);
            }
            set
            {
                if (!object.ReferenceEquals(_currentEncoding, _encodings[value]))
                {
                    if (_currentEncoding != null)
                    {
                        _currentEncoding.StoreSettings(_settings);
                        _currentEncoding.EncodingChanged -= currentEncoding_EncodingChanged;
                    }
                    _currentEncoding = _encodings[value];
                    _currentEncoding.AdoptSettings(_settings);
                    _currentEncoding.EncodingChanged += currentEncoding_EncodingChanged;

                    Encode();
                }
            }
        }

        public ToolBar ToolBar
        {
            get
            {
                return _currentEncoding.ToolBar;
            }
        }

        public bool IsContainerSupported(Type type)
        {
            return _currentEncoding != null && _currentEncoding.IsContainerSupported(type);
        }

        public void EncodeInto(object container)
        {
            _currentEncoding.Encode(_sourcePreview.DisplayPicture, container);
        }

        private void currentEncoding_EncodingChanged(object sender, EventArgs e)
        {
            Encode();
        }

        private void sourcePreview_DisplayPictureChange(object sender, EventArgs e)
        {
            Encode();
        }

        private void Encode()
        {
            if (_sourcePreview.DisplayPicture != null)
            {
                DisplayImage = _currentEncoding.Preview(_sourcePreview.DisplayPicture);
            }
            else
            {
                DisplayImage = null;
            }
            OnDisplayImageChanged();
        }

        private void OnDisplayImageChanged()
        {
            if (DisplayImageChanged != null)
            {
                DisplayImageChanged(this, EventArgs.Empty);
            }
        }

        private static readonly IEncoding[] _encodings =
        {
            new IdentityEncoding(),
            new NativeEncoding("GR7", new Gr7ImageFormat()),
            new NativeEncoding("MGR", new MgrImageFormat()),
            new NativeEncoding("HGR", new HgrImageFormat()),
            new NativeEncoding("MGR9", new Mgr9ImageFormat()),
            new NativeEncoding("HGR9", new Hgr9ImageFormat()),
        };
    }
}
