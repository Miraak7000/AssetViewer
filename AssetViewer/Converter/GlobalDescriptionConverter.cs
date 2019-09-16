using System;
using System.Globalization;
using System.Windows.Data;

namespace AssetViewer.Converter {

  public class GlobalDescriptionConverter : IValueConverter {

    #region Methods

    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      return App.Descriptions.TryGetValue(parameter.ToString(), out var str) ? str : null;
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Methods
  }
}