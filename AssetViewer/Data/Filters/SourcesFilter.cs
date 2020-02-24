using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class SourcesFilter : BaseFilter<Description> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedComparisonValue != null && SelectedComparisonValue.ID != 0) {
        if (SelectedValue != null && SelectedValue.ID != 0) {
          if (Comparison == ValueComparisons.UnEqual) {
            return result.Where(w => w.Sources?.Any(s => !(s.Text.Equals(SelectedValue) && s.Additionals.Any(u => u.Text.Equals(SelectedComparisonValue)))) == true);
          }
          else {
            return result.Where(w => w.Sources?.Where(s => s.Text.Equals(SelectedValue)).SelectMany(s => s.Additionals).Any(l => l.Text.Equals(SelectedComparisonValue)) == true);
          }

        }
        else {
          if (Comparison == ValueComparisons.UnEqual) {
            return result.Where(w => w.Sources?.Any(l => !l.Additionals.Any(a=> a.Text.Equals(SelectedComparisonValue))) == true);
          }
          else {
            return result.Where(w => w.Sources?.SelectMany(s => s.Additionals).Any(l => l.Text.Equals(SelectedComparisonValue)) == true);
          }
        }
      }
      else if (SelectedValue != null && SelectedValue.ID != 0) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => w.Sources?.Any(l => l.Text.Equals(SelectedValue)) == false);
        }
        else {
          return result.Where(w => w.Sources?.Any(l => l.Text.Equals(SelectedValue)) == true);
        }
      }
      return null;
    };

    public override int DescriptionID => -1005;

    #endregion Properties

    #region Constructors

    public SourcesFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Constructors

    #region Methods

    public override void SetCurrenValues() {
      CurrentValues = ItemsHolder
         .GetResultWithoutFilter(this)
         .Where(s => s.Sources != null)
         .SelectMany(s => s.Sources)
         .Select(s => s.Text)
         .Distinct()
         .Concat(new[] { new Description(0) })
         .OrderBy(o => o.CurrentLang)
         .ToList();

      ComparisonValues = GetComparisonValues();
    }

    private List<Description> GetComparisonValues() {
      if (SelectedValue == null || SelectedValue.ID == 0) {
        return ItemsHolder
        .GetResultWithoutFilter(this)
        .Where(s => s.Sources != null)
        .SelectMany(s => s.Sources)
        .SelectMany(s => s.Additionals)
        .Select(s => s.Text)
        .Distinct()
        .Concat(new[] { new Description(0) })
        .OrderBy(o => o.CurrentLang)
        .ToList();
      }
      else {
        return ItemsHolder
        .GetResultWithoutFilter(this)
        .Where(s => s.Sources != null)
        .SelectMany(s => s.Sources)
        .Where(s => s.Text.Equals(SelectedValue))
        .SelectMany(s => s.Additionals)
        .Select(s => s.Text)
        .Distinct()
        .Concat(new[] { new Description(0) })
        .OrderBy(o => o.CurrentLang)
        .ToList();
      }
    }

    #endregion Methods
  }
}