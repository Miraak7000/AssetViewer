using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ItemsHolderCityFestival : ItemsHolder {

    #region Public Constructors

    public ItemsHolderCityFestival() : base(AssetProvider.FestivalBuffs.Values.ToList()) {
      StandardFilters.Add("Upgrades", new UpgradesFilter(this));
      StandardFilters.Add("SearchText", new SearchTextFilter(this));
      StandardFilters.Add("TargetBuilding", new TargetBuildingFilter(this));
    }

    #endregion Public Constructors

    #region Public Methods

    public override FilterHolder CreateFilterHolder() {
      var holder = new FilterHolder();
      holder.Filters.Add(new UpgradesFilter(this));
      holder.Filters.Add(new TargetsFilter(this));
      holder.Filters.Add(new ReleaseVersionsFilter(this));
      holder.Filters.Add(new SearchTextFilter(this));
      holder.Filters.Add(new TargetBuildingFilter(this));
      return holder;
    }

    #endregion Public Methods
  }
}