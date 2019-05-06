using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using AssetViewer.Data;
using AssetViewer.Library;

namespace AssetViewer.Controls {

  public partial class GuildhouseItem : UserControl, INotifyPropertyChanged {

    #region Properties
    #endregion

    #region Constructor
    public GuildhouseItem() {
      this.InitializeComponent();
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.GuildhouseItem.xml")) {
        using (var reader = new StreamReader(stream)) {
          //this.AssetCategory = XDocument.Parse(reader.ReadToEnd()).Root;
        }
      }
      this.DataContext = this;
    }
    #endregion

    #region Private Methods
    private void GuildhouseItem_OnLoaded(Object sender, RoutedEventArgs e) {
      //this.ComboBoxAllocations.SelectedItem = this.ComboBoxAllocations.Items.OfType<Allocation>().Single(w => w.GUID == 0);
      this.ComboBoxTypes.SelectedIndex = 0;
      this.ComboBoxTargets.SelectedIndex = 0;
      this.ListBoxItems.SelectedIndex = 0;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
    }
    private void ComboBoxAllocations_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
    }
    private void ComboBoxTypes_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
    }
    private void ComboBoxTargets_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
    }
    private void CheckBoxFactoryUpgrades_OnChanged(Object sender, RoutedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      this.ListBoxItems.SelectedIndex = 0;
    }
    private void CheckBoxBuildingUpgrades_OnChanged(Object sender, RoutedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      this.ListBoxItems.SelectedIndex = 0;
    }
    private void ListBoxItems_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItem"));
      if (e.AddedItems.Count == 0) this.ListBoxItems.SelectedIndex = 0;
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
      this.ComboBoxAllocations.SelectedItem = null;
      this.ComboBoxTypes.SelectedItem = null;
      this.ComboBoxTargets.SelectedItem = null;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Descriptions"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Allocations"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Targets"));
      this.ComboBoxAllocations.SelectedIndex = 0;
      this.ComboBoxTypes.SelectedIndex = 0;
      this.ComboBoxTargets.SelectedIndex = 0;
    }
    private void CheckBoxPopulationUpgrade_OnChanged(Object sender, RoutedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      this.ListBoxItems.SelectedIndex = 0;
    }
    private void CheckBoxResidenceUpgrade_OnChanged(Object sender, RoutedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      this.ListBoxItems.SelectedIndex = 0;
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
  }

}