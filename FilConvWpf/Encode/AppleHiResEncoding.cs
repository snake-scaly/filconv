using FilConvWpf.I18n;
using ImageLib;
using ImageLib.Apple;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FilConvWpf.Encode
{
    class AppleHiResEncoding : IEncoding
    {
        private bool _fill;
        private bool _pal;

        private ToggleButton _fillButton;
        private ToggleButton _palButton;

        private Apple2HiResImageFormat _format;

        public AppleHiResEncoding()
        {
            ToolBar = new ToolBar();

            Label toolbarTitle = new Label();
            L10n.AddLocalizedProperty(toolbarTitle, Label.ContentProperty, "FormatNameApple2").Update();

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

            ToolBar.Items.Add(toolbarTitle);
            ToolBar.Items.Add(_fillButton);
            ToolBar.Items.Add(_palButton);
            _format = new Apple2HiResImageFormat(new Apple2SimpleTv(Apple2Palettes.European));
        }

        public event EventHandler<EventArgs> EncodingChanged;

        public string Name
        {
            get { return "FormatNameApple2"; }
        }

        public System.Windows.Controls.ToolBar ToolBar { get; private set; }

        public AspectBitmap Preview(BitmapSource original)
        {
            return new AspectBitmap(
                _format.FromNative(_format.ToNative(original, new EncodingOptions())),
                _format.Aspect);
        }

        public IEnumerable<ISaveDelegate> GetSaveDelegates(BitmapSource original)
        {
            yield return new FilSaveDelegate(original, _format, new EncodingOptions());
        }

        public void StoreSettings(IDictionary<string, object> settings)
        {
            settings[SettingNames.AppleFill] = _fill;
            settings[SettingNames.ApplePalette] = _pal;
        }

        public void AdoptSettings(IDictionary<string, object> settings)
        {
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

        protected virtual void OnEncodingChanged()
        {
            if (EncodingChanged != null)
            {
                EncodingChanged(this, EventArgs.Empty);
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

        private void UpdateFormat()
        {
            Color[] pal = _pal ? Apple2Palettes.American : Apple2Palettes.European;
            Apple2TvSet tv = _fill ? (Apple2TvSet)new Apple2FillTv(pal) : (Apple2TvSet)new Apple2SimpleTv(pal);
            _format = new Apple2HiResImageFormat(tv);
            OnEncodingChanged();
        }
    }
}
