using System.Collections.Generic;
using FilConvWpf.UI;
using ImageLib.Apple;
using ImageLib.Util;

namespace FilConvWpf.Native
{
    class AppleHiResDisplayMode : AppleDisplayModeBase
    {
        private bool _fill;
        private bool _pal;

        private readonly IToggle _fillToggle;
        private readonly IToggle _palToggle;

        public AppleHiResDisplayMode()
            : base("FormatNameApple2HiRes", new Apple2HiResImageFormat(new Apple2SimpleTv(Apple2Palettes.European)))
        {
            _fillToggle = new ToggleBuilder()
                .WithIcon("fill.png")
                .WithTooltip("Apple2ColorFillToggleTooltip")
                .WithCallback( on => { _fill = on; UpdateFormat(); })
                .WithInitialState(_fill)
                .Build();

            _palToggle = new ToggleBuilder()
                .WithIcon("useu.png")
                .WithTooltip("Apple2PaletteToggleTooltip")
                .WithCallback( on => { _pal = on; UpdateFormat(); })
                .WithInitialState(_pal)
                .Build();

            Tools = new ITool[] { _fillToggle, _palToggle, _ntscToggle };
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

            if (settings.TryGetValue(SettingNames.AppleFill, out var o))
            {
                _fill = (bool)o;
                _fillToggle.IsChecked = _fill;
            }

            if (settings.TryGetValue(SettingNames.ApplePalette, out o))
            {
                _pal = (bool)o;
                _palToggle.IsChecked = _pal;
            }
        }

        protected override void UpdateFormat()
        {
            if (_ntsc)
            {
                Format = new Apple2HiResNtscImageFormat();
            }
            else
            {
                Rgb[] pal = _pal ? Apple2Palettes.American : Apple2Palettes.European;
                Apple2TvSet tv = _fill ? (Apple2TvSet)new Apple2FillTv(pal) : (Apple2TvSet)new Apple2SimpleTv(pal);
                Format = new Apple2HiResImageFormat(tv);
            }
            OnFormatChanged();
        }
    }
}
