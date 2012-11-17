using System;
using ImageLib;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace FilConvWpf.Native
{
    class NativeDisplayMode : INativeDisplayMode
    {
        private const bool _defaultTvAspect = true;
        private bool _tvAspect;
        private ToggleButton _aspectButton;
        public event EventHandler<EventArgs> FormatChanged;

        public NativeDisplayMode(string name, NativeImageFormat format)
        {
            Name = name;
            Format = format;
            _tvAspect = _defaultTvAspect;
            Aspect = _tvAspect ? Format.Aspect : 1;

            if (Format.Aspect != 1)
            {
                _aspectButton = new ToggleButton();
                _aspectButton.IsChecked = _tvAspect;
                _aspectButton.Content = ResourceUtils.GetResourceImage("television.png");
                _aspectButton.ToolTip = "Пропорции оригинала";
                _aspectButton.Checked += aspect_Checked;
                _aspectButton.Unchecked += aspect_Unchecked;
            }
        }

        public string Name { get; private set; }
        public double Aspect { get; private set; }
        public NativeImageFormat Format { get; protected set; }

        public virtual void GrantToolbarFragment(ToolbarFragment fragment)
        {
            if (_aspectButton != null)
            {
                fragment.Add(_aspectButton);
            }
        }

        public virtual void RevokeToolbarFragment()
        {
        }

        public virtual void StoreSettings(IDictionary<string, object> settings)
        {
            settings[SettingNames.TvAspect] = _tvAspect;
        }

        public virtual void AdoptSettings(IDictionary<string, object> settings)
        {
            object tvAspect;
            if (settings.TryGetValue(SettingNames.TvAspect, out tvAspect))
            {
                _tvAspect = (bool)tvAspect;
                if (_aspectButton != null)
                {
                    _aspectButton.IsChecked = _tvAspect;
                }
            }
        }

        private void aspect_Checked(object sender, EventArgs e)
        {
            _tvAspect = true;
            Aspect = Format.Aspect;
            OnFormatChanged();
        }

        private void aspect_Unchecked(object sender, EventArgs e)
        {
            _tvAspect = false;
            Aspect = 1;
            OnFormatChanged();
        }

        protected virtual void OnFormatChanged()
        {
            if (FormatChanged != null)
            {
                FormatChanged(this, EventArgs.Empty);
            }
        }
    }
}
