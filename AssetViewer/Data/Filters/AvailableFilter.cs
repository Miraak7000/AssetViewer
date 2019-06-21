using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class AvailableFilter : BaseFilter {
    public AvailableFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      IsChecked = true;
    }

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (IsChecked)
        result = result.Where(w => w.Sources.Count > 0);
      return result;
    };

    public override IEnumerable<String> CurrentValues => Enumerable.Empty<string>();

    public override int DescriptionID => 1101;
  }
}