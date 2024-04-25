using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using FilConv.I18n;

namespace FilConv.UI;

public class MultiChoiceBuilder<TChoice>
{
    private List<ComboBoxItem>? _items;
    private Func<TChoice, string>? _nameResolver;
    private ComboBoxItem? _defaultItem;
    private Action<TChoice>? _onChoice;
    private string? _title;
    private string? _description;

    public MultiChoiceBuilder<TChoice> WithChoices(
        IEnumerable<TChoice> choices,
        Func<TChoice, string>? nameResolver = null)
    {
        _nameResolver = nameResolver;
        _items = choices
            .Select(c => ChoiceToItem(nameResolver, c))
            .ToList();
        return this;
    }

    public MultiChoiceBuilder<TChoice> WithDefaultChoice(TChoice choice)
    {
        ArgumentNullException.ThrowIfNull(choice);
        if (_items == null)
            throw new InvalidOperationException("Must add choices before setting default");
        _defaultItem = _items?.FirstOrDefault(i => Equals(i.Tag, choice)) ??
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
        var comboBox = new ComboBox();
        if (_items != null)
        {
            foreach (var item in _items)
            {
                comboBox.Items.Add(item);
            }
        }
        comboBox.SelectedItem = _defaultItem;

        if (_onChoice != null)
        {
            var onChoice = _onChoice;
            comboBox.SelectionChanged += (_, e) =>
            {
                var selectedItem = e.AddedItems.Cast<ComboBoxItem>().FirstOrDefault();
                if (selectedItem == null) return;
                var tag = selectedItem.Tag ?? throw new Exception("Unexpected null Tag");
                onChoice((TChoice)tag);
            };
        }

        Control element = comboBox;

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
            L10n.AddLocalizedProperty(element, ToolTip.TipProperty, _description).Update();

        return new MultiChoice(element, comboBox, _nameResolver);
    }

    private static ComboBoxItem ChoiceToItem(Func<TChoice, string>? nameResolver, TChoice choice)
    {
        ArgumentNullException.ThrowIfNull(choice);
        var comboBoxItem = new ComboBoxItem { Tag = choice };
        var choiceName = nameResolver?.Invoke(choice) ?? choice.ToString() ?? "<null>";
        L10n.AddLocalizedProperty(comboBoxItem, ContentControl.ContentProperty, choiceName).Update();
        return comboBoxItem;
    }

    private class MultiChoice : IMultiChoice<TChoice>
    {
        private readonly ComboBox _comboBox;
        private readonly Func<TChoice, string>? _nameResolver;

        public MultiChoice(Control element, ComboBox comboBox, Func<TChoice, string>? nameResolver)
        {
            Element = element;
            _comboBox = comboBox;
            _nameResolver = nameResolver;
        }

        public Control Element { get; }

        public IEnumerable<TChoice> Choices
        {
            get => _comboBox.Items.Cast<ComboBoxItem>().Select(i => (TChoice)i.Tag!);

            set
            {
                _comboBox.Items.Clear();
                foreach (var c in value)
                {
                    _comboBox.Items.Add(ChoiceToItem(_nameResolver, c));
                }
            }
        }

        public TChoice CurrentChoice
        {
            get
            {
                object selected = _comboBox.SelectedItem ?? throw new Exception("No selected item");
                object tag = ((ComboBoxItem)selected).Tag ?? throw new Exception("Tag is null");
                return (TChoice)tag;
            }

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
