using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ItemsHolderGuildhouse : ItemsHolder {

    #region Public Constructors

    public ItemsHolderGuildhouse() : base(AssetProvider.Items.Values.ToList()) {
      StandardFilters.Add("Upgrades", new UpgradesFilter(this));
      StandardFilters.Add("Rarities", new RaritiesFilter(this));
      StandardFilters.Add("Available", new AvailableFilter(this) { SelectedValue = true });
      StandardFilters.Add("Equipped", new EquippedFilter(this));
      StandardFilters.Add("SearchText", new SearchTextFilter(this));
      StandardFilters.Add("TargetBuilding", new TargetBuildingFilter(this));
    }

    #endregion Public Constructors

    #region Public Methods

    public override FilterHolder CreateFilterHolder() {
      var holder = new FilterHolder();
      holder.Filters.Add(new UpgradesFilter(this));
      holder.Filters.Add(new SourcesFilter(this));
      holder.Filters.Add(new ItemTypesFilter(this));
      holder.Filters.Add(new TargetsFilter(this));
      holder.Filters.Add(new ExpeditionAttributeFilter(this));
      holder.Filters.Add(new ItemSetFilter(this));
      holder.Filters.Add(new ReleaseVersionsFilter(this));
      holder.Filters.Add(new RaritiesFilter(this));
      holder.Filters.Add(new EquippedFilter(this));
      holder.Filters.Add(new SearchTextFilter(this));
      holder.Filters.Add(new TargetBuildingFilter(this));
      return holder;
    }

    #endregion Public Methods
  }
}