using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class EquippedFilter : BaseFilter<string> {

    #region Properties

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue))
        if (Comparison == ValueComparisons.UnEqual) {
          result = result.Where(w => w.Allocation != null && w.Allocation.Text.CurrentLang != SelectedValue);
        }
        else {
          result = result.Where(w => w.Allocation != null && w.Allocation.Text.CurrentLang == SelectedValue);
        }
      return result;
    };

    public override IEnumerable<String> CurrentValues => ItemsHolder
         .GetResultWithoutFilter(this)
         .Select(s => s.Allocation == null ? "" : s.Allocation.Text.CurrentLang)
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Concat(new[] { string.Empty })
         .OrderBy(o => o)
         .ToList();

    public override string DescriptionID => "-106";

    #endregion Properties

    #region Constructors

    public EquippedFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
    }

    #endregion Constructors
  }
}