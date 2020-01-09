using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ItemSetFilter : BaseFilter<Description> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue != null && SelectedValue.ID != 0) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => w.ItemSets?.Any(l => l.Text?.Equals(SelectedValue) == true) == false);
        }
        else {
          return result.Where(w => w.ItemSets?.Any(l => l.Text?.Equals(SelectedValue) == true) == true);
        }
      }

      return null;
    };

    public override int DescriptionID => -1221;

    #endregion Properties

    #region Constructors

    public ItemSetFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Constructors

    #region Methods

    public override void SetCurrenValues() {
      CurrentValues = ItemsHolder
         .GetResultWithoutFilter(this)
         .Where(s => s.ItemSets != null)
         .SelectMany(s => s.ItemSets)
         .Select(s => s.Text)
         .Distinct()
         .Concat(new[] { new Description(0) })
         .OrderBy(o => o.CurrentLang)
         .ToList();
    }

    #endregion Methods
  }
}