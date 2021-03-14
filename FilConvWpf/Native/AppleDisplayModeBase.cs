using FilConvWpf.I18n;
using ImageLib;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FilConvWpf.Native
{
    abstract class AppleDisplayModeBase : NativeDisplayMode
    {
        protected bool _ntsc;

        private ToggleButton _ntscButton;

        public AppleDisplayModeBase(string name, INativeImageFormat format)
            : base(name, format)
        {
            BuildToolBar();
        }

        protected virtual void BuildToolBar()
        {
            _ntscButton = new ToggleButton();
            _ntscButton.IsChecked = _ntsc;
            _ntscButton.Content = ResourceUtils.GetResourceImage("television.png");
            L10n.AddLocalizedProperty(_ntscButton, ToggleButton.ToolTipProperty, "Apple2NtscModeToggleTooltip").Update();
            _ntscButton.Checked += ntsc_Checked;
            _ntscButton.Unchecked += ntsc_Unchecked;

            CreateToolBarOnce("Apple2ToolBarTitle");
            ToolBar.Items.Add(_ntscButton);
        }

        public override void StoreSettings(IDictionary<string, object> settings)
        {
            base.StoreSettings(settings);
            settings[SettingNames.NtscMode] = _ntsc;
        }

        public override void AdoptSettings(IDictionary<string, object> settings)
        {
            base.AdoptSettings(settings);

            object o;

            if (settings.TryGetValue(SettingNames.NtscMode, out o))
            {
                _ntsc = (bool)o;
                _ntscButton.IsChecked = _ntsc;
            }
        }

        protected virtual void ntsc_Checked(object sender, RoutedEventArgs e)
        {
            _ntsc = true;
            UpdateFormat();
        }

        protected virtual void ntsc_Unchecked(object sender, RoutedEventArgs e)
        {
            _ntsc = false;
            UpdateFormat();
        }

        protected abstract void UpdateFormat();
    }
}
