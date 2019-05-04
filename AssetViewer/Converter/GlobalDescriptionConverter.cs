using System;
using System.Globalization;
using System.Windows.Data;
using AssetViewer.Library;

namespace AssetViewer.Converter {

  public class GlobalDescriptionConverter : IValueConverter {

    #region Public Methods
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      var key = Int32.Parse(parameter.ToString());
      switch (App.Language) {
        case Languages.English:
          return App.Descriptions[key].ShortEN;
        case Languages.German:
          return App.Descriptions[key].ShortDE;
        default:
          throw new NotImplementedException();
      }
    }
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
    #endregion

  }

}