using AssetViewer.Comparer;
using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class RaritiesFilter : BaseFilter {
    public RaritiesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
    }

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue))
        result = result.Where(w => w.Rarity.CurrentLang == SelectedValue);
      return result;
    };

    public override IEnumerable<String> CurrentValues => ItemsHolder
         .GetResultWithoutFilter(this)
         .Select(s => s.Rarity.CurrentLang)
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Concat(new[] { string.Empty })
         .OrderBy(o => o, RarityComparer.Default)
         .ToList();

    public override int DescriptionID => 1023;
  }
}