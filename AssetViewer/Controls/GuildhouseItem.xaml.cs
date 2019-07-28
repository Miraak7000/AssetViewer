using AssetViewer.Data;
using AssetViewer.Data.Filters;
using AssetViewer.Library;
using AssetViewer.Templates;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AssetViewer.Controls {

  public partial class GuildhouseItem : UserControl, INotifyPropertyChanged {

    #region Properties

    public TemplateAsset SelectedAsset { get; set; }

    public ItemsHolder ItemsHolder { get; } = new ItemsHolder();
    public Data.Description ResetButtonText => App.Descriptions[1100];
    public Data.Description AdvancedFiltersText => App.Descriptions[1104];

    #endregion Properties

    #region Constructors

    public GuildhouseItem() {
      this.InitializeComponent();
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      ItemsHolder.SetItems();
      this.DataContext = this;
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

    private void BtnResetFilters_Click(object sender, RoutedEventArgs e) => ItemsHolder.ResetFilters();
    private void GuildhouseItem_OnLoaded(Object sender, RoutedEventArgs e) {
      ItemsHolder.IsRefreshingUi = true;
      this.ComboBoxRarities.SelectedIndex = 0;
      //this.ComboBoxTypes.SelectedIndex = 0;
      this.ComboBoxTargets.SelectedIndex = 0;
      this.ComboBoxEquipped.SelectedIndex = 0;
      //this.ComboBoxSources.SelectedIndex = 0;
      this.ComboBoxUpgrades.SelectedIndex = 0;
      //this.ComboBoxReleases.SelectedIndex = 0;
      //this.ComboBoxDetailedSources.SelectedIndex = 0;
      this.ListBoxItems.SelectedIndex = 0;
      ItemsHolder.IsRefreshingUi = false;
    }
    //private void ComboBoxFilter_OnSelectionChanged(object sender, EventArgs e) {
    //  ItemsHolder.UpdateUI();
    //}
    private void ListBoxItems_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      if (e.AddedItems.Count == 0)
        this.ListBoxItems.SelectedIndex = 0;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedAsset"));
    }
    private void ComboBoxLanguage_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
      switch (((ComboBox)sender).SelectedIndex) {
        case 0:
          App.Language = Languages.English;
          break;

        case 1:
          App.Language = Languages.German;
          break;
      }
      ItemsHolder.RaiseLanguageChanged();
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
    }

    private void BtnAddFilter_Click(object sender, RoutedEventArgs e) {
      ItemsHolder.CustomFilters.Add(new FilterHolder(ItemsHolder));
    }

    private void BtnRemoveFilter_Click(object sender, RoutedEventArgs e) {
      var filter = (FilterHolder)(sender as Button).DataContext;
      if (ItemsHolder.CustomFilters.Contains(filter)) {
        ItemsHolder.CustomFilters.Remove(filter);
      }
    }

    private void ComboBox_SelectionChanged(object sender, EventArgs e) {
      ItemsHolder.UpdateUI();
    }

    #endregion Methods
  }
}