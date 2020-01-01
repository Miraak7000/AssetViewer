using AssetViewer.Comparer;
using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class RaritiesFilter : BaseFilter<string> {

    #region Properties

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedComparisonValue)) {
        return result.Where(w => CompareToRarity(w.Rarity.CurrentLang));
      }
      else if (!String.IsNullOrEmpty(SelectedValue)) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => w.Rarity.CurrentLang != SelectedValue);
        }
        else {
          return result.Where(w => w.Rarity.CurrentLang == SelectedValue);
        }
      }
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

    public override IEnumerable<string> ComparisonValues => ItemsHolder
         .GetResultWithoutFilter(this)
         .Select(s => s.Rarity.CurrentLang)
         .Distinct()
         .Where(l => !string.IsNullOrWhiteSpace(l))
         .Concat(new[] { string.Empty })
         .OrderBy(o => o, RarityComparer.Default)
         .ToList();

    public override string DescriptionID => "-1023";

    #endregion Properties

    #region Constructors

    public RaritiesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
      FilterType = FilterType.None;
    }

    #endregion Constructors

    #region Methods

    private bool CompareToRarity(string l) {
      switch (Comparison) {
        case ValueComparisons.Equals:
          return RarityComparer.Default.Compare(l, SelectedComparisonValue) == 0;

        case ValueComparisons.LesserThan:
          return RarityComparer.Default.Compare(l, SelectedComparisonValue) <= 0;

        case ValueComparisons.GraterThan:
          return RarityComparer.Default.Compare(l, SelectedComparisonValue) >= 0;

        case ValueComparisons.UnEqual:
          return RarityComparer.Default.Compare(l, SelectedComparisonValue) != 0;
      }
      return false;
    }

    #endregion Methods
  }
}