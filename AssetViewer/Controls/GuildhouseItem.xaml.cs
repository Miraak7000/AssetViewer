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

    #region Public Methods

    public override void ComboBoxLanguage_SelectionChanged() {
      base.ComboBoxLanguage_SelectionChanged();
      DataContext = null;
      DataContext = this;
    }

    #endregion Public Methods

  }
}