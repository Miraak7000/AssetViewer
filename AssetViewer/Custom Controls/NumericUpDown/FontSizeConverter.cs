using System;
using System.Globalization;
using System.Windows.Data;

namespace AssetViewer {

  internal class FontSizeConverter : IValueConverter {

    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      var v = (double)value;
      return v * 0.6;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Public Methods
  }
}