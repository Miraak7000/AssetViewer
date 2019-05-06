using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;

namespace AssetViewer.Converter {

  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public class DescriptionConverterOld : IValueConverter {

    #region Public Methods
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value == null) return null;
      if (parameter == null) parameter = "Short";
      switch (App.Language) {
        case Languages.English:
          return ((XElement)value).XPathSelectElement($"EN/{parameter}").Value;
        case Languages.German:
          return ((XElement)value).XPathSelectElement($"DE/{parameter}").Value;
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