using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class SearchTextFilter : BaseFilter {
    public SearchTextFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
    }
    private string _selectedValue;
    public override string SelectedValue {
      get { return _selectedValue; }
      set {
        if (_selectedValue != value) {
          _selectedValue = value;
          RaisePropertyChanged();
          ItemsHolder.UpdateUI();
        }
      }
    }
    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue))
        result = result.Where(w => w.ID.StartsWith(SelectedValue, StringComparison.InvariantCultureIgnoreCase) || w.Text.CurrentLang.IndexOf(SelectedValue, StringComparison.CurrentCultureIgnoreCase) >= 0);
      return result;
    };

    public override IEnumerable<String> CurrentValues => Enumerable.Empty<string>();

    public override int DescriptionID => 1004;
  }
}