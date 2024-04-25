using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Media.Imaging;
using FilConv.Presenter;
using FilConv.UI;

namespace FilConv.Encode;

public sealed class EncodingImagePresenter : IImagePresenter
{
    private const string _modeSettingsKey = "encodingMode";

    private readonly IOriginal _original;
    private IEncoding _currentEncoding;
    private Dictionary<string, object> _settings;
    private List<IEncoding> _encodings;
    private IMultiChoice<IEncoding> _encodingSelector;

    public event EventHandler<EventArgs>? DisplayImageChanged;
    public event EventHandler<EventArgs>? ToolBarChanged;

    public EncodingImagePresenter(IOriginal original)
    {
        _original = original;
        _original.OriginalChanged += original_OriginalChanged;

        _settings = new Dictionary<string, object>();
        UpdateEncodings();
    }

    public void Dispose()
    {
        _original.OriginalChanged -= original_OriginalChanged;
        _currentEncoding.EncodingChanged -= currentEncoding_EncodingChanged;
    }

    public AspectBitmapSource? DisplayImage { get; private set; }

    public IEnumerable<ITool> Tools { get; private set; } = [];

    public IEnumerable<ISaveDelegate> SaveDelegates
    {
        get
        {
            IEnumerable<ISaveDelegate> delegates = [];
            if (_original.OriginalBitmap != null)
                delegates = delegates.Concat(_currentEncoding.GetSaveDelegates(_original.OriginalBitmap));
            if (DisplayImage != null)
                delegates = delegates.Concat(GetStandardSaveDelegates(DisplayImage.Bitmap));
            return delegates;
        }
    }

    [MemberNotNull(nameof(_currentEncoding))]
    private void SetCurrentEncoding(IEncoding newEncoding)
    {
        if (newEncoding == _currentEncoding)
        {
            return;
        }

        if (_currentEncoding != null)
        {
            _currentEncoding.StoreSettings(_settings);
            _currentEncoding.EncodingChanged -= currentEncoding_EncodingChanged;
        }

        _currentEncoding = newEncoding;
        _currentEncoding.AdoptSettings(_settings);
        _currentEncoding.EncodingChanged += currentEncoding_EncodingChanged;

        _encodingSelector.CurrentChoice = _currentEncoding;

        UpdateTools();
        Encode();
    }

    private void currentEncoding_EncodingChanged(object? sender, EventArgs e)
    {
        UpdateTools();
        Encode();
    }

    private void original_OriginalChanged(object? sender, EventArgs e)
    {
        UpdateEncodings();
        Encode();
    }

    [MemberNotNull(nameof(_encodings), nameof(_currentEncoding), nameof(_encodingSelector))]
    private void UpdateEncodings()
    {
        _encodings = EncodingResolver.GetPossibleEncodings(_original).ToList();

        var newEncoding = GetCompatibleEncoding(_currentEncoding);

        var encodingSelector = CreateEncodingSelector(newEncoding);
        _encodingSelector = encodingSelector;

        SetCurrentEncoding(newEncoding);
    }

    private IMultiChoice<IEncoding> CreateEncodingSelector(IEncoding newEncoding)
    {
        return new MultiChoiceBuilder<IEncoding>()
            .WithChoices(_encodings, e => e.Name)
            .WithDefaultChoice(newEncoding)
            .WithCallback(SetCurrentEncoding)
            .Build();
    }

    private IEncoding GetCompatibleEncoding(IEncoding? desiredEncoding)
    {
        return desiredEncoding != null && _encodings.Contains(desiredEncoding)
            ? desiredEncoding
            : _encodings.First();
    }

    private void Encode()
    {
        if (_original.OriginalBitmap != null)
        {
            DisplayImage = _currentEncoding.Preview(_original.OriginalBitmap);
        }
        else
        {
            DisplayImage = null;
        }
        OnDisplayImageChanged();
    }

    private void OnDisplayImageChanged()
    {
        DisplayImageChanged?.Invoke(this, EventArgs.Empty);
    }

    private static IEnumerable<ISaveDelegate> GetStandardSaveDelegates(Bitmap bitmap)
    {
        yield return new GdiSaveDelegate(bitmap, "FileFormatNamePng", ["*.png"]);
        yield return new GdiSaveDelegate(bitmap, "FileFormatNameJpeg", ["*.jpg", "*,jpeg"]);
        yield return new GdiSaveDelegate(bitmap, "FileFormatNameGif", ["*.gif"]);
        yield return new GdiSaveDelegate(bitmap, "FileFormatNameBmp", ["*.bmp"]);
        yield return new GdiSaveDelegate(bitmap, "FileFormatNameTiff", ["*.tif", "*.tiff"]);
    }

    public void StoreSettings(IDictionary<string, object> settings)
    {
        _currentEncoding.StoreSettings(settings);
        settings[_modeSettingsKey] = _currentEncoding;
    }

    public void AdoptSettings(IDictionary<string, object> settings)
    {
        _settings = new Dictionary<string, object>(settings);
        if (settings.TryGetValue(_modeSettingsKey, out var setting))
        {
            var newEncoding = (IEncoding)setting;
            SetCurrentEncoding(GetCompatibleEncoding(newEncoding));
        }
    }

    private void UpdateTools()
    {
        Tools = new[] { _encodingSelector }.Concat(_currentEncoding.Tools);
        ToolBarChanged?.Invoke(this, EventArgs.Empty);
    }
}
