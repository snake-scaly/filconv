using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib;

namespace FilConvWpf.Encode
{
    class NativeEncoding : IEncoding
    {
        private NativeImageFormat _format;
        private bool _dither;

        public event EventHandler<EventArgs> EncodingChanged;

        public NativeEncoding(string name, NativeImageFormat format)
        {
            Name = name;
            _format = format;
        }

        public string Name { get; private set; }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(
                _format.FromNative(ToNative(original)),
                _format.Aspect);
        }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
            Image ditherIcon = new Image();
            ditherIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/rainbow.png"));
            ToggleButton dither = new ToggleButton();
            dither.Content = ditherIcon;
            dither.ToolTip = "Улучшенная передача цветов";
            dither.IsChecked = _dither;
            dither.Checked += dither_Checked;
            dither.Unchecked += dither_Unchecked;
            fragment.Add(dither);
        }

        public void RevokeToolbarFragment()
        {
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
