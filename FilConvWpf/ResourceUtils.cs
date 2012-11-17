using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    static class ResourceUtils
    {
        public static Image GetResourceImage(string fileName)
        {
            Image result = new Image();
            result.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/" + fileName));
            return result;
        }
    }
}
