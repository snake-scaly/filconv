using System;
using ImageLib;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;

namespace FilConvWpf.Native
{
    class AppleDisplayMode : INativeDisplayMode
    {
        private bool _fill;
        private bool _pal;

        public event EventHandler<EventArgs> FormatChanged;

        public AppleDisplayMode()
        {
            Format = new Apple2ImageFormat(new Apple2SimpleTv(Apple2Palettes.European));
        }

        public string Name { get { return "Apple ]["; } }

        public NativeImageFormat Format { get; private set; }

        public void GrantToolbarFragment(ToolbarFragment fragment)
        {
            ToggleButton fill = new ToggleButton();
            fill.IsChecked = _fill;
            fill.Content = "З";
            fill.ToolTip = "Заливка цветов";
            fill.Checked += fill_Checked;
            fill.Unchecked += fill_Unchecked;
            fragment.Add(fill);

            ToggleButton pal = new ToggleButton();
            pal.IsChecked = _pal;
            pal.Content = "П";
            pal.ToolTip = "Переключение между европейской и американской палитрами";
            pal.Checked += pal_Checked;
            pal.Unchecked += pal_Unchecked;
            fragment.Add(pal);
        }

        public void RevokeToolbarFragment()
        {
        }

        private void fill_Checked(object sender, RoutedEventArgs e)
        {
            _fill = true;
            UpdateFormat();
        }

        private void fill_Unchecked(object sender, RoutedEventArgs e)
        {
            _fill = false;
            UpdateFormat();
        }

        private void pal_Checked(object sender, RoutedEventArgs e)
        {
            _pal = true;
            UpdateFormat();
        }

        private void pal_Unchecked(object sender, RoutedEventArgs e)
        {
            _pal = false;
            UpdateFormat();
        }

        private void UpdateFormat()
        {
            Color[] pal = _pal ? Apple2Palettes.American : Apple2Palettes.European;
            Apple2TvSet tv = _fill ? (Apple2TvSet)new Apple2FillTv(pal) : (Apple2TvSet)new Apple2SimpleTv(pal);
            Format = new Apple2ImageFormat(tv);
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
