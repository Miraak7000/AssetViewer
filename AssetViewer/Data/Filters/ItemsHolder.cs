using AssetViewer.Templates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public class ItemsHolder : INotifyPropertyChanged {

    #region Properties

    public List<TemplateAsset> Items { get; set; }

    //public IQueryable<TemplateAsset> Result { get; set; } = ItemProvider.Items.Values.AsQueryable();

    public IQueryable<TemplateAsset> Base { get; } = ItemProvider.Items.Values.AsQueryable();

    public Dictionary<string, IFilter> Filters { get; } = new Dictionary<string, IFilter>();
    public bool IsRefreshingUi { get; set; }

    #endregion Properties

    #region Constructors

    public ItemsHolder() {
      Filters.Add("Rarities", new RaritiesFilter(this));
      Filters.Add("ItemTypes", new ItemTypesFilter(this));
      Filters.Add("Targets", new TargetsFilter(this));
      Filters.Add("Available", new AvailableFilter(this));
      Filters.Add("DetailedSources", new DetailedSourcesFilter(this));
      Filters.Add("Equipped", new EquippedFilter(this));
      Filters.Add("ReleaseVersions", new ReleaseVersionsFilter(this));
      Filters.Add("SearchText", new SearchTextFilter(this));
      Filters.Add("Sources", new SourcesFilter(this));
      Filters.Add("Upgrades", new UpgradesFilter(this));
      Filters.Add("TargetBuilding", new TargetBuildingFilter(this));
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

    public void UpdateUI() {
      if (!IsRefreshingUi) {
        SetItems();
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        foreach (var filter in Filters.Values) {
          filter.RaisePropertyChanged("CurrentValues");
        }
      }
    }
    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public void SetItems() {
      var result = Base;
      foreach (var filter in Filters.Values) {
        result = filter.FilterFunc(result);
      }
      result.OrderBy(s => s.Text.CurrentLang);
      Items = result.ToList();
    }
    public IQueryable<TemplateAsset> GetResultWithoutFilter(IFilter filter) {
      var result = Base;
      foreach (var f in Filters.Values.Except(new[] { filter })) {
        result = f.FilterFunc(result);
      }
      return result;
    }

    public void ResetFilters() {
      IsRefreshingUi = true;
      foreach (var filter in Filters.Values) {
        filter.SelectedValue = string.Empty;
      }
      IsRefreshingUi = false;
      UpdateUI();
    }

    #endregion Methods
  }
}