using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using FilConv.Presenter;
using FilConv.UI;
using ImageLib;

namespace FilConv.Encode;

public class NativeEncoding : IEncoding
{
    private readonly INativeImageFormat _format;
    private readonly bool _canDither;
    private NamedDisplay? _currentDisplay;
    private NamedPalette? _currentPalette;
    private bool _dither;
    private readonly IMultiChoice<NamedDisplay> _displaySelector;
    private readonly IMultiChoice<NamedPalette> _paletteSelector;
    private readonly IToggle _ditherToggle;
    private readonly ITool[] _tools;

    private int _encodingChangedSemaphore;

    public event EventHandler<EventArgs>? EncodingChanged;

    public NativeEncoding(string name, INativeImageFormat format, bool canDither)
    {
        Name = name;
        _format = format;
        _canDither = canDither;

        _displaySelector = new MultiChoiceBuilder<NamedDisplay>()
            .WithCallback(DisplaySelector_Callback)
            .Build();

        _paletteSelector = new MultiChoiceBuilder<NamedPalette>()
            .WithCallback(PaletteSelector_Callback)
            .Build();

        _ditherToggle = new ToggleBuilder()
            .WithIcon("rainbow.png")
            .WithTooltip("ColorDitherToggleTooltip")
            .WithCallback(DitherToggle_Callback)
            .WithInitialState(_dither)
            .Build();

        _ditherToggle.Element.IsEnabled = canDither;

        _tools = new ITool[] { _displaySelector, _paletteSelector, _ditherToggle };

        UpdateDisplayTool();
    }

    public string Name { get; }

    public IEnumerable<ITool> Tools => _tools.Where(x => x.Element.IsEnabled);

    public AspectBitmapSource Preview(Bitmap original)
    {
        return _format.FromNative(ToNative(original), GetDecodingOptions()).ToAspectBitmapSource();
    }

    public IEnumerable<ISaveDelegate> GetSaveDelegates(Bitmap original)
    {
        return new[] { new FilSaveDelegate(original, _format, GetEncodingOptions()) };
    }

    public void StoreSettings(IDictionary<string, object> settings)
    {
        if (_displaySelector.Element.IsEnabled && _currentDisplay != null)
            settings[EncodingSettingNames.Display] = _currentDisplay;
        if (_paletteSelector.Element.IsEnabled && _currentPalette != null)
            settings[EncodingSettingNames.Palette] = _currentPalette;
        if (_canDither)
            settings[EncodingSettingNames.Dithering] = _dither;
    }

    public void AdoptSettings(IDictionary<string, object> settings)
    {
        _encodingChangedSemaphore++;

        if (_displaySelector.Element.IsEnabled && settings.TryGetValue(EncodingSettingNames.Display, out var d))
        {
            var display = (NamedDisplay?)d;
            if (_displaySelector.Choices.Contains(display))
                _displaySelector.CurrentChoice = display!;
        }

        if (_paletteSelector.Element.IsEnabled && settings.TryGetValue(EncodingSettingNames.Palette, out var p))
        {
            var palette = (NamedPalette?)p;
            if (_paletteSelector.Choices.Contains(palette))
                _paletteSelector.CurrentChoice = palette!;
        }

        if (_canDither && settings.TryGetValue(EncodingSettingNames.Dithering, out var o))
        {
            _dither = (bool)o;
            _ditherToggle.IsChecked = _dither;
        }

        _encodingChangedSemaphore--;
        OnEncodingChanged();
    }

    protected virtual void OnEncodingChanged()
    {
        if (_encodingChangedSemaphore < 0)
            throw new Exception($"{nameof(_encodingChangedSemaphore)} below zero");
        if (_encodingChangedSemaphore > 0)
            return;
        EncodingChanged?.Invoke(this, EventArgs.Empty);
    }

    private NativeImage ToNative(Bitmap original)
    {
        return _format.ToNative(new BitmapPixels(original), GetEncodingOptions());
    }

    private void DisplaySelector_Callback(NamedDisplay display)
    {
        if (display == _currentDisplay)
            return;

        _currentDisplay = display;

        _encodingChangedSemaphore++;

        UpdatePaletteTool();

        _encodingChangedSemaphore--;
        OnEncodingChanged();
    }

    private void PaletteSelector_Callback(NamedPalette palette)
    {
        if (palette == _currentPalette)
            return;

        _currentPalette = palette;
        OnEncodingChanged();
    }

    private void DitherToggle_Callback(bool dither)
    {
        if (dither == _dither)
            return;

        _dither = dither;
        OnEncodingChanged();
    }

    private void UpdateDisplayTool()
    {
        if (_format.SupportedEncodingDisplays == null)
        {
            _displaySelector.Element.IsEnabled = false;
            _paletteSelector.Element.IsEnabled = false;
            return;
        }

        var choices = _displays.Where(x => _format.SupportedEncodingDisplays.Contains(x.Display)).ToList();
        var current = _currentDisplay;
        _displaySelector.Choices = choices;
        _displaySelector.CurrentChoice = choices.Contains(current!) ? current! : choices.First();
        _displaySelector.Element.IsEnabled = true;
    }

    private void UpdatePaletteTool()
    {
        if (_currentDisplay == null)
            throw new Exception("Current display cannot be null here");

        var supportedPalettes = _format.GetSupportedPalettes(_currentDisplay.Display)?.ToList();
        if (supportedPalettes == null)
        {
            _paletteSelector.Element.IsEnabled = false;
            return;
        }

        var choices = _palettes.Where(x => supportedPalettes.Contains(x.Palette)).ToList();
        var current = _currentPalette;
        _paletteSelector.Choices = choices;
        _paletteSelector.CurrentChoice = choices.Contains(current!) ? current! : choices.First();
        _paletteSelector.Element.IsEnabled = true;
    }

    private EncodingOptions GetEncodingOptions() =>
        new EncodingOptions
        {
            Display = _currentDisplay?.Display ?? NativeDisplay.Color,
            Palette = _currentPalette?.Palette ?? NativePalette.Agat1,
            Dither = _dither
        };

    private DecodingOptions GetDecodingOptions() =>
        new DecodingOptions
        {
            Display = _currentDisplay?.Display ?? NativeDisplay.Color,
            Palette = _currentPalette?.Palette ?? NativePalette.Agat1,
        };

    private static readonly NamedDisplay[] _displays =
    {
        new NamedDisplay("DisplayNameColor", NativeDisplay.Color),
        new NamedDisplay("DisplayNameColorFilled", NativeDisplay.ColorFilled),
        new NamedDisplay("DisplayNameColorStriped", NativeDisplay.ColorStriped),
        new NamedDisplay("DisplayNameMono", NativeDisplay.Mono),
        new NamedDisplay("DisplayNameMeta", NativeDisplay.Meta),
    };

    private static readonly NamedPalette[] _palettes =
    {
        new NamedPalette("PaletteNameAgat1", NativePalette.Agat1),
        new NamedPalette("PaletteNameAgat2", NativePalette.Agat2),
        new NamedPalette("PaletteNameAgat3", NativePalette.Agat3),
        new NamedPalette("PaletteNameAgat4", NativePalette.Agat4),
    };
}
