using AssetViewer.Controls;
using AssetViewer.Data;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AssetViewer {

  public partial class MainWindow : Window {

    #region Public Constructors

    public MainWindow() {
      this.InitializeComponent();
      this.DataContext = this;
      ComboBoxLanguage.SelectedItem = App.Language;
      ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_OnSelectionChanged;
    }

    #endregion Public Constructors

    #region Public Events

    public event Action<object, SelectionChangedEventArgs> OnLanguage_Changed;

    #endregion Public Events

    #region Private Methods

    private void MainWindow_OnLoaded(Object sender, RoutedEventArgs e) {
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
        }
      }
    }

    private void ComboBoxLanguage_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      if (ComboBoxLanguage.SelectedItem is Languages lang) {
        App.Language = lang;
      }
      App.LoadLanguageFile();
      DataContext = null;
      OnLanguage_Changed(sender, e);
      DataContext = this;
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    #endregion Private Methods
  }
}