using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ItemsHolderItemSets : ItemsHolder {

    #region Constructors

    public ItemsHolderItemSets() : base(AssetProvider.ItemSets.Values.ToList()) {
      StandardFilters.Add("Upgrades", new UpgradesFilter(this));
      StandardFilters.Add("Equipped", new EquippedFilter(this));
      StandardFilters.Add("SearchText", new SearchTextFilter(this));
      StandardFilters.Add("TargetBuilding", new TargetBuildingFilter(this));
    }

    #endregion Constructors

    #region Methods

    public override FilterHolder CreateFilterHolder() {
      var holder = new FilterHolder();
      holder.Filters.Add(new UpgradesFilter(this));
      holder.Filters.Add(new TargetsFilter(this));
      holder.Filters.Add(new ReleaseVersionsFilter(this));
      holder.Filters.Add(new EquippedFilter(this));
      holder.Filters.Add(new SearchTextFilter(this));
      holder.Filters.Add(new TargetBuildingFilter(this));
      return holder;
    }

    #endregion Methods
  }
}