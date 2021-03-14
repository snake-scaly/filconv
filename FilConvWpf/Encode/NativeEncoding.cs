using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using FilConvWpf.I18n;
using ImageLib;

namespace FilConvWpf.Encode
{
    class NativeEncoding : IEncoding
    {
        private INativeImageFormat _format;
        private bool _dither;
        private ToggleButton _ditherButton;

        public event EventHandler<EventArgs> EncodingChanged;

        public NativeEncoding(string name, INativeImageFormat format, bool canDither)
        {
            Name = name;
            _format = format;

            if (canDither)
            {
                ToolBar = new ToolBar();

                Label label = new Label();
                L10n.AddLocalizedProperty(label, Label.ContentProperty, "EncodingToolbarTitle").Update();
                ToolBar.Items.Add(label);

                _ditherButton = new ToggleButton();
                _ditherButton.Content = ResourceUtils.GetResourceImage("rainbow.png");
                L10n.AddLocalizedProperty(_ditherButton, ToggleButton.ToolTipProperty, "ColorDitherToggleTooltip").Update();
                _ditherButton.IsChecked = _dither;
                _ditherButton.Checked += dither_Checked;
                _ditherButton.Unchecked += dither_Unchecked;
                ToolBar.Items.Add(_ditherButton);
            }
        }

        public string Name { get; private set; }

        public ToolBar ToolBar { get; private set; }

        private EncodingOptions EncodingOptions
        {
            get
            {
                EncodingOptions options = new EncodingOptions();
                options.Dither = _dither;
                return options;
            }
        }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(
                _format.FromNative(ToNative(original)),
                _format.Aspect);
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            yield return new FilSaveDelegate(original, _format, EncodingOptions);
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[SettingNames.Dithering] = _dither;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            object o;

            if (_ditherButton != null && settings.TryGetValue(SettingNames.Dithering, out o))
            {
                _dither = (bool)o;
                _ditherButton.IsChecked = _dither;
            }
        }

        private NativeImage ToNative(BitmapSource original)
        {
            return _format.ToNative(original, EncodingOptions);
        }

        private void dither_Checked(object sender, RoutedEventArgs e)
        {
            if (!_dither)
            {
                _dither = true;
                OnEncodingChanged();
            }
        }

        private void dither_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_dither)
            {
                _dither = false;
                OnEncodingChanged();
            }
        }

        protected virtual void OnEncodingChanged()
        {
            if (EncodingChanged != null)
            {
                EncodingChanged(this, EventArgs.Empty);
            }
        }
    }
}
