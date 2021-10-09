using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ReleaseVersionsFilter : BaseFilter<string> {

    #region Public Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (!string.IsNullOrEmpty(SelectedComparisonValue)) {
        return result.Where(w => CompareToReleaseVersion(w.ReleaseVersion));
      }
      if (!string.IsNullOrEmpty(SelectedValue)) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => w.ReleaseVersion != SelectedValue);
        }
        else {
          return result.Where(w => w.ReleaseVersion == SelectedValue);
        }
      }

      return null;
    };

    public override int DescriptionID => -1007;

    #endregion Public Properties

    #region Public Constructors

    public ReleaseVersionsFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      FilterType = FilterType.None;
      ComparisonType = FilterType.Selection;
    }

    #endregion Public Constructors

    #region Public Methods

    public override void SetCurrenValues() {
      ComparisonValues = ItemsHolder
         .GetResultWithoutFilter(this)
         .Select(s => s.ReleaseVersion)
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Concat(new[] { string.Empty })
         .OrderBy(o => o)
         .ToList();
    }

    #endregion Public Methods

    #region Private Methods

    private bool CompareToReleaseVersion(string l) {
      switch (Comparison) {
        case ValueComparisons.Equals:
          return l.CompareTo(SelectedComparisonValue) == 0;

        case ValueComparisons.LesserThan:
          return l.CompareTo(SelectedComparisonValue) <= 0;

        case ValueComparisons.GraterThan:
          return l.CompareTo(SelectedComparisonValue) >= 0;

        case ValueComparisons.UnEqual:
          return l.CompareTo(SelectedComparisonValue) != 0;
      }
      return false;
    }

    #endregion Private Methods
  }
}