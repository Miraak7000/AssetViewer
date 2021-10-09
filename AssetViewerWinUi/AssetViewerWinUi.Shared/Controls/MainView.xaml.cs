using System.Diagnostics;
using System.Windows;
using AssetViewer.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AssetViewer.Controls {

  /// <summary> Interaktionslogik für MainView.xaml </summary>
  public partial class MainView : UserControl {

    #region Public Constructors

    public MainView() {
      InitializeComponent();
      DataContext = this;
      ComboBoxLanguage.SelectedItem = AssetProvider.Language;
      ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_OnSelectionChanged;
    }

    #endregion Public Constructors

    #region Private Methods

    //private static void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
    //  Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
    //  e.Handled = true;
    //}

    private void UserControl_OnLoaded(object sender, RoutedEventArgs e) {
      ComboBoxAsset.SelectedIndex = 0;
    }

    private void ComboBoxAsset_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (Presenter != null) {
        switch (ComboBoxAsset.SelectedIndex) {
          case 0:
            Presenter.Content = new GuildhouseItem();
            break;

          case 1:
            Presenter.Content = new Monument();
            break;

          case 2:
            Presenter.Content = new ThirdParty();
            break;

          case 3:
            Presenter.Content = new ExpeditionEvents();
            break;

          case 4:
            Presenter.Content = new Tourism();
            break;

          case 5:
            Presenter.Content = new Buildings();
            break;

          case 6:
            Presenter.Content = new ItemSets();
            break;

          case 7:
            Presenter.Content = new CityFestival();
            break;

          case 8:
            Presenter.Content = new AllBuffs();
            break;

          case 9:
            Presenter.Content = new PoolToTree();
            break;

          default:
            break;
        }
      }
    }

    private void ComboBoxLanguage_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      DataContext = null;
      if (ComboBoxLanguage.SelectedItem is Languages lang) {
        AssetProvider.SetLanguage(lang);
      }
      DataContext = this;
    }

    #endregion Private Methods
  }
}