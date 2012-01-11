using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace FilConvWpf
{
    class GrayscaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FormatConvertedBitmap((BitmapSource)value, PixelFormats.Gray8, null, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((FormatConvertedBitmap)value).Source;
        }
    }
}
