using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public abstract class ItemsHolder : INotifyPropertyChanged {

    #region Public Properties

    public IFilter OrderFilter { get; }
    public List<TemplateAsset> Items { get; set; }

    public List<TemplateAsset> Base { get; }

    public Dictionary<string, IFilter> StandardFilters { get; } = new Dictionary<string, IFilter>();
    public ObservableCollection<FilterHolder> CustomFilters { get; } = new ObservableCollection<FilterHolder>();
    public bool IsRefreshingUi { get; set; }

    public List<IFilter> AllFilters { get; set; } = new List<IFilter>();

    #endregion Public Properties

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Methods

    public void UpdateUI(IFilter filter = null) {
      if (!IsRefreshingUi) {
        IsRefreshingUi = true;
        SetItems();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
        foreach (var item in StandardFilters.Values.Concat(CustomFilters.Select(cf => cf.SelectedFilter)).Except(new[] { filter })) {
          item.UpdateUI();
        }
        OrderFilter.UpdateUI();
        IsRefreshingUi = false;
      }
    }

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void SetItems() {
      Items = OrderFilter.FilterFunc(GetResultWithoutFilter<IFilter>(null)).ToList();
    }

    public IEnumerable<TemplateAsset> GetResultWithoutFilter<T>(IFilter<T> filter) {
      IEnumerable<TemplateAsset> result = null;
      foreach (var f in StandardFilters.Values.Concat(CustomFilters.Where(cf => cf.SelectedFilter != null).Select(cf => cf.SelectedFilter)).Except(new[] { filter })) {
        if (f.SavedItems != null) {
          result = result == null ? f.SavedItems : result.Intersect(f.SavedItems);
        }
      }
      if (result == null) {
        result = Base;
      }
      return result;
    }

    public void ResetFilters() {
      IsRefreshingUi = true;
      foreach (var filter in StandardFilters.Values) {
        filter.ResetFilter();
      }
      CustomFilters.Clear();
      CustomFilters.Add(CreateFilterHolder());
      OrderFilter.ResetFilter();
      IsRefreshingUi = false;
      UpdateUI();
    }

    public void RaiseLanguageChanged() {
      ResetFilters();
    }

    public abstract FilterHolder CreateFilterHolder();

    #endregion Public Methods

    #region Protected Constructors

    protected ItemsHolder(List<TemplateAsset> Base) {
      this.Base = Base;
      OrderFilter = new OrderFilter(this);
      CustomFilters.Add(CreateFilterHolder());
    }

    #endregion Protected Constructors
  }
}