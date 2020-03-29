using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class SearchTextFilter : BaseFilter<string> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue)) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => !w.ID.ToString().StartsWith(SelectedValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(SelectedValue, StringComparison.CurrentCultureIgnoreCase) == -1);
        }
        else {
          return result.Where(w => w.ID.ToString().StartsWith(SelectedValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(SelectedValue, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }
      }

      return null;
    };

    public override int DescriptionID => -1004;

    public override string SelectedValue {
      get {
        return _selectedValue;
      }
      set {
        if (!(_selectedValue?.Equals(value) ?? false)) {
          _selectedValue = value;
          UpdateSavedItems();
          RaisePropertyChanged();
        }
      }
    }

    #endregion Properties

    #region Constructors

    public SearchTextFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      FilterType = FilterType.Text;
    }

    #endregion Constructors

    #region Fields

    private string _selectedValue;

    #endregion Fields
  }
}