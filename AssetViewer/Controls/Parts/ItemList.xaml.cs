using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AssetViewer.Data;
using VAV;

namespace AssetViewer.Controls {

  public partial class ItemList : UserControl {

    #region Public Properties

    public IEnumerable ItemSource {
      get => (IEnumerable)GetValue(ItemSourceProperty);
      set => SetValue(ItemSourceProperty, value);
    }

    public int SelectedIndex {
      get => (int)GetValue(SelectedIndexProperty);
      set => SetValue(SelectedIndexProperty, value);
    }
    public ICommand CopyToClipboardCommand {
      get => (ICommand)GetValue(CopyToClipboardCommandProperty);
      set => SetValue(CopyToClipboardCommandProperty, value);
    }
    public IList SelectedItems => ListBoxItems.SelectedItems;

    #endregion Public Properties

    #region Public Constructors

    public ItemList() {
      InitializeComponent();
      CopyToClipboardCommand = new RelayCommand(OnCopyToClipboard);
    }

    #endregion Public Constructors

    #region Public Fields

    public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register(nameof(ItemSource), typeof(IEnumerable), typeof(ItemList), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ItemList), new PropertyMetadata(0));

    public static readonly DependencyProperty CopyToClipboardCommandProperty =
    DependencyProperty.Register("CopyToClipboardCommand", typeof(ICommand), typeof(ItemList), new PropertyMetadata(null));

    #endregion Public Fields

    #region Private Methods

    private void OnCopyToClipboard() {
      Clipboard.SetText(string.Join((","), ListBoxItems.Items.OfType<TemplateAsset>().Select(a => $"{a.ID} {a.Text.CurrentLang}")));
    }

    #endregion Private Methods
  }
}