using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using AssetViewer.Data;

namespace AssetViewer.Converter {

  [ValueConversion(typeof(IEnumerable<RewardsItem>), typeof(IEnumerable<TemplateAsset>))]
  [ValueConversion(typeof(IEnumerable<string>), typeof(IEnumerable<TemplateAsset>))]
  [ValueConversion(typeof(RewardsItem), typeof(IEnumerable<TemplateAsset>))]
  public class RewardToItemConverter : IValueConverter {

    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is RewardsItem item) {
        return item.ID.GetItemsById().OrderBy(l => l.Text.CurrentLang);
      }
      else if (value is IEnumerable<RewardsItem> rewards) {
        return rewards.SelectMany(l => l.ID.GetItemsById().OrderBy(k => k.Text.CurrentLang));
      }
      else if (value is IEnumerable<string> ints) {
        return ints.SelectMany(l => l.GetItemsById().OrderBy(k => k.Text.CurrentLang));
      }
      else if (value is IEnumerable<string> strings) {
        return strings.Select(s => s).SelectMany(l => l.GetItemsById().OrderBy(k => k.Text.CurrentLang));
      }
      else if (value is string poolint) {
        return poolint.GetItemsById().OrderBy(k => k.Text.CurrentLang);
      }
      else if (value is string poolstring) {
        return poolstring.GetItemsById().OrderBy(k => k.Text.CurrentLang);
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Public Methods
  }
}