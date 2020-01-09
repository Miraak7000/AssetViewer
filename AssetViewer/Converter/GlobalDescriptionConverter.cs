using System;
using System.Globalization;
using System.Windows.Data;

namespace AssetViewer.Converter {

  public class GlobalDescriptionConverter : IValueConverter {

    #region Methods

    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (parameter is int idint) {
        return App.Descriptions.TryGetValue(idint, out var str) ? str : null;
      }
      else if (parameter is string idstring && int.TryParse(idstring, out var id)) {
        return App.Descriptions.TryGetValue(id, out var str) ? str : null;
      }
      return null;
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Methods
  }
}