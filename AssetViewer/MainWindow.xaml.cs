using AssetViewer.Controls;
using AssetViewer.Library;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AssetViewer {

  public partial class MainWindow : Window {
    #region Constructors

    public MainWindow() {
      this.InitializeComponent();
      this.DataContext = this;
      if (App.Language == Languages.German) {
        ComboBoxLanguage.SelectedIndex = 1;
      }
      ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_OnSelectionChanged;
    }

    #endregion Constructors

    #region Events


    #endregion Events

    #region Methods

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
        }
      }
    }
    private void ComboBoxLanguage_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      switch (this.ComboBoxLanguage.SelectedIndex) {
        case 0:
          App.Language = Languages.English;
          break;

        case 1:
          App.Language = Languages.German;
          break;
        case 2:
          App.Language = Languages.Korean;
          break;
      }
      App.LoadLanguageFile();
      DataContext = null;
      DataContext = this;
    }

    #endregion Methods
  }
}