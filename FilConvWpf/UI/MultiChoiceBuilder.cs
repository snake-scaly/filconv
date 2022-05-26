using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FilConvWpf.I18n;

namespace FilConvWpf.UI
{
    public class MultiChoiceBuilder<TChoice>
    {
        private List<ComboBoxItem> _items;
        private ComboBoxItem _defaultItem;
        private Action<TChoice> _onChoice;
        private string _title;
        private string _description;

        public MultiChoiceBuilder<TChoice> WithChoices(
            IEnumerable<TChoice> choices,
            Func<TChoice, string> nameResolver = null)
        {
            _items = choices
                .Select(c =>
                {
                    var comboBoxItem = new ComboBoxItem { Tag = c };
                    var choiceName = nameResolver?.Invoke(c) ?? c.ToString();
                    L10n.AddLocalizedProperty(comboBoxItem, ContentControl.ContentProperty, choiceName).Update();
                    return comboBoxItem;
                })
                .ToList();

            return this;
        }

        public MultiChoiceBuilder<TChoice> WithDefaultChoice(TChoice choice)
        {
            _defaultItem = _items.FirstOrDefault(i => Equals(i.Tag, choice)) ??
                throw new InvalidOperationException("Default choice is not among choices");
            return this;
        }

        public MultiChoiceBuilder<TChoice> WithCallback(Action<TChoice> onChoice)
        {
            _onChoice = onChoice;
            return this;
        }

        public MultiChoiceBuilder<TChoice> WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public MultiChoiceBuilder<TChoice> WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public IMultiChoice<TChoice> Build()
        {
            if (_items == null)
                throw new InvalidOperationException("Choices are not set");

            var comboBox = new ComboBox();
            foreach (var item in _items)
                comboBox.Items.Add(item);
            comboBox.SelectedItem = _defaultItem;
            comboBox.Style = (Style)Application.Current.FindResource(ToolBar.ComboBoxStyleKey);

            if (_onChoice != null)
            {
                var onChoice = _onChoice;
                comboBox.SelectionChanged += (s, e) =>
                {
                    var selectedItem = e.AddedItems.Cast<ComboBoxItem>().FirstOrDefault();
                    onChoice((TChoice)selectedItem?.Tag);
                };
            }

            FrameworkElement element = comboBox;

            if (_title != null)
            {
                var title = new Label { Target = comboBox };
                L10n.AddLocalizedProperty(title, ContentControl.ContentProperty, _title).Update();
                var stackPanel = new StackPanel
                {
                    Children = { title, comboBox },
                    Orientation = Orientation.Horizontal
                };
                element = stackPanel;
            }

            if (_description != null)
                L10n.AddLocalizedProperty(element, FrameworkElement.ToolTipProperty, _description).Update();

            return new MultiChoice(element, comboBox);
        }

        private class MultiChoice : IMultiChoice<TChoice>
        {
            private readonly ComboBox _comboBox;

            public MultiChoice(FrameworkElement element, ComboBox comboBox)
            {
                Element = element;
                _comboBox = comboBox;
            }

            public FrameworkElement Element { get; }

            public TChoice CurrentChoice
            {
                get => (TChoice)((ComboBoxItem)_comboBox.SelectedItem)?.Tag;

                set
                {
                    var item = _comboBox.Items
                        .Cast<ComboBoxItem>()
                        .FirstOrDefault(i => Equals(i.Tag, value));
                    _comboBox.SelectedItem = item;
                }
            }
        }
    }
}
