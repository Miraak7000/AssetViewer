using System;
using System.Globalization;
using System.Windows.Data;
using AssetViewer.Library;

namespace AssetViewer.Converter {

  public class GlobalTranslationConverter : IValueConverter {

    #region Public Methods
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value is string key && !string.IsNullOrWhiteSpace(key)) {
        return App.GetTranslation(key);
      }
      return null;
    }
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
    #endregion

  }

}