using System;
using System.Diagnostics;
using System.Windows.Controls;
using ImageLib;
using ImageLib.Agat;
using ImageLib.Apple;
using ImageLib.Spectrum;

namespace FilConvWpf.Native
{
    class NativeImagePresenter : IImagePresenter
    {
        private const int _defaultMode = 1; // MGR

        private NativeImage _nativeImage;
        private INativeDisplayMode _currentMode;
        private ToolbarFragment _toolbar;
        private ToolbarFragment _subBar;
        private ComboBox _displayModeCombo;

        public event EventHandler<EventArgs> DisplayImageChanged;

        public NativeImagePresenter(NativeImage nativeImage)
        {
            _nativeImage = nativeImage;
            _displayModeCombo = new ComboBox();
            foreach (INativeDisplayMode m in _displayModes)
            {
                _displayModeCombo.Items.Add(m.Name);
            }
            _displayModeCombo.SelectionChanged += displayModeCombo_SelectionChanged;
            SetMode(_defaultMode);
        }

        public AspectBitmap DisplayImage { get; private set; }
        public bool EnableAspectCorrection { get { return true; } }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
            _toolbar = fragment;
            _toolbar.Add(_displayModeCombo);
            _subBar = _toolbar.GetFragment(_displayModeCombo, null);
            if (_currentMode != null)
            {
                _currentMode.GrantToolbarFragment(_subBar);
            }
        }

        public void RevokeToolbarFragment()
        {
            if (_currentMode != null)
            {
                _currentMode.RevokeToolbarFragment();
            }
            _toolbar = null;
            _subBar = null;
        }

        private void displayModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, _displayModeCombo));
            SetMode(_displayModeCombo.SelectedIndex);
        }

        private void currentMode_FormatChanged(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, _currentMode));
            Convert(_currentMode.Format);
        }

        private void Convert(NativeImageFormat f)
        {
            DisplayImage = new AspectBitmap(f.FromNative(_nativeImage), f.Aspect);
            OnDisplayImageChanged();
        }

        protected virtual void OnDisplayImageChanged()
        {
            if (DisplayImageChanged != null)
            {
                DisplayImageChanged(this, EventArgs.Empty);
            }
        }

        private void SetMode(int mode)
        {
            _displayModeCombo.SelectedIndex = mode;

            if (!object.ReferenceEquals(_displayModes[mode], _currentMode))
            {
                if (_currentMode != null)
                {
                    _currentMode.FormatChanged -= currentMode_FormatChanged;
                    _currentMode.RevokeToolbarFragment();
                }
                _currentMode = _displayModes[mode];
                _currentMode.FormatChanged += currentMode_FormatChanged;
                if (_subBar != null)
                {
                    _subBar.Clear();
                    _currentMode.GrantToolbarFragment(_subBar);
                }

                Convert(_currentMode.Format);
            }
        }

        private static readonly INativeDisplayMode[] _displayModes =
        {
            new NativeDisplayMode("GR7", new Gr7ImageFormat()),
            new NativeDisplayMode("MGR", new MgrImageFormat()),
            new NativeDisplayMode("HGR", new HgrImageFormat()),
            new NativeDisplayMode("MGR9", new Mgr9ImageFormat()),
            new NativeDisplayMode("HGR9", new Hgr9ImageFormat()),
            new AppleDisplayMode(),
            new NativeDisplayMode("Apple ][ NTSC", new Apple2ImageFormat(new Apple2NtscTv(Apple2Palettes.American))),
            new NativeDisplayMode("Spectrum", new SpectrumImageFormatInterleave()),
            new NativeDisplayMode("Spectrum (Speccy)", new SpectrumImageFormatSpeccy()),
        };
    }
}
