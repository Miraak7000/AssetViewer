using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class AvailableFilter : BaseFilter<bool> {
    public AvailableFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      FilterType = FilterType.Bool;
    }

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue)
        result = result.Where(w => w.Sources.Count > 0);
      return result;
    };

    public override void ResetFilter() {
      SelectedValue = true;
    }
    public override IEnumerable<bool> ComparisonValues => base.ComparisonValues;

    public override int DescriptionID => 1101;
  }
}