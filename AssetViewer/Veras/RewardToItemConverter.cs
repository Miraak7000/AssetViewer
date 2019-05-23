using AssetViewer.Templates;
using AssetViewer.Veras;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AssetViewer.Converter {

    [ValueConversion(typeof(IEnumerable<ExpeditionEventPathRewardsItem>), typeof(IEnumerable<TemplateAsset>))]
    [ValueConversion(typeof(ExpeditionEventPathRewardsItem), typeof(IEnumerable<TemplateAsset>))]
    public class RewardToItemConverter : IValueConverter {

        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is ExpeditionEventPathRewardsItem item) {
                return ItemProvider.GetItemsById(item.ID);
            }
            else if (value is IEnumerable<ExpeditionEventPathRewardsItem> rewards) {
                return rewards.SelectMany(l => ItemProvider.GetItemsById(l.ID));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}