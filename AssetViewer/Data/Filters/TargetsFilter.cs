using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class TargetsFilter : BaseFilter<string> {

    #region Properties

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue))
        if (Comparison == ValueComparisons.UnEqual) {
          result = result.Where(w => w.EffectTargets != null && !w.EffectTargets.Any(s => s.Text.CurrentLang == SelectedValue));
        }
        else {
          result = result.Where(w => w.EffectTargets != null && w.EffectTargets.Any(s => s.Text.CurrentLang == SelectedValue));
        }
      return result;
    };

    public override IEnumerable<String> CurrentValues => ItemsHolder
         .GetResultWithoutFilter(this)
         .SelectMany(s => s.EffectTargets)
         .Select(s => s.Text.CurrentLang)
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Concat(new[] { string.Empty })
         .OrderBy(o => o)
         .ToList();

    public override string DescriptionID => "-1003";

    #endregion Properties

    #region Constructors

    public TargetsFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Constructors
  }
}