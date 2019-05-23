using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace AssetViewer.Veras {
    public class ItemMarkupExtension : MarkupExtension {
        public string Id { get; set; }
        
        public bool IsSingleItem { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (IsSingleItem) {
                return ItemProvider.GetItemsById(Id).FirstOrDefault();
            }
            return ItemProvider.GetItemsById(Id);
        }
    }
}
