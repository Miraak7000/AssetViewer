using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AssetViewer {

  public static class TreeViewMultipleSelection {

    #region Public Methods

    public static bool GetIsMultipleSelection(TreeView element) {
      return (bool)element.GetValue(IsMultipleSelectionProperty);
    }

    public static void SetIsMultipleSelection(TreeView element, bool value) {
      element.SetValue(IsMultipleSelectionProperty, value);
    }

    public static bool GetIsItemSelected(TreeViewItem element) {
      return (bool)element.GetValue(IsItemSelectedProperty);
    }

    public static void SetIsItemSelected(TreeViewItem element, bool value) {
      element.SetValue(IsItemSelectedProperty, value);
    }

    public static IList GetSelectedItems(TreeView element) {
      return (IList)element.GetValue(SelectedItemsProperty);
    }

    public static void SetSelectedItems(TreeView element, IList value) {
      element.SetValue(SelectedItemsProperty, value);
    }

    #endregion Public Methods

    #region Public Fields

    public static readonly DependencyProperty IsMultipleSelectionProperty =
                               DependencyProperty.RegisterAttached(
           "IsMultipleSelection",
           typeof(bool),
           typeof(TreeViewMultipleSelection),
           new PropertyMetadata(false, OnMultipleSelectionPropertyChanged));

    public static readonly DependencyProperty IsItemSelectedProperty =
        DependencyProperty.RegisterAttached(
            "IsItemSelected",
            typeof(bool),
            typeof(TreeViewMultipleSelection),
            new PropertyMetadata(false, OnIsItemSelectedPropertyChanged));

    public static readonly DependencyProperty SelectedItemsProperty =
       DependencyProperty.RegisterAttached(
           "SelectedItems",
           typeof(IList),
           typeof(TreeViewMultipleSelection),
           new PropertyMetadata());

    #endregion Public Fields

    #region Private Methods

    private static void OnMultipleSelectionPropertyChanged(DependencyObject d,
                                                             DependencyPropertyChangedEventArgs e) {
      if (d is TreeView treeView && e.NewValue is bool) {
        if ((bool)e.NewValue) {
          treeView.AddHandler(TreeViewItem.MouseLeftButtonDownEvent,
            new MouseButtonEventHandler(OnTreeViewItemClicked), true);
        }
        else {
          treeView.RemoveHandler(TreeViewItem.MouseLeftButtonDownEvent,
               new MouseButtonEventHandler(OnTreeViewItemClicked));
        }
      }
    }

    private static void OnTreeViewItemClicked(object sender, MouseButtonEventArgs e) {
      var treeViewItem = FindTreeViewItem(
                                      e.OriginalSource as DependencyObject);

      if (treeViewItem != null && sender is TreeView treeView) {
        if (Keyboard.Modifiers == ModifierKeys.Control) {
          SelectMultipleItemsRandomly(treeView, treeViewItem);
        }
        else if (Keyboard.Modifiers == ModifierKeys.Shift) {
          SelectMultipleItemsContinuously(treeView, treeViewItem);
        }
        else {
          SelectSingleItem(treeView, treeViewItem);
        }
      }
    }

    private static TreeViewItem FindTreeViewItem(DependencyObject dependencyObject) {
      if (dependencyObject == null) {
        return null;
      }

      if (dependencyObject is TreeViewItem treeViewItem) {
        return treeViewItem;
      }

      return FindTreeViewItem(VisualTreeHelper.GetParent(dependencyObject));
    }

    private static void SelectSingleItem(TreeView treeView,
                                                   TreeViewItem treeViewItem) {
      // first deselect all items
      DeSelectAllItems(treeView, null);
      SetIsItemSelected(treeViewItem, true);
      SetStartItem(treeView, treeViewItem);
    }

    private static void DeSelectAllItems(TreeView treeView,
                                                TreeViewItem treeViewItem) {
      if (treeView != null) {
        for (var i = 0; i < treeView.Items.Count; i++) {
          if (treeView.ItemContainerGenerator.
                                     ContainerFromIndex(i) is TreeViewItem item) {
            SetIsItemSelected(item, false);
            DeSelectAllItems(null, item);
          }
        }
      }
      else {
        for (var i = 0; i < treeViewItem.Items.Count; i++) {
          if (treeViewItem.ItemContainerGenerator.
                                     ContainerFromIndex(i) is TreeViewItem item) {
            SetIsItemSelected(item, false);
            DeSelectAllItems(null, item);
          }
        }
      }
    }

    private static void OnIsItemSelectedPropertyChanged(DependencyObject d,
                                           DependencyPropertyChangedEventArgs e) {
      var treeViewItem = d as TreeViewItem;
      var treeView = FindTreeView(treeViewItem);
      if (treeViewItem != null && treeView != null) {
        var selectedItems = GetSelectedItems(treeView);
        if (selectedItems != null) {
          if (GetIsItemSelected(treeViewItem)) {
            selectedItems.Add(treeViewItem.Header);
          }
          else {
            selectedItems.Remove(treeViewItem.Header);
          }
        }
      }
    }

    private static TreeView FindTreeView(DependencyObject dependencyObject) {
      if (dependencyObject == null) {
        return null;
      }

      if (dependencyObject is TreeView treeView) {
        return treeView;
      }

      return FindTreeView(VisualTreeHelper.GetParent(dependencyObject));
    }

    private static TreeViewItem GetStartItem(TreeView element) {
      return (TreeViewItem)element.GetValue(StartItemProperty);
    }

    private static void SetStartItem(TreeView element, TreeViewItem value) {
      element.SetValue(StartItemProperty, value);
    }

    private static void SelectMultipleItemsRandomly(TreeView treeView,
                                                   TreeViewItem treeViewItem) {
      SetIsItemSelected(treeViewItem, !GetIsItemSelected(treeViewItem));
      if (GetStartItem(treeView) == null) {
        if (GetIsItemSelected(treeViewItem)) {
          SetStartItem(treeView, treeViewItem);
        }
      }
      else {
        if ((GetSelectedItems(treeView)?.Count ?? 0) == 0) {
          SetStartItem(treeView, null);
        }
      }
    }

    private static void SelectMultipleItemsContinuously(TreeView treeView,
                                                    TreeViewItem treeViewItem) {
      var startItem = GetStartItem(treeView);
      if (startItem != null) {
        if (startItem == treeViewItem) {
          SelectSingleItem(treeView, treeViewItem);
          return;
        }

        ICollection<TreeViewItem> allItems = new List<TreeViewItem>();
        GetAllItems(treeView, null, allItems);
        DeSelectAllItems(treeView, null);
        var isBetween = false;
        foreach (var item in allItems) {
          if (item == treeViewItem || item == startItem) {
            // toggle to true if first element is found and back to false if last element is found
            isBetween = !isBetween;

            // set boundary element
            SetIsItemSelected(item, true);
            continue;
          }

          if (isBetween) {
            SetIsItemSelected(item, true);
          }
        }
      }
    }

    private static void GetAllItems(TreeView treeView, TreeViewItem treeViewItem,
                                   ICollection<TreeViewItem> allItems) {
      if (treeView != null) {
        for (var i = 0; i < treeView.Items.Count; i++) {
          if (treeView.ItemContainerGenerator.
                                     ContainerFromIndex(i) is TreeViewItem item) {
            allItems.Add(item);
            GetAllItems(null, item, allItems);
          }
        }
      }
      else {
        for (var i = 0; i < treeViewItem.Items.Count; i++) {
          if (treeViewItem.ItemContainerGenerator.
                                     ContainerFromIndex(i) is TreeViewItem item) {
            allItems.Add(item);
            GetAllItems(null, item, allItems);
          }
        }
      }
    }

    #endregion Private Methods

    #region Private Fields

    private static readonly DependencyProperty StartItemProperty =
                                                       DependencyProperty.RegisterAttached(
           "StartItem",
           typeof(TreeViewItem),
           typeof(TreeViewMultipleSelection),
           new PropertyMetadata());

    #endregion Private Fields
  }
}