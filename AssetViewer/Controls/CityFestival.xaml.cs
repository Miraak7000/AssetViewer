using AssetViewer.Data.Filters;

namespace AssetViewer.Controls {

  public partial class CityFestival : ItemsBase {

    #region Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderCityFestival();

    #endregion Properties

    #region Constructors

    public CityFestival() : base() {
      this.InitializeComponent();
    }

    #endregion Constructors
  }
}