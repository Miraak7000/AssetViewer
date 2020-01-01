using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class UpgradesFilter : BaseFilter<string> {

    #region Properties

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue)) {
        if (ComparisonType != FilterType.None && !String.IsNullOrEmpty(SelectedComparisonValue)) {
          var hits = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text.CurrentLang == SelectedValue));

          result = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text.CurrentLang == SelectedValue && CompareToUpgrade(l)))
           .Concat(result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text.CurrentLang == SelectedValue && l.Additionals != null && l.Additionals.Any(a => a.Text != null && a.Text.CurrentLang == SelectedComparisonValue))))
           .Concat(result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text.CurrentLang == SelectedValue && l.Text.AdditionalInformation != null && l.Text.AdditionalInformation.CurrentLang == SelectedComparisonValue)));
        }
        else if (Comparison == ValueComparisons.UnEqual) {
          result = result.Where(w => w.AllUpgrades != null && !w.AllUpgrades.Any(l => l.Text != null && l.Text.CurrentLang == SelectedValue));
        }
        else {
          result = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text.CurrentLang == SelectedValue));
        }
      }

      return result;
    };

    public override IEnumerable<String> CurrentValues => ItemsHolder
         .GetResultWithoutFilter(this)
         .SelectMany(s => s.AllUpgrades)
         .Select(s => s.Text.CurrentLang)
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Concat(new[] { string.Empty })
         .OrderBy(o => o)
         .ToList();

    public override IEnumerable<string> ComparisonValues {
      get {
        var list = (new[] { string.Empty }).Concat(ItemsHolder
           .GetResultWithoutFilter(this)
           .SelectMany(s => s.AllUpgrades.Where(u => u.Text.CurrentLang == SelectedValue))
           .Select(u => u.Value)
           .Distinct()
           .Where(l => !string.IsNullOrWhiteSpace(l))
           .OrderBy(o => float.Parse(o.TrimEnd(' ', '%'))))
           .ToList();
        if (list.Count == 1) {
          list = (new[] { string.Empty }).Concat(ItemsHolder
           .GetResultWithoutFilter(this)
           .SelectMany(s => s.AllUpgrades.Where(u => u.Text.CurrentLang == SelectedValue))
           .SelectMany(u => u.Additionals ?? Enumerable.Empty<Upgrade>()).Select(a => a.Text.CurrentLang)
           .Distinct()
           .Where(l => !string.IsNullOrWhiteSpace(l))
           .OrderBy(o => o))
           .ToList();
        }
        if (list.Count == 1) {
          list = (new[] { string.Empty }).Concat(ItemsHolder
           .GetResultWithoutFilter(this)
           .SelectMany(s => s.AllUpgrades.Where(u => u.Text.CurrentLang == SelectedValue && u.Text.AdditionalInformation != null))
           .Select(u => u.Text.AdditionalInformation.CurrentLang)
           .Distinct()
           .Where(l => !string.IsNullOrWhiteSpace(l))
           .OrderBy(o => o))
           .ToList();
        }

        return list;
      }
    }

    public override string DescriptionID => "-1006";

    #endregion Properties

    #region Constructors

    public UpgradesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Constructors

    #region Methods

    private bool CompareToUpgrade(Upgrade l) {
      if (float.TryParse(l.Value?.TrimEnd(' ', '%'), out var x) && float.TryParse(this.SelectedComparisonValue.TrimEnd(' ', '%'), out var y)) {
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
        var stringCompare = l.Value?.CompareTo(this.SelectedValue);
        switch (stringCompare) {
          case -1:
            return Comparison == ValueComparisons.LesserThan || Comparison == ValueComparisons.UnEqual;

          case 0:
            return true;

          case 1:
            return Comparison == ValueComparisons.GraterThan || Comparison == ValueComparisons.UnEqual;
        }
      }
      return false;
    }

    #endregion Methods
  }
}