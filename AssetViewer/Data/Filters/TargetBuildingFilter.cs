using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class TargetBuildingFilter : BaseFilter<string> {

    #region Properties

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue))
        result = result.Where(w => w.EffectTargets != null && w.EffectTargets.SelectMany(e => e.Buildings).Any(s => s.CurrentLang == SelectedValue));
      return result;
    };

    public override IEnumerable<String> CurrentValues => ItemsHolder
         .GetResultWithoutFilter(this)
         .SelectMany(s => s.EffectTargets)
         .SelectMany(s => s.Buildings)
         .Select(s => s.CurrentLang)
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Concat(new[] { string.Empty })
         .OrderBy(o => o)
         .ToList();

    public override string DescriptionID => "-1102";

    #endregion Properties

    #region Constructors

    public TargetBuildingFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
    }

    #endregion Constructors
  }
}