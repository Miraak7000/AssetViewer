using AssetViewer.Comparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class RaritiesFilter : BaseFilter<Description> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedComparisonValue != null && SelectedComparisonValue.ID != 0) {
        return result.Where(w => CompareToRarity(w.Rarity.ID));
      }
      else if (SelectedValue != null && SelectedValue.ID != 0) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => !w.Rarity.Equals(SelectedValue));
        }
        else {
          return result.Where(w => w.Rarity.Equals(SelectedValue));
        }
      }
      return null;
    };

    public override int DescriptionID => -1023;

    #endregion Properties

    #region Constructors

    public RaritiesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
      FilterType = FilterType.None;
    }

    #endregion Constructors

    #region Methods

    public override void SetCurrenValues() {
      CurrentValues = ItemsHolder
         .GetResultWithoutFilter(this)
         .Where(s => s.Rarity != null)
         .Select(s => s.Rarity)
         .Distinct()
         .Concat(new[] { new Description(0) })
         .OrderBy(o => o.ID, RarityComparer.Default)
         .ToList();

      ComparisonValues = ItemsHolder
         .GetResultWithoutFilter(this)
         .Where(s => s.Rarity != null)
         .Select(s => s.Rarity)
         .Distinct()
         .Concat(new[] { new Description(0) })
         .OrderBy(o => o.ID, RarityComparer.Default)
         .ToList();
    }

    private bool CompareToRarity(int l) {
      switch (Comparison) {
        case ValueComparisons.Equals:
          return RarityComparer.Default.Compare(l, SelectedComparisonValue.ID) == 0;

        case ValueComparisons.LesserThan:
          return RarityComparer.Default.Compare(l, SelectedComparisonValue.ID) <= 0;

        case ValueComparisons.GraterThan:
          return RarityComparer.Default.Compare(l, SelectedComparisonValue.ID) >= 0;

        case ValueComparisons.UnEqual:
          return RarityComparer.Default.Compare(l, SelectedComparisonValue.ID) != 0;
      }
      return false;
    }

    #endregion Methods
  }
}