using AssetViewer.Data.Filters;

namespace AssetViewer.Controls {

  public partial class AllBuffs : ItemsBase {

    #region Public Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderAllBuffs();

    #endregion Public Properties

    #region Public Constructors

    public AllBuffs() : base() {
      InitializeComponent();
    }

    #endregion Public Constructors
  }
}