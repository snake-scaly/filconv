using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using FilConv.I18n;

namespace FilConv.UI;

public class ToggleBuilder
{
    private Image? _icon;
    private string? _tooltip;
    private Action<bool>? _onToggle;
    private bool _initialState;

    public ToggleBuilder WithIcon(string resourceName)
    {
        _icon = ResourceUtils.GetResourceImage(resourceName);
        return this;
    }

    public ToggleBuilder WithTooltip(string tooltip)
    {
        _tooltip = tooltip;
        return this;
    }

    public ToggleBuilder WithCallback(Action<bool> onToggle)
    {
        _onToggle = onToggle;
        return this;
    }

    public ToggleBuilder WithInitialState(bool on)
    {
        _initialState = on;
        return this;
    }

    public IToggle Build()
    {
        var toggleButton = new ToggleButton { Content = _icon, IsChecked = _initialState };
        if (_tooltip != null)
            L10n.AddLocalizedProperty(toggleButton, ToolTip.TipProperty, _tooltip).Update();
        return new Toggle(toggleButton, _onToggle);
    }

    private class Toggle : IToggle
    {
        private readonly ToggleButton _button;

        public Toggle(ToggleButton button, Action<bool>? onToggle)
        {
            _button = button;
            if (onToggle != null)
                button.IsCheckedChanged += (_, _) => onToggle(button.IsChecked == true);
        }

        public bool IsChecked
        {
            get => _button.IsChecked ?? false;
            set => _button.IsChecked = value;
        }

        public Control Element => _button;
    }
}
