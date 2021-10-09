using AssetViewer.Data.Filters;

namespace AssetViewer.Controls {

  public partial class GuildhouseItem : ItemsBase {

    #region Public Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderGuildhouse();

    #endregion Public Properties

    #region Public Constructors

    public GuildhouseItem() : base() {
      InitializeComponent();
    }

    #endregion Public Constructors
  }
}