using ImageLib;
using System.Collections.Generic;
using FilConvWpf.UI;

namespace FilConvWpf.Native
{
    public abstract class AppleDisplayModeBase : NativeDisplayMode
    {
        protected bool _ntsc;
        protected readonly IToggle _ntscToggle;

        public AppleDisplayModeBase(string name, INativeImageFormat format)
            : base(name, format)
        {
            _ntscToggle = new ToggleBuilder()
                .WithIcon("television.png")
                .WithTooltip("Apple2NtscModeToggleTooltip")
                .WithCallback(on => { _ntsc = on; UpdateFormat(); })
                .WithInitialState(_ntsc)
                .Build();

            Tools = new ITool[] { _ntscToggle };
        }

        public override void StoreSettings(IDictionary<string, object> settings)
        {
            base.StoreSettings(settings);
            settings[SettingNames.NtscMode] = _ntsc;
        }

        public override void AdoptSettings(IDictionary<string, object> settings)
        {
            base.AdoptSettings(settings);

            if (settings.TryGetValue(SettingNames.NtscMode, out var o))
            {
                _ntsc = (bool)o;
                _ntscToggle.IsChecked = _ntsc;
            }
        }

        protected abstract void UpdateFormat();
    }
}
