using AssetViewer.Data;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AssetViewer.Controls {

  /// <summary>
  /// Interaktionslogik für MainView.xaml
  /// </summary>
  public partial class MainView : UserControl {

    #region Public Constructors

    public MainView() {
      InitializeComponent();
      this.DataContext = this;
      ComboBoxLanguage.SelectedItem = AssetProvider.Language;
      ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_OnSelectionChanged;
    }

    #endregion Public Constructors

    #region Private Methods

    private void UserControl_OnLoaded(Object sender, RoutedEventArgs e) {
      this.ComboBoxAsset.SelectedIndex = 0;
    }

    private void ComboBoxAsset_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      if (this.Presenter != null) {
        switch (this.ComboBoxAsset.SelectedIndex) {
          case 0:
            this.Presenter.Content = new GuildhouseItem();
            break;

          case 1:
            this.Presenter.Content = new Monument();
            break;

          case 2:
            this.Presenter.Content = new ThirdParty();
            break;

          case 3:
            this.Presenter.Content = new ExpeditionEvents();
            break;

          case 4:
            this.Presenter.Content = new Tourism();
            break;

          case 5:
            this.Presenter.Content = new Buildings();
            break;

          case 6:
            this.Presenter.Content = new ItemSets();
            break;

          case 7:
            this.Presenter.Content = new CityFestival();
            break;

          case 8:
            this.Presenter.Content = new PoolToTree();
            break;
          default:
            break;
        }
      }
    }

    private void ComboBoxLanguage_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      DataContext = null;
      if (ComboBoxLanguage.SelectedItem is Languages lang) {
        AssetProvider.SetLanguage(lang);
      }
      DataContext = this;
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    #endregion Private Methods
  }
}