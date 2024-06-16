using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using FilConv.Presenter;
using FilConv.UI;
using ImageLib;
using ImageLib.Agat;
using ImageLib.Apple;
using ImageLib.Spectrum;

namespace FilConv.Native;

public sealed class NativeImagePresenter : IImagePresenter, IOriginal
{
    private const string _modeSettingsKey = "previewMode";
    private const string _displaySettingsKey = "display";
    private const string _paletteSettingsKey = "palette";

    private readonly IMultiChoice<NamedMode> _modeSelector;
    private readonly IMultiChoice<NamedDisplay> _displaySelector;
    private readonly IMultiChoice<NamedPalette> _paletteSelector;
    private readonly ITool[] _tools;

    private NamedMode _currentMode;
    private NamedDisplay? _currentDisplay;
    private NamedPalette? _currentPalette;

    private int _updateDisplayImageSemaphore;
    private int _toolBarChangedSemaphore;

    public event EventHandler<EventArgs>? DisplayImageChanged;
    public event EventHandler<EventArgs>? ToolBarChanged;
    public event EventHandler<EventArgs>? OriginalChanged;

    public NativeImagePresenter(NativeImage nativeImage)
    {
        NativeImage = nativeImage;

        _modeSelector = new MultiChoiceBuilder<NamedMode>()
            .WithChoices(_modes, m => m.Name)
            .WithCallback(ModeSelector_Callback)
            .Build();

        _displaySelector = new MultiChoiceBuilder<NamedDisplay>()
            .WithCallback(DisplaySelector_Callback)
            .Build();

        _paletteSelector = new MultiChoiceBuilder<NamedPalette>()
            .WithCallback(PaletteSelector_Callback)
            .Build();

        _tools = new ITool[] { _modeSelector, _displaySelector, _paletteSelector };

        // setting _modeSelector.CurrentChoice will set _currentMode to non-null before the constructor finishes
        _currentMode = null!;
        _modeSelector.CurrentChoice = GuessPreviewMode(null);
    }

    public void Dispose()
    {
    }

    public AspectBitmapSource? DisplayImage { get; private set; }

    public Bitmap? OriginalBitmap => DisplayImage?.Bitmap;

    public NativeImage NativeImage { get; }

    public IEnumerable<ITool> Tools => _tools.Where(x => x.Element.IsEnabled);

    public void StoreSettings(IDictionary<string, object> settings)
    {
        settings[_modeSettingsKey] = _currentMode;
        if (_currentMode.Format.SupportedDisplays != null && _currentDisplay != null)
        {
            settings[_displaySettingsKey] = _currentDisplay;
            if (_currentMode.Format.GetSupportedPalettes(_currentDisplay.Display) != null && _currentPalette != null)
                settings[_paletteSettingsKey] = _currentPalette;
        }
    }

    public void AdoptSettings(IDictionary<string, object> settings)
    {
        if (NativeImage.Metadata != null)
        {
            return;
        }

        _updateDisplayImageSemaphore++;
        _toolBarChangedSemaphore++;

        if (settings.TryGetValue(_modeSettingsKey, out var m))
        {
            _modeSelector.CurrentChoice = GuessPreviewMode((NamedMode)m);
        }

        var supportedDisplays = GetSupportedNamedDisplays(_currentMode.Format);
        if (supportedDisplays != null && settings.TryGetValue(_displaySettingsKey, out var d))
        {
            var displaySettings = (NamedDisplay)d;
            var display = ChooseCompatibleDisplay(supportedDisplays, displaySettings);
            _displaySelector.CurrentChoice = display;
        }

        if (_currentDisplay != null)
        {
            var supportedPalettes = GetSupportedNamedPalettes(_currentDisplay.Display);

            if (supportedPalettes != null && settings.TryGetValue(_paletteSettingsKey, out var p))
            {
                var savedPalette = (NamedPalette)p;
                _paletteSelector.CurrentChoice = GetValidPalette(supportedPalettes, savedPalette);
            }
        }

        _toolBarChangedSemaphore--;
        OnToolBarChanged();

        _updateDisplayImageSemaphore--;
        UpdateDisplayImage();
    }

    private void ModeSelector_Callback(NamedMode mode)
    {
        if (mode == _currentMode)
            return;

        _currentMode = mode;

        _updateDisplayImageSemaphore++;
        _toolBarChangedSemaphore++;

        UpdateDisplayTool();

        _toolBarChangedSemaphore--;
        OnToolBarChanged();

        _updateDisplayImageSemaphore--;
        UpdateDisplayImage();
    }

    private void DisplaySelector_Callback(NamedDisplay display)
    {
        if (display == _currentDisplay)
            return;

        _currentDisplay = display;

        _updateDisplayImageSemaphore++;
        _toolBarChangedSemaphore++;

        UpdatePaletteTool();

        _toolBarChangedSemaphore--;
        OnToolBarChanged();

        _updateDisplayImageSemaphore--;
        UpdateDisplayImage();
    }

    private void PaletteSelector_Callback(NamedPalette palette)
    {
        if (palette == _currentPalette)
            return;

        _currentPalette = palette;
        UpdateDisplayImage();
    }

    private void UpdateDisplayTool()
    {
        var supportedDisplays = GetSupportedNamedDisplays(_currentMode.Format);

        if (supportedDisplays == null)
        {
            _displaySelector.Element.IsEnabled = false;
            _paletteSelector.Element.IsEnabled = false;
            return;
        }

        _displaySelector.Choices = supportedDisplays;
        _displaySelector.CurrentChoice = ChooseCompatibleDisplay(supportedDisplays, _currentDisplay);
        _displaySelector.Element.IsEnabled = true;
    }

    private void UpdatePaletteTool()
    {
        if (_currentDisplay == null)
        {
            _paletteSelector.Element.IsEnabled = false;
            return;
        }

        var supportedPalettes = GetSupportedNamedPalettes(_currentDisplay.Display);

        if (supportedPalettes == null)
        {
            _paletteSelector.Element.IsEnabled = false;
            return;
        }

        _paletteSelector.Choices = supportedPalettes;
        _paletteSelector.CurrentChoice = GetValidPalette(supportedPalettes, _currentPalette);
        _paletteSelector.Element.IsEnabled = true;
    }

    private void UpdateDisplayImage()
    {
        if (_updateDisplayImageSemaphore < 0)
            throw new Exception($"{nameof(_updateDisplayImageSemaphore)} below zero");
        if (_updateDisplayImageSemaphore > 0)
            return;

        var options = new DecodingOptions
        {
            Display = _currentDisplay?.Display ?? NativeDisplay.Color,
            Palette = _currentPalette?.Palette ?? NativePalette.Default,
        };

        DisplayImage = _currentMode.Format.FromNative(NativeImage, options).ToAspectBitmapSource();
        OnDisplayImageChanged();
    }

    private void OnDisplayImageChanged()
    {
        DisplayImageChanged?.Invoke(this, EventArgs.Empty);
        OriginalChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnToolBarChanged()
    {
        if (_toolBarChangedSemaphore < 0)
            throw new Exception($"{nameof(_toolBarChangedSemaphore)} below zero");
        if (_toolBarChangedSemaphore > 0)
            return;
        ToolBarChanged?.Invoke(this, EventArgs.Empty);
    }

    private NamedMode GuessPreviewMode(NamedMode? preferred)
    {
        // This relies on OrderByDescending performing a stable sort. Therefore if
        // preferred is among the best it will come out first.
        NamedMode[] head = preferred != null ? [preferred] : [];
        return head.Concat(_modes)
            .OrderByDescending(f => f.Format.ComputeMatchScore(NativeImage))
            .First();
    }

    private NamedDisplay ChooseCompatibleDisplay(List<NamedDisplay> supportedDisplays, NamedDisplay? preferredDisplay)
    {
        if (preferredDisplay != null && supportedDisplays.Contains(preferredDisplay))
            return preferredDisplay;
        var defaultDisplay = _currentMode.Format.GetDefaultDecodingOptions(NativeImage).Display;
        return supportedDisplays.FirstOrDefault(d => d.Display == defaultDisplay) ?? supportedDisplays.First();
    }

    private List<NamedPalette>? GetSupportedNamedPalettes(NativeDisplay display)
    {
        return _currentMode.Format.GetSupportedPalettes(display)?
            .Select(p => _palettes.First(s => s.Palette == p))
            .ToList();
    }

    private NamedPalette GetValidPalette(List<NamedPalette> supportedPalettes, NamedPalette? preferredPalette)
    {
        if (preferredPalette != null && supportedPalettes.Contains(preferredPalette))
            return preferredPalette;
        var defaultNativePalette = _currentMode.Format.GetDefaultDecodingOptions(NativeImage).Palette;
        return supportedPalettes.FirstOrDefault(x => x.Palette == defaultNativePalette, supportedPalettes.First());
    }

    private List<NamedDisplay>? GetSupportedNamedDisplays(INativeImageFormat format)
    {
        var supportedDisplays = format.SupportedDisplays?
            .Select(s => _displays.First(d => d.Display == s));

        if (NativeImage.Metadata == null)
        {
            supportedDisplays = supportedDisplays?.Where(s => s.Display != NativeDisplay.Meta);
        }

        return supportedDisplays?.ToList();
    }

    private static readonly NamedMode[] _modes =
    {
        new("FormatNameAgatCGNR", new AgatCGNRImageFormat()),
        new("FormatNameAgatCGSR", new AgatCGSRImageFormat()),
        new("FormatNameAgatMGVR", new AgatMGVRImageFormat()),
        new("FormatNameAgatCGVR", new AgatCGVRImageFormat()),
        new("FormatNameAgatMGDP", new AgatMGDPImageFormat()),
        new("FormatNameAgatApple", new AgatAppleImageFormat()),
        new("FormatNameAgatCGSRDV", new AgatCGSRDVImageFormat()),
        new("FormatNameAgat7Charset", new AgatCharsetImageFormat(agat9: false)),
        new("FormatNameAgat9Charset", new AgatCharsetImageFormat(agat9: true)),
        new("FormatNameApple2LoRes", new Apple2LoResImageFormat(false)),
        new("FormatNameApple2DoubleLoRes", new Apple2LoResImageFormat(true)),
        new("FormatNameApple2HiRes", new Apple2HiResImageFormat()),
        new("FormatNameApple2DoubleHiRes", new Apple2DoubleHiResImageFormat()),
        new("FormatNameSpectrum", new SpectrumImageFormatInterleave()),
    };

    private static readonly NamedDisplay[] _displays =
    {
        new("DisplayNameColor", NativeDisplay.Color),
        new("DisplayNameColorFilled", NativeDisplay.ColorFilled),
        new("DisplayNameColorStriped", NativeDisplay.ColorStriped),
        new("DisplayNameMono", NativeDisplay.Mono),
        new("DisplayNameMeta", NativeDisplay.Meta),
        new("DisplayNameArtifact", NativeDisplay.Artifact),
    };

    private static readonly NamedPalette[] _palettes =
    {
        new("PaletteNameAgat1", NativePalette.Agat1),
        new("PaletteNameAgat2", NativePalette.Agat2),
        new("PaletteNameAgat3", NativePalette.Agat3),
        new("PaletteNameAgat4", NativePalette.Agat4),
    };

    private class NamedMode : NamedChoice
    {
        public readonly INativeImageFormat Format;

        public NamedMode(string name, INativeImageFormat format) : base(name)
        {
            Format = format;
        }
    }
}
