﻿using System;
using FilConvWpf.Native;
using ImageLib;
using System.Windows.Controls;
using System.Diagnostics;

namespace FilConvWpf.Encode
{
    class EncodingImageDisplayAdapter : IImageDisplayAdapter
    {
        private const int _defaultEncoding = 2;

        private Preview _sourcePreview;
        private ComboBox _encodingCombo;
        private IEncoding _currentEncoding;
        private ToolbarFragment _subBar;

        public event EventHandler<EventArgs> DisplayImageChanged;

        public EncodingImageDisplayAdapter(Preview sourcePreview)
        {
            _sourcePreview = sourcePreview;
            _sourcePreview.DisplayPictureChange += sourcePreview_DisplayPictureChange;

            _encodingCombo = new ComboBox();
            foreach (IEncoding e in _encodings)
            {
                _encodingCombo.Items.Add(e.Name);
            }
            _encodingCombo.SelectionChanged += encodingCombo_SelectionChanged;

            SetEncoding(_defaultEncoding);
        }

        public bool EnableAspectCorrection { get { return true; } }

        public DisplayImage DisplayImage { get; private set; }

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

        public void FillContainerData(object container)
        {
            _currentEncoding.FillContainerData(container, _sourcePreview.DisplayPicture);
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
                    _currentEncoding.EncodingChanged -= currentEncoding_EncodingChanged;
                    _currentEncoding.RevokeToolbarFragment();
                }
                _currentEncoding = _encodings[encoding];
                _currentEncoding.EncodingChanged += currentEncoding_EncodingChanged;
                if (_subBar != null)
                {
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
                DisplayImage = _currentEncoding.Encode(_sourcePreview.DisplayPicture);
            }
            else
            {
                DisplayImage = new DisplayImage();
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
