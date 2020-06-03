using System.Collections;
using AssetViewer.Data;
using AssetViewer.Data.Filters;

namespace AssetViewer.Controls {

  public partial class ItemSets : ItemsBase {

    #region Public Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderItemSets();

    public IList SelectedItems { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public ItemSets() : base() {
      InitializeComponent();
    }

    #endregion Public Constructors

    #region Private Methods

    private void ListBoxItems_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e) {
      if (e.NewValue is TemplateAsset asset) {
        SelectedAsset = asset;
      }
    }

    #endregion Private Methods
  }
}