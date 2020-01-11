using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AssetViewer.Converter {

  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public class VisibilityConverter : IValueConverter {

    #region Methods

    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
      if (value is IEnumerable items) {
        foreach (var item in items) {
          return Visibility.Visible;
        }
        return Visibility.Collapsed;
      }
      return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Methods
  }
}