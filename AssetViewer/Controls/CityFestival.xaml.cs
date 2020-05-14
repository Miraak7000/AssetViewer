using AssetViewer.Data.Filters;

namespace AssetViewer.Controls {

  public partial class CityFestival : ItemsBase {

    #region Public Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderCityFestival();

    #endregion Public Properties

    #region Public Constructors

    public CityFestival() : base() {
      InitializeComponent();
    }

    #endregion Public Constructors
  }
}