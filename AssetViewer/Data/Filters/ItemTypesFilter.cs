using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ItemTypesFilter : BaseFilter<string> {
    public ItemTypesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue))
        if (Comparison == ValueComparisons.UnEqual) {
          result = result.Where(w => w.ItemType != SelectedValue);
        }
        else {
          result = result.Where(w => w.ItemType == SelectedValue);
        }
      return result;
    };

    public override IEnumerable<String> CurrentValues => ItemsHolder
        .GetResultWithoutFilter(this)
        .Select(s => s.ItemType)
        .Distinct()
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .Concat(new[] { string.Empty })
        .OrderBy(o => o)
        .ToList();

    public override string DescriptionID => "-1002";
  }
}