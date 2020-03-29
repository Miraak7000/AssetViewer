using AssetViewer.Data.Filters;
using AssetViewer.Data;

namespace AssetViewer.Controls {

  public partial class ItemSets : ItemsBase {

    #region Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderItemSets();

    #endregion Properties

    #region Constructors

    public ItemSets() : base() {
      this.InitializeComponent();
    }

    #endregion Constructors

    #region Methods

    private void ListBoxItems_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e) {
      if (e.NewValue is TemplateAsset asset) {
        SelectedAsset = asset;
      }
    }

    #endregion Methods
  }
}