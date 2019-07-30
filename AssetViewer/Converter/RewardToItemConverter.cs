using AssetViewer.Data;
using AssetViewer.Library;
using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AssetViewer.Converter {

  [ValueConversion(typeof(IEnumerable<RewardsItem>), typeof(IEnumerable<TemplateAsset>))]
  [ValueConversion(typeof(RewardsItem), typeof(IEnumerable<TemplateAsset>))]
  public class RewardToItemConverter : IValueConverter {

    #region Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value is RewardsItem item) {
        return item.ID.GetItemsById().OrderBy(l => l.Text.CurrentLang);
      }
      else if (value is IEnumerable<RewardsItem> rewards) {
        return rewards.SelectMany(l => l.ID.GetItemsById().OrderBy(k => k.Text.CurrentLang));
      }
      else if (value is string pool) {
        return pool.GetItemsById().OrderBy(k => k.Text.CurrentLang);
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }

    #endregion Methods
  }
}