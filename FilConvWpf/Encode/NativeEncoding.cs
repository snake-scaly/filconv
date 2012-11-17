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
        private bool _tvAspect;
        private bool _dither;
        private ToggleButton _aspectButton;
        private ToggleButton _ditherButton;

        public event EventHandler<EventArgs> EncodingChanged;

        public NativeEncoding(string name, NativeImageFormat format)
        {
            Name = name;
            _format = format;
            _tvAspect = true;

            if (format.Aspect != 1)
            {
                _aspectButton = new ToggleButton();
                _aspectButton.Content = ResourceUtils.GetResourceImage("television.png");
                _aspectButton.ToolTip = "Пропорциональное отображение";
                _aspectButton.IsChecked = _tvAspect;
                _aspectButton.Checked += aspect_Checked;
                _aspectButton.Unchecked += aspect_Unchecked;
            }

            _ditherButton = new ToggleButton();
            _ditherButton.Content = ResourceUtils.GetResourceImage("rainbow.png");
            _ditherButton.ToolTip = "Улучшенная передача цветов";
            _ditherButton.IsChecked = _dither;
            _ditherButton.Checked += dither_Checked;
            _ditherButton.Unchecked += dither_Unchecked;
        }

        public string Name { get; private set; }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(
                _format.FromNative(ToNative(original)),
                _tvAspect ? _format.Aspect : 1);
        }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
            if (_aspectButton != null)
            {
                fragment.Add(_aspectButton);
            }
            fragment.Add(_ditherButton);
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

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[SettingNames.OutTvAspect] = _tvAspect;
            settings[SettingNames.Dithering] = _dither;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
            object o;

            if (settings.TryGetValue(SettingNames.OutTvAspect, out o))
            {
                _tvAspect = (bool)o;
                if (_aspectButton != null)
                {
                    _aspectButton.IsChecked = _tvAspect;
                }
            }

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

        private void aspect_Checked(object sender, RoutedEventArgs e)
        {
            if (!_tvAspect)
            {
                _tvAspect = true;
                OnEncodingChanged();
            }
        }

        private void aspect_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_tvAspect)
            {
                _tvAspect = false;
                OnEncodingChanged();
            }
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
