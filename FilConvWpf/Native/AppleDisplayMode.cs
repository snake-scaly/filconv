using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageLib;
using ImageLib.Apple;

namespace FilConvWpf.Native
{
    class AppleDisplayMode : NativeDisplayMode
    {
        private bool _fill;
        private bool _pal;

        public AppleDisplayMode()
            : base("Apple ][", new Apple2ImageFormat(new Apple2SimpleTv(Apple2Palettes.European)))
        {
        }

        public override void GrantToolbarFragment(ToolbarFragment fragment)
        {
            base.GrantToolbarFragment(fragment);

            ToggleButton fill = new ToggleButton();
            fill.IsChecked = _fill;
            fill.Content = ResourceUtils.GetResourceImage("fill.png");
            fill.ToolTip = "Заливка цветов";
            fill.Checked += fill_Checked;
            fill.Unchecked += fill_Unchecked;
            fragment.Add(fill);

            ToggleButton pal = new ToggleButton();
            pal.IsChecked = _pal;
            pal.Content = ResourceUtils.GetResourceImage("useu.png");
            pal.ToolTip = "Европейская / американская палитра";
            pal.Checked += pal_Checked;
            pal.Unchecked += pal_Unchecked;
            fragment.Add(pal);
        }

        public override void RevokeToolbarFragment()
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
    }
}
