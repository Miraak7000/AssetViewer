using AssetViewer.Data;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

namespace AssetViewer.Converter {

  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public class DescriptionConverter : IValueConverter {

    #region Methods

    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      var description = value as Description;
      if (description == null)
        return String.Empty;
      switch (App.Language) {
        case AssetViewer.Library.Languages.German:
          return description.DE;

        default:
          return description.EN;
      }
    }
    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Methods
  }
}