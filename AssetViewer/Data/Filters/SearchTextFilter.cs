using AssetViewer.Templates;
using System;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class SearchTextFilter : BaseFilter<string> {

    #region Properties

    public override string SelectedValue {
      get {
        return _selectedValue;
      }
      set {
        if (_selectedValue != value) {
          _selectedValue = value;
          RaisePropertyChanged();
          ItemsHolder.UpdateUI(this);
        }
      }
    }

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue))
        if (Comparison == ValueComparisons.UnEqual) {
          result = result.Where(w => !w.ID.StartsWith(SelectedValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(SelectedValue, StringComparison.CurrentCultureIgnoreCase) == -1);

        }
        else {
          result = result.Where(w => w.ID.StartsWith(SelectedValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(SelectedValue, StringComparison.CurrentCultureIgnoreCase) >= 0);

        }
      return result;
    };

    public override string DescriptionID => "-1004";

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