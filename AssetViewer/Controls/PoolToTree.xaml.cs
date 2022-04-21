using System.Windows;
using System.Windows.Controls;
using AssetViewer.Data;

namespace AssetViewer.Controls {

  public partial class PoolToTree : UserControl {

    #region Public Properties

    public bool IsExpanded { get; private set; }

    #endregion Public Properties

    #region Public Constructors

    public PoolToTree() {
      InitializeComponent();
      FillTreeView("190774");
    }

    #endregion Public Constructors

    #region Private Methods

    private void FillTreeView(string v) {
      if (AssetProvider.Pools.ContainsKey(v)) {
        var pool = AssetProvider.Pools[v];
        var tvi = new TreeViewItem {
          FontSize = 22,
          Header = pool.ID + " -  " + pool.Name
        };
        trvMenu.Items.Add(tvi);
        AddMore(pool, tvi);
      }
    }

    private void AddMore(Pool pool, TreeViewItem tvi) {
      foreach (var item in pool.Items) {
        if (item.Weight > 0) {
          var tvi2 = new TreeViewItem {
            FontSize = 22,
            Header = $"{item.Weight} - {item.ID} - {(item.Item is Pool p ? p.Name : (item.Item is TemplateAsset a ? a.Text.CurrentLang : null))}"
          };
          tvi.Items.Add(tvi2);
          if (item.Item is Pool po) {
            AddMore(po, tvi2);
          }
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (!IsExpanded) {
        foreach (var item in trvMenu.Items) {
          var treeItem = trvMenu.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
          if (treeItem != null) {
            ExpandAll(treeItem, true);
          }

          treeItem.IsExpanded = true;
          IsExpanded = true;
        }
      }
      else {
        foreach (var item in trvMenu.Items) {
          var treeItem = trvMenu.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
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

    #endregion Private Methods
  }
}