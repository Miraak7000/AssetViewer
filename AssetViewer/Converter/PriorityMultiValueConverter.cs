using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AssetViewer.Converter {

  public class PriorityMultiValueConverter : IMultiValueConverter {

    #region Methods
    public static ImageSourceConverter imageConverter = new ImageSourceConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
      if (Array.Find(values, o => o != null && o != DependencyProperty.UnsetValue) is string path) {
        return new BitmapImage(new Uri(path, UriKind.Relative));
      }
      return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Methods
  }
}