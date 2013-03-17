using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib;
using System.Collections.Generic;

namespace FilConvWpf.Encode
{
    class NativeEncoding : IEncoding
    {
        private NativeImageFormat _format;
        private bool _dither;
        private ToggleButton _ditherButton;

        public event EventHandler<EventArgs> EncodingChanged;

        public NativeEncoding(string name, NativeImageFormat format)
        {
            Name = name;
            _format = format;

            ToolBar = new ToolBar();

            _ditherButton = new ToggleButton();
            _ditherButton.Content = ResourceUtils.GetResourceImage("rainbow.png");
            _ditherButton.ToolTip = "Улучшенная передача цветов";
            _ditherButton.IsChecked = _dither;
            _ditherButton.Checked += dither_Checked;
            _ditherButton.Unchecked += dither_Unchecked;
            ToolBar.Items.Add(_ditherButton);
        }

        public string Name { get; private set; }

        public ToolBar ToolBar { get; private set; }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(
                _format.FromNative(ToNative(original)),
                _format.Aspect);
        }

        public bool IsContainerSupported(Type type)
        {
            return type == typeof(Fil);
        }

        public void Encode(BitmapSource original, object container)
        {
            if (container is Fil)
            {
                Fil f = (Fil)container;
                f.Data = ToNative(original).Data;
            }
            else
            {
                throw new NotSupportedException("Unsupported container type: " + container.GetType());
            }
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[SettingNames.Dithering] = _dither;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            object o;

            if (settings.TryGetValue(SettingNames.Dithering, out o))
            {
                _dither = (bool)o;
                _ditherButton.IsChecked = _dither;
            }
        }

        private NativeImage ToNative(BitmapSource original)
        {
            EncodingOptions options = new EncodingOptions();
            options.Dither = _dither;
            return _format.ToNative(original, options);
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
