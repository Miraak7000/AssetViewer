using AssetViewer.Templates;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public class ItemsHolder : INotifyPropertyChanged {

    #region Properties

    public List<TemplateAsset> Items { get; set; }

    public IQueryable<TemplateAsset> Base { get; } = ItemProvider.Items.Values.AsQueryable();

    public Dictionary<string, IFilter> StandardFilters { get; } = new Dictionary<string, IFilter>();
    public ObservableCollection<FilterHolder> CustomFilters { get; }
    public bool IsRefreshingUi { get; set; }

    public List<IFilter> AllFilters { get; set; } = new List<IFilter>();

    #endregion Properties

    #region Constructors

    public ItemsHolder() {
      StandardFilters.Add("Upgrades", new UpgradesFilter(this));
      StandardFilters.Add("Rarities", new RaritiesFilter(this));
      //StandardFilters.Add("ItemTypes", new ItemTypesFilter(this));
      //StandardFilters.Add("Targets", new TargetsFilter(this));
      StandardFilters.Add("Available", new AvailableFilter(this) { SelectedValue = true });
      //StandardFilters.Add("DetailedSources", new DetailedSourcesFilter(this));
      StandardFilters.Add("Equipped", new EquippedFilter(this));
      //StandardFilters.Add("ReleaseVersions", new ReleaseVersionsFilter(this));
      StandardFilters.Add("SearchText", new SearchTextFilter(this));
      //StandardFilters.Add("Sources", new SourcesFilter(this));
      StandardFilters.Add("TargetBuilding", new TargetBuildingFilter(this));
      StandardFilters.Add("Order", new OrderFilter(this));

      CustomFilters = new ObservableCollection<FilterHolder>(new[] { new FilterHolder(this) });
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

    public void UpdateUI(IFilter filter = null) {
      if (!IsRefreshingUi) {
        SetItems();
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        if (filter == null) {
          foreach (var item in StandardFilters.Values.Concat(CustomFilters.Where(cf => cf.SelectedFilter != null).Select(cf => cf.SelectedFilter))) {
            item.UpdateUI();
          }
        }
        else {
          foreach (var item in StandardFilters.Values.Concat(CustomFilters.Where(cf => cf.SelectedFilter != null).Select(cf => cf.SelectedFilter).Except(new[] { filter }))) {
            item.UpdateUI();
          }
        }
      }
    }

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void SetItems() {
      var result = Base;
      foreach (var filter in StandardFilters.Values.Concat(CustomFilters.Where(cf => cf.SelectedFilter != null).Select(cf => cf.SelectedFilter))) {
        result = filter.FilterFunc(result);
      }
      //result = result.OrderBy(s => s.Text);
      Items = result.ToList();
    }

    public IQueryable<TemplateAsset> GetResultWithoutFilter<T>(IFilter<T> filter) {
      var result = Base;

      foreach (var f in StandardFilters.Values.Concat(CustomFilters.Where(cf => cf.SelectedFilter != null).Select(cf => cf.SelectedFilter)).Except(new[] { filter })) {
        result = f.FilterFunc(result);
      }
      return result;
    }

    public void ResetFilters() {
      IsRefreshingUi = true;
      foreach (var filter in StandardFilters.Values) {
        filter.ResetFilter();
      }
      CustomFilters.Clear();
      IsRefreshingUi = false;
      UpdateUI();
    }

    public void RaiseLanguageChanged() {
      IsRefreshingUi = true;
      foreach (var filter in StandardFilters.Values) {
        filter.ResetFilter();
      }
      CustomFilters.Clear();
      IsRefreshingUi = false;
      UpdateUI();
    }

    #endregion Methods
  }
}