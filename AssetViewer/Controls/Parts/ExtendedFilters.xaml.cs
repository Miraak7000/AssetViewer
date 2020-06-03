using System;
using System.Windows;
using System.Windows.Controls;

namespace AssetViewer.Controls {

  /// <summary> Interaktionslogik für ExtendedFilters.xaml </summary>
  public partial class ExtendedFilters : UserControl {

    #region Public Events

    public event Action<object, RoutedEventArgs> AddFilter_Click;

    public event Action<object, RoutedEventArgs> RemoveFilter_Click;

    public event Action<object, EventArgs> SelectionChanged;

    #endregion Public Events

    #region Public Constructors

    public ExtendedFilters() {
      InitializeComponent();
    }

    #endregion Public Constructors

    #region Private Methods

    private void BtnAddFilter_Click(object sender, RoutedEventArgs e) {
      AddFilter_Click?.Invoke(sender, e);
    }

    private void ComboBox_SelectionChanged(object sender, EventArgs e) {
      SelectionChanged?.Invoke(sender, e);
    }

    private void BtnRemoveFilter_Click(object sender, RoutedEventArgs e) {
      RemoveFilter_Click?.Invoke(sender, e);
    }

    #endregion Private Methods
  }
}