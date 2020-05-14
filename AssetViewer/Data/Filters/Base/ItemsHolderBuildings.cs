using System.Linq;

namespace AssetViewer.Data.Filters {

  public class ItemsHolderBuildings : ItemsHolder {

    #region Public Constructors

    public ItemsHolderBuildings() : base(AssetProvider.Buildings.Values.ToList()) {
      StandardFilters.Add("Upgrades", new UpgradesFilter(this));
      StandardFilters.Add("Available", new BuildableFilter(this) { SelectedValue = true });
      StandardFilters.Add("SearchText", new SearchTextFilter(this));
    }

    #endregion Public Constructors

    #region Public Methods

    public override FilterHolder CreateFilterHolder() {
      var holder = new FilterHolder();
      holder.Filters.Add(new UpgradesFilter(this));
      holder.Filters.Add(new ReleaseVersionsFilter(this));
      holder.Filters.Add(new SearchTextFilter(this));
      holder.Filters.Add(new BuildableFilter(this));
      return holder;
    }

    #endregion Public Methods
  }
}