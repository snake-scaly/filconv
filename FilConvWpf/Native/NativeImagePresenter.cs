using System;
using System.Diagnostics;
using System.Windows.Controls;
using ImageLib;
using ImageLib.Agat;
using ImageLib.Apple;
using ImageLib.Spectrum;
using System.Collections.Generic;

namespace FilConvWpf.Native
{
    class NativeImagePresenter : IImagePresenter
    {
        private const int _defaultMode = 1; // MGR

        private NativeImage _nativeImage;
        private INativeDisplayMode _currentMode;
        private Dictionary<string, object> _settings;

        public event EventHandler<EventArgs> DisplayImageChanged;

        public NativeImagePresenter(NativeImage nativeImage)
        {
            _nativeImage = nativeImage;
            _settings = new Dictionary<string, object>();

            var displayModeNames = new List<string>();
            foreach (INativeDisplayMode m in _displayModes)
            {
                displayModeNames.Add(m.Name);
            }
            SupportedPreviewModes = displayModeNames.ToArray();

            PreviewMode = _defaultMode;
        }

        public AspectBitmap DisplayImage { get; private set; }

        public string[] SupportedPreviewModes { get; private set; }
        
        public int PreviewMode
        {
            get
            {
                return Array.IndexOf(_displayModes, _currentMode);
            }

            set
            {
                if (value < 0 || value >= _displayModes.Length)
                {
                    value = _defaultMode;
                }
                if (!object.ReferenceEquals(_displayModes[value], _currentMode))
                {
                    if (_currentMode != null)
                    {
                        _currentMode.StoreSettings(_settings);
                        _currentMode.FormatChanged -= currentMode_FormatChanged;
                    }
                    _currentMode = _displayModes[value];
                    _currentMode.AdoptSettings(_settings);

                    _currentMode.FormatChanged += currentMode_FormatChanged;

                    Convert(_currentMode.Format);
                }
            }
        }

        public ToolBar ToolBar
        {
            get
            {
                return _currentMode.ToolBar;
            }
        }

        private void currentMode_FormatChanged(object sender, EventArgs e)
        {
            Debug.Assert(object.ReferenceEquals(sender, _currentMode));
            Convert(_currentMode.Format);
        }

        private void Convert(NativeImageFormat f)
        {
            DisplayImage = new AspectBitmap(f.FromNative(_nativeImage), _currentMode.Aspect);
            OnDisplayImageChanged();
        }

        protected virtual void OnDisplayImageChanged()
        {
            if (DisplayImageChanged != null)
            {
                DisplayImageChanged(this, EventArgs.Empty);
            }
        }

        private static readonly INativeDisplayMode[] _displayModes =
        {
            new NativeDisplayMode("FormatNameGR7", new Gr7ImageFormat()),
            new NativeDisplayMode("FormatNameMGR", new MgrImageFormat()),
            new NativeDisplayMode("FormatNameHGR", new HgrImageFormat()),
            new NativeDisplayMode("FormatNameMGR9", new Mgr9ImageFormat()),
            new NativeDisplayMode("FormatNameHGR9", new Hgr9ImageFormat()),
            new NativeDisplayMode("FormatNamePicler", new SpectrumImageFormatPicler()),
            new AppleLoResDisplayMode(),
            new AppleHiResDisplayMode(),
            new NativeDisplayMode("FormatNameApple2NTSC", new Apple2HiResImageFormat(new Apple2NtscTv(Apple2Palettes.American))),
            new NativeDisplayMode("FormatNameSpectrum", new SpectrumImageFormatInterleave()),
        };
    }
}
