using AssetViewer.Data.Filters;

namespace AssetViewer.Controls {

  public partial class Buildings : ItemsBase {

    #region Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderBuildings();

    #endregion Properties

    #region Constructors

    public Buildings() : base() {
      this.InitializeComponent();
    }

    #endregion Constructors
  }
}