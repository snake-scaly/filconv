using System.ComponentModel;
using System.Windows.Controls;

namespace FilConvWpf.UI
{
    public class ScaleComboBoxModel : INotifyPropertyChanged
    {
        private int? _percent;
        private ComboBoxItem _selectedItem;

        public event PropertyChangedEventHandler PropertyChanged;

        public int? Percent
        {
            get => _percent;

            set
            {
                if (value == _percent)
                    return;
                _percent = value;
                OnPropertyChanged(nameof(Percent));
                OnPropertyChanged(nameof(Scale));
            }
        }

        public ComboBoxItem SelectedItem
        {
            get => _selectedItem;

            set
            {
                if (value == _selectedItem)
                    return;
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));

                if (_selectedItem != null)
                    Percent = (int?)_selectedItem.Tag;
            }
        }

        public double? Scale => _percent / 100.0;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
