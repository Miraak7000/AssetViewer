using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using AssetViewer.Controls;
using AssetViewer.Library;

namespace AssetViewer {

  public partial class MainWindow : Window, INotifyPropertyChanged {

    #region Properties
    public Dictionary<Int32, Description> Descriptions {
      get { return App.Descriptions; }
    }
    #endregion

    #region Constructor
    public MainWindow() {
      this.InitializeComponent();
      this.DataContext = this;
    }
    #endregion

    #region Private Methods

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        e.Handled = true;
    }


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
      }
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Descriptions"));
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
  }

}