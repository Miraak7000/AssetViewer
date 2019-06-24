using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class DetailedSourcesFilter : BaseFilter<string> {
    public DetailedSourcesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
    }

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue as string))
        result = result.Where(w => w.Sources != null && w
            .Sources
            .SelectMany(s => s.Additionals)
            .Any(l => l.Text.CurrentLang == SelectedValue));
      return result;
    };

    public override IEnumerable<String> CurrentValues => GetValues();
    private IEnumerable<string> GetValues() {
      if (String.IsNullOrEmpty(ItemsHolder.StandardFilters["Sources"].SelectedValue as string)) {
        return ItemsHolder
        .GetResultWithoutFilter(this)
        .SelectMany(s => s.Sources)
        .SelectMany(s => s.Additionals)
        .Select(s => s.Text.CurrentLang)
        .Distinct()
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .Concat(new[] { string.Empty })
        .OrderBy(o => o)
        .ToList();
      }
      else {
        return ItemsHolder
        .GetResultWithoutFilter(this)
        .SelectMany(s => s.Sources)
        .Where(s=> s.Text.CurrentLang == ItemsHolder.StandardFilters["Sources"].SelectedValue)
        .SelectMany(s => s.Additionals)
        .Select(s => s.Text.CurrentLang)
        .Distinct()
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .Concat(new[] { string.Empty })
        .OrderBy(o => o)
        .ToList();
      }
     
    }



  public override int DescriptionID => 1008;
}
}