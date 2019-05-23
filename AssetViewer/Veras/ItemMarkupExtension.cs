using System;
using System.Linq;
using System.Windows.Markup;

namespace AssetViewer.Veras {

    public class ItemMarkupExtension : MarkupExtension {

        #region Properties

        public string Id { get; set; }

        public bool IsSingleItem { get; set; }

        #endregion Properties

        #region Methods

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (IsSingleItem) {
                return ItemProvider.GetItemsById(Id).FirstOrDefault();
            }
            return ItemProvider.GetItemsById(Id);
        }

        #endregion Methods
    }
}