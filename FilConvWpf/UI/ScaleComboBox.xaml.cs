using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FilConvWpf.UI
{
    public partial class ScaleComboBox : UserControl
    {
        private static readonly DependencyPropertyKey ScaleKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Scale), typeof(double?), typeof(ScaleComboBox), new PropertyMetadata(null));
        public static readonly DependencyProperty ScaleProperty = ScaleKey.DependencyProperty;

        private readonly ScaleComboBoxModel _model = new ScaleComboBoxModel();

        public ScaleComboBox()
        {
            _model.PropertyChanged += ModelChangedEventHandler;
            Scale = _model.Scale;
            InitializeComponent();
            _model.SelectedItem = FitWindowItem;
            ComboBox.DataContext = _model;
        }

        public event EventHandler<EventArgs> ScaleChanged;

        public double? Scale
        {
            get => (double?)GetValue(ScaleProperty);

            set
            {
                SetValue(ScaleKey, value);
                OnScaleChanged();
            }
        }

        protected virtual void OnScaleChanged()
        {
            ScaleChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ComboBoxKeyDownEventHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (!TryParsePercent(ComboBox.Text, out var percent))
                return;

            if (percent == null)
            {
                ComboBox.SelectedItem = FitWindowItem;
            }
            else
            {
                ComboBox.SelectedItem = null;
                ComboBox.Text = $"{percent}%";
                _model.Percent = percent;
            }
        }

        private void ModelChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScaleComboBoxModel.Scale))
                Scale = _model.Scale;
        }

        private static bool TryParsePercent(string value, out int? percent)
        {
            percent = null;

            if (value == null)
                return true;

            var digits = string.Concat(value.Where(char.IsDigit));
            if (!int.TryParse(digits, out var parsed) || !IsPercentWithinValidRange(parsed))
                return false;

            percent = parsed;
            return true;
        }

        private static bool IsPercentWithinValidRange(int parsed)
        {
            return parsed >= 1 && parsed <= 5000;
        }
    }
}
