using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace AssetViewer.Controls {

  public partial class ItemList : UserControl {

    #region Properties

    public IEnumerable ItemSource {
      get { return (IEnumerable)GetValue(ItemSourceProperty); }
      set { SetValue(ItemSourceProperty, value); }
    }

    public int SelectedIndex {
      get { return (int)GetValue(SelectedIndexProperty); }
      set { SetValue(SelectedIndexProperty, value); }
    }

    #endregion Properties

    #region Fields

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(ItemList), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ItemList), new PropertyMetadata(0));

    #endregion Fields

    #region Constructors

    public ItemList() {
      InitializeComponent();
    }

    #endregion Constructors
  }
}