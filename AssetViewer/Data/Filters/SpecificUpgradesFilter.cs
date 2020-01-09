using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class SpecificUpgradesFilter : BaseFilter<Description> {

    #region Properties

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue != null && SelectedValue.ID != 0) {
        if (ComparisonType != FilterType.None && SelectedComparisonValue != null && SelectedComparisonValue.ID != 0) {
          result = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text == SelectedValue && CompareToUpgrade(l)));
        }
        else if (Comparison == ValueComparisons.UnEqual) {
          result = result.Where(w => w.AllUpgrades != null && !w.AllUpgrades.Any(l => l.Text != null && l.Text == SelectedValue));
        }
        else {
          result = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text == SelectedValue));
        }
      }

      return result;
    };

    public override IEnumerable<Description> CurrentValues => ItemsHolder
         .GetResultWithoutFilter(this)
         .SelectMany(s => s.AllUpgrades)
         .Select(s => s.Text)
         .Distinct()
         .Concat(new[] { new Description(0) })
         .OrderBy(o => o.CurrentLang)
         .ToList();

    public override IEnumerable<Description> ComparisonValues => (new[] { new Description(0) }).Concat(ItemsHolder
         .GetResultWithoutFilter(this)
         .SelectMany(s => s.AllUpgrades.Where(u => u.Text == SelectedValue))
         .Select(u => u.Value)
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .OrderBy(o => float.Parse(o.TrimEnd(' ', '%').Replace(":", ""))))
         .ToList();

    public override int DescriptionID => -1006;

    #endregion Properties

    #region Constructors

    public SpecificUpgradesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Constructors

    #region Methods

    private bool CompareToUpgrade(Upgrade l) {
      if (float.TryParse(l.Value.TrimEnd(' ', '%').Replace(":", ""), out var x) && float.TryParse(this.SelectedComparisonValue.TrimEnd(' ', '%').Replace(":", ""), out var y)) {
        switch (Comparison) {
          case ValueComparisons.Equals:
            return x == y;

          case ValueComparisons.LesserThan:
            return x <= y;

          case ValueComparisons.GraterThan:
            return x >= y;

          case ValueComparisons.UnEqual:
            return x != y;
        }
      }
      else {
        var stringCompare = l.Value.CompareTo(this.SelectedValue);
        switch (stringCompare) {
          case -1:
            return Comparison == ValueComparisons.LesserThan;

          case 0:
            return true;

          case 1:
            return Comparison == ValueComparisons.GraterThan;
        }
      }
      return false;
    }

    #endregion Methods
  }
}