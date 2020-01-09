using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class UpgradesFilter : BaseFilter<Description> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue != null && SelectedValue.ID != 0) {
        if (ComparisonType != FilterType.None && SelectedComparisonValue != null && SelectedComparisonValue.ID != 0) {
          return result
          .Where(w =>
          w.AllUpgrades.Any(l => l.Text?.Equals(SelectedValue) == true &&
          (
          (l.Additionals?.Any(a => a.Text?.Equals(SelectedComparisonValue) == true) == true) ||
          (l.Text.AdditionalInformation?.Equals(SelectedComparisonValue) == true) ||
          CompareToUpgrade(l)
          )));
        }
        else if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => !w.AllUpgrades.Any(l => l.Text?.Equals(SelectedValue) == true));
        }
        else {
          return result.Where(w => w.AllUpgrades.Any(l => l.Text.Equals(SelectedValue)));
        }
      }
      return null;
    };

    public override int DescriptionID => -1006;

    #endregion Properties

    #region Constructors

    public UpgradesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Constructors

    #region Methods

    public override void SetCurrenValues() {
      var items = ItemsHolder.GetResultWithoutFilter(this).ToList();
      CurrentValues = items
         .SelectMany(s => s.AllUpgrades)
         .Select(s => s.Text)
         .Distinct()
         .Concat(new[] { new Description(0) })
         .OrderBy(o => o.CurrentLang)
         .ToList();

      var upgrades = items.SelectMany(s => s.AllUpgrades.Where(u => u.Text.Equals(SelectedValue)).ToList());

      var list = (new[] { new Description(0) }).Concat(upgrades
          .Where(u => !string.IsNullOrWhiteSpace(u.Value))
          .Select(u => new DummyDescription(u.Value))
          .Distinct()
          .OrderBy(o => float.Parse(o.CurrentLang.TrimEnd(' ', '%').Replace(":", "").Replace("/", "").Replace(" ", ""))));

      if (!list.Skip(1).Any()) {
        list = (new[] { new Description(0) }).Concat(upgrades
         .SelectMany(u => u.Additionals ?? Enumerable.Empty<Upgrade>()).Select(a => a.Text)
         .Distinct()
         .OrderBy(o => o.CurrentLang));
      }

      if (!list.Skip(1).Any()) {
        list = (new[] { new Description(0) }).Concat(upgrades
         .Where(u => u.Text.AdditionalInformation != null)
         .Select(u => u.Text.AdditionalInformation)
         .Distinct()
         .OrderBy(o => o.CurrentLang));
      }

      ComparisonValues = list.ToList();
    }

    private bool CompareToUpgrade(Upgrade l) {
      if (float.TryParse(l.Value?.TrimEnd(' ', '%').Replace(":", "").Replace("/", "").Replace(" ", ""), out var x) && float.TryParse(this.SelectedComparisonValue.CurrentLang.TrimEnd(' ', '%').Replace(":", "").Replace("/", "").Replace(" ", ""), out var y)) {
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
        var stringCompare = l.Value?.CompareTo(this.SelectedValue.CurrentLang);
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