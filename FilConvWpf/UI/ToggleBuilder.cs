using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using FilConvWpf.I18n;

namespace FilConvWpf.UI
{
    public class ToggleBuilder
    {
        private Image _icon;
        private string _tooltip;
        private Action<bool> _onToggle;
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
            L10n.AddLocalizedProperty(toggleButton, FrameworkElement.ToolTipProperty, _tooltip).Update();
            return new Toggle(toggleButton, _onToggle);
        }

        private class Toggle : IToggle
        {
            private readonly ToggleButton _button;

            public Toggle(ToggleButton button, Action<bool> onToggle)
            {
                _button = button;
                button.Checked += (s, e) => onToggle?.Invoke(true);
                button.Unchecked += (s, e) => onToggle?.Invoke(false);
            }

            public bool IsChecked
            {
                get => _button.IsChecked ?? false;
                set => _button.IsChecked = value;
            }

            public FrameworkElement Element => _button;
        }
    }
}
