using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AssetViewer.Converter {

  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public class VisibilityConverter : IValueConverter {

    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is IEnumerable items) {
        foreach (var _ in items) {
          return Visibility.Visible;
        }
        return Visibility.Collapsed;
      }
      return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Public Methods
  }
}