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
        private ComboBox _encodingCombo;
        private IEncoding _currentEncoding;
        private ToolbarFragment _subBar;
        private Dictionary<string, object> _settings;

        public event EventHandler<EventArgs> DisplayImageChanged;

        public EncodingImagePresenter(Preview sourcePreview)
        {
            _sourcePreview = sourcePreview;
            _sourcePreview.DisplayPictureChange += sourcePreview_DisplayPictureChange;

            _encodingCombo = new ComboBox();
            foreach (IEncoding e in _encodings)
            {
                _encodingCombo.Items.Add(e.Name);
            }
            _encodingCombo.SelectionChanged += encodingCombo_SelectionChanged;

            _settings = new Dictionary<string, object>();

            SetEncoding(_defaultEncoding);
        }

        public bool EnableAspectCorrection { get { return true; } }

        public AspectBitmap DisplayImage { get; private set; }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
            fragment.Add(_encodingCombo);
            _subBar = fragment.GetFragment(_encodingCombo, null);
            if (_currentEncoding != null)
            {
                _currentEncoding.GrantToolbarFragment(_subBar);
            }
        }

        public void RevokeToolbarFragment()
        {
            if (_currentEncoding != null)
            {
                _currentEncoding.RevokeToolbarFragment();
            }
            _subBar = null;
        }

        public bool IsContainerSupported(Type type)
        {
            return _currentEncoding != null && _currentEncoding.IsContainerSupported(type);
        }

        public void EncodeInto(object container)
        {
            _currentEncoding.Encode(_sourcePreview.DisplayPicture, container);
        }

        private void encodingCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, _encodingCombo));
            SetEncoding(_encodingCombo.SelectedIndex);
        }

        private void currentEncoding_EncodingChanged(object sender, EventArgs e)
        {
            Encode();
        }

        private void sourcePreview_DisplayPictureChange(object sender, EventArgs e)
        {
            Encode();
        }

        private void SetEncoding(int encoding)
        {
            _encodingCombo.SelectedIndex = encoding;

            if (!object.ReferenceEquals(_currentEncoding, _encodings[encoding]))
            {
                if (_currentEncoding != null)
                {
                    _currentEncoding.StoreSettings(_settings);
                    _currentEncoding.EncodingChanged -= currentEncoding_EncodingChanged;
                    _currentEncoding.RevokeToolbarFragment();
                }
                _currentEncoding = _encodings[encoding];
                _currentEncoding.EncodingChanged += currentEncoding_EncodingChanged;
                if (_subBar != null)
                {
                    _currentEncoding.AdoptSettings(_settings);
                    _subBar.Clear();
                    _currentEncoding.GrantToolbarFragment(_subBar);
                }

                Encode();
            }
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
