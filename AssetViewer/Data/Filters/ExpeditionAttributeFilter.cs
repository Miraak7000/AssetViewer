using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ExpeditionAttributeFilter : BaseFilter<Description> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue != null && SelectedValue.ID != 0) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => w.ExpeditionAttributes?.Any(l => l.Text.Equals(SelectedValue)) == false);
        }
        else {
          return result.Where(w => w.ExpeditionAttributes?.Any(l => l.Text.Equals(SelectedValue)) == true);
        }
      }

      return null;
    };

    public override int DescriptionID => -1220;

    #endregion Properties

    #region Constructors

    public ExpeditionAttributeFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Constructors

    #region Methods

    public override void SetCurrenValues() {
      CurrentValues = ItemsHolder
         .GetResultWithoutFilter(this)
         .Where(s => s.ExpeditionAttributes != null)
         .SelectMany(s => s.ExpeditionAttributes)
         .Select(s => s.Text)
         .Distinct()
         .Concat(new[] { new Description(0) })
         .OrderBy(o => o.CurrentLang)
         .ToList();
    }

    #endregion Methods
  }
}