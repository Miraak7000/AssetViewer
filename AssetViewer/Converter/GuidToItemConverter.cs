using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using AssetViewer.Data;

namespace AssetViewer.Converter {

  [ValueConversion(typeof(string), typeof(TemplateAsset))]
  public class GuidToItemConverter : IValueConverter {

    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is string poolstring) {
        return poolstring.GetItemsById().FirstOrDefault();
      }
      if (value is string poolint) {
        return poolint.GetItemsById().FirstOrDefault();
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Public Methods
  }
}