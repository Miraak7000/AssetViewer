using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class SearchTextFilter : BaseFilter<string> {

    #region Public Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      return FilterItems(result, SelectedValue);
    };

    public override int DescriptionID => -1004;

    public override string SelectedValue {
      get => _selectedValue;
      set {
        var tempval = value.Trim();
        if (!(_selectedValue?.Equals(tempval) ?? false)) {
          //_oldValue = _selectedValue;
          if ((tempval != null && string.IsNullOrWhiteSpace(tempval)) || _possibilities == null || (FilterItems(_possibilities, tempval)?.Any() ?? false)) {
            _selectedValue = tempval;
            UpdateSavedItems();
          }
          RaisePropertyChanged();
        }
      }
    }

    #endregion Public Properties

    #region Public Constructors

    public SearchTextFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      FilterType = FilterType.Text;
    }

    #endregion Public Constructors

    #region Public Methods

    public override void SetCurrenValues() {
      _possibilities = ItemsHolder.GetResultWithoutFilter(this).ToList();
    }

    #endregion Public Methods

    #region Private Methods

    private IEnumerable<TemplateAsset> FilterItems(IEnumerable<TemplateAsset> result, string selectedValue) {
      if (!string.IsNullOrEmpty(selectedValue)) {
        if (Comparison == ValueComparisons.UnEqual) {
          var res = result.Where(w => !w.ID.ToString().StartsWith(selectedValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(selectedValue, StringComparison.CurrentCultureIgnoreCase) == -1);
          //if (!res.Any()) {
          //  _selectedValue = _oldValue;
          //  return result.Where(w => !w.ID.ToString().StartsWith(_oldValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(_oldValue, StringComparison.CurrentCultureIgnoreCase) == -1); ;
          //}
          return res;
        }
        else {
          var res = result.Where(w => w.ID.ToString().StartsWith(selectedValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(selectedValue, StringComparison.CurrentCultureIgnoreCase) >= 0);
          //if (!res.Any()) {
          //  _selectedValue = _oldValue;
          //  return result.Where(w => w.ID.ToString().StartsWith(_oldValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(_oldValue, StringComparison.CurrentCultureIgnoreCase) >= 0);
          //}
          return res;
        }
      }

      return null;
    }

    #endregion Private Methods

    #region Private Fields

    private string _selectedValue;
    private string _oldValue = string.Empty;
    private List<TemplateAsset> _possibilities;

    #endregion Private Fields
  }
}