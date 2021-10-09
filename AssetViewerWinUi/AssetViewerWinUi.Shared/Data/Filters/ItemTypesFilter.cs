using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ItemTypesFilter : BaseFilter<string> {

    #region Public Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (!string.IsNullOrEmpty(SelectedValue)) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => w.ItemType != SelectedValue);
        }
        else {
          return result.Where(w => w.ItemType == SelectedValue);
        }
      }

      return null;
    };

    public override int DescriptionID => -1002;

    #endregion Public Properties

    #region Public Constructors

    public ItemTypesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Public Constructors

    #region Public Methods

    public override void SetCurrenValues() {
      CurrentValues = ItemsHolder
        .GetResultWithoutFilter(this)
        .Select(s => s.ItemType)
        .Distinct()
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .Concat(new[] { string.Empty })
        .OrderBy(o => o)
        .ToList();
    }

    #endregion Public Methods
  }
}