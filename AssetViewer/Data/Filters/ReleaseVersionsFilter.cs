using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ReleaseVersionsFilter : BaseFilter<string> {
    public ReleaseVersionsFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      FilterType = FilterType.None;
      ComparisonType = FilterType.Selection;
    }

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedComparisonValue))
        result = result.Where(w => CompareToReleaseVersion(w.ReleaseVersion));
      if (!String.IsNullOrEmpty(SelectedValue))
        result = result.Where(w => w.ReleaseVersion == SelectedValue);
      return result;
    };

    public override IEnumerable<string> ComparisonValues => ItemsHolder
         .GetResultWithoutFilter(this)
         .Select(s => s.ReleaseVersion)
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Concat(new[] { string.Empty })
         .OrderBy(o => o)
         .ToList();

    private bool CompareToReleaseVersion(string l) {
      switch (Comparison) {
        case ValueComparisons.Equals:
          return l.CompareTo(SelectedComparisonValue) == 0;

        case ValueComparisons.LesserThan:
          return l.CompareTo(SelectedComparisonValue) <= 0;

        case ValueComparisons.GraterThan:
          return l.CompareTo(SelectedComparisonValue) >= 0;
      }
      return false;
    }
    public override string DescriptionID => "-1007";
  }
}