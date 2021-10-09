using System;
using System.Globalization;
using System.Windows.Data;

namespace AssetViewer.Converter {

  public class GlobalDescriptionConverter : IValueConverter {

    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (parameter is int idint) {
        return AssetProvider.Descriptions.TryGetValue(idint, out var str) ? str : null;
      }
      else if (parameter is string idstring && int.TryParse(idstring, out var id)) {
        return AssetProvider.Descriptions.TryGetValue(id, out var str) ? str : null;
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Public Methods
  }
}