using AssetViewer.Data;
using AssetViewer.Data;
using System.Windows;
using System.Windows.Controls;

namespace AssetViewer.Controls {

  public partial class PoolToTree : UserControl {

    #region Properties

    public bool IsExpanded { get; private set; }

    #endregion Properties

    #region Constructors

    public PoolToTree() {
      InitializeComponent();
      FillTreeView(190774);
    }

    #endregion Constructors

    #region Methods

    private void FillTreeView(int v) {
      if (AssetProvider.Pools.ContainsKey(v)) {
        var pool = AssetProvider.Pools[v];
        var tvi = new TreeViewItem();
        tvi.FontSize = 22;
        tvi.Header = pool.ID + " -  " + pool.Name;
        trvMenu.Items.Add(tvi);
        AddMore(pool, tvi);
      }
    }

    private void AddMore(Pool pool, TreeViewItem tvi) {
      foreach (var item in pool.Items) {
        if (item.Weight > 0) {
          var tvi2 = new TreeViewItem();
          tvi2.FontSize = 22;
          tvi2.Header = $"{item.Weight} - {item.ID} - {(item.Item is Pool p ? p.Name : (item.Item is TemplateAsset a ? a.Text.CurrentLang : null))}";
          tvi.Items.Add(tvi2);
          if (item.Item is Pool po) {
            AddMore(po, tvi2);
          }
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (!IsExpanded) {
        foreach (var item in this.trvMenu.Items) {
          var treeItem = this.trvMenu.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
          if (treeItem != null) {
            ExpandAll(treeItem, true);
          }

          treeItem.IsExpanded = true;
          IsExpanded = true;
        }
      }
      else {
        foreach (var item in this.trvMenu.Items) {
          var treeItem = this.trvMenu.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
          if (treeItem != null) {
            ExpandAll(treeItem, false);
          }

          treeItem.IsExpanded = false;
          IsExpanded = false;
        }
      }
    }

    private void ExpandAll(ItemsControl items, bool expand) {
      foreach (var obj in items.Items) {
        var childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
        if (childControl != null) {
          ExpandAll(childControl, expand);
        }
        if (childControl is TreeViewItem tvi) {
          tvi.IsExpanded = true;
        }
      }
    }

    #endregion Methods
  }
}