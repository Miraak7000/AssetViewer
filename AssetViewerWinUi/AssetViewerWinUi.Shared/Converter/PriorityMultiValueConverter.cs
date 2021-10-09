using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AssetViewer.Converter {

  public class PriorityMultiValueConverter : IMultiValueConverter {

    #region Public Methods

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
      if (Array.Find(values, o => o != null && o != DependencyProperty.UnsetValue) is string path) {
        return new BitmapImage(new Uri(path, UriKind.Relative));
      }
      return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Public Methods

    #region Public Fields

    public readonly static ImageSourceConverter imageConverter = new ImageSourceConverter();

    #endregion Public Fields
  }
}