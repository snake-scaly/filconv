using FilConvWpf.I18n;
using ImageLib.Apple;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace FilConvWpf.Native
{
    class AppleHiResDisplayMode : AppleDisplayModeBase
    {
        private bool _fill;
        private bool _pal;

        private ToggleButton _fillButton;
        private ToggleButton _palButton;

        public AppleHiResDisplayMode()
            : base("FormatNameApple2", new Apple2HiResImageFormat(new Apple2SimpleTv(Apple2Palettes.European)))
        {
        }

        protected override void BuildToolBar()
        {
            _fillButton = new ToggleButton();
            _fillButton.IsChecked = _fill;
            _fillButton.Content = ResourceUtils.GetResourceImage("fill.png");
            L10n.AddLocalizedProperty(_fillButton, ToggleButton.ToolTipProperty, "Apple2ColorFillToggleTooltip").Update();
            _fillButton.Checked += fill_Checked;
            _fillButton.Unchecked += fill_Unchecked;

            _palButton = new ToggleButton();
            _palButton.IsChecked = _pal;
            _palButton.Content = ResourceUtils.GetResourceImage("useu.png");
            L10n.AddLocalizedProperty(_palButton, ToggleButton.ToolTipProperty, "Apple2PaletteToggleTooltip").Update();
            _palButton.Checked += pal_Checked;
            _palButton.Unchecked += pal_Unchecked;

            CreateToolBarOnce("Apple2ToolBarTitle");
            ToolBar.Items.Add(_fillButton);
            ToolBar.Items.Add(_palButton);

            base.BuildToolBar();
        }

        public override void StoreSettings(IDictionary<string, object> settings)
        {
            base.StoreSettings(settings);
            settings[SettingNames.AppleFill] = _fill;
            settings[SettingNames.ApplePalette] = _pal;
        }

        public override void AdoptSettings(IDictionary<string, object> settings)
        {
            base.AdoptSettings(settings);

            object o;

            if (settings.TryGetValue(SettingNames.AppleFill, out o))
            {
                _fill = (bool)o;
                _fillButton.IsChecked = _fill;
            }

            if (settings.TryGetValue(SettingNames.ApplePalette, out o))
            {
                _pal = (bool)o;
                _palButton.IsChecked = _pal;
            }
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

        protected override void UpdateFormat()
        {
            if (_ntsc)
            {
                Format = new Apple2HiResNtscImageFormat();
            }
            else
            {
                Color[] pal = _pal ? Apple2Palettes.American : Apple2Palettes.European;
                Apple2TvSet tv = _fill ? (Apple2TvSet)new Apple2FillTv(pal) : (Apple2TvSet)new Apple2SimpleTv(pal);
                Format = new Apple2HiResImageFormat(tv);
            }
            OnFormatChanged();
        }
    }
}
