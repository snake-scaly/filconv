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
        public event EventHandler<EventArgs> FormatChanged;

        public NativeDisplayMode(string name, NativeImageFormat format)
        {
            Name = name;
            Format = format;
            _tvAspect = _defaultTvAspect;
            Aspect = _tvAspect ? Format.Aspect : 1;
        }

        public string Name { get; private set; }
        public double Aspect { get; private set; }
        public NativeImageFormat Format { get; protected set; }

        public virtual void GrantToolbarFragment(ToolbarFragment fragment)
        {
            if (Format.Aspect != 1)
            {
                ToggleButton aspect = new ToggleButton();
                aspect.IsChecked = _tvAspect;
                aspect.Content = ResourceUtils.GetResourceImage("television.png");
                aspect.ToolTip = "Пропорции Агата";
                aspect.Checked += aspect_Checked;
                aspect.Unchecked += aspect_Unchecked;
                fragment.Add(aspect);
            }
        }

        public virtual void RevokeToolbarFragment()
        {
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
