using AssetViewer.Data.Filters;

namespace AssetViewer.Controls {

  public partial class Buildings : ItemsBase {

    #region Public Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderBuildings();

    #endregion Public Properties

    #region Public Constructors

    public Buildings() : base() {
      InitializeComponent();
    }

    #endregion Public Constructors
  }
}