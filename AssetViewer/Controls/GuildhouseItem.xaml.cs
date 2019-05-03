using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AssetViewer.Data;
using AssetViewer.Data.GuildhouseItem;
using AssetViewer.Library;

namespace AssetViewer.Controls {

  public partial class GuildhouseItem : UserControl, INotifyPropertyChanged {

    public event PropertyChangedEventHandler PropertyChanged;

    #region Properties
    public Dictionary<Int32, Description> Descriptions {
      get { return App.Descriptions; }
    }
    public IEnumerable<Allocation> Allocations {
      get {
        var result = App.AssetGuildhouseItems.Select(s => s.Allocation).GroupBy(k => k.GUID).Select(s => s.First()).OrderBy(o => o.ToString()).ToList();
        return result;
      }
    }
    public IEnumerable<EffectTarget> Targets {
      get {
        var result = App.AssetGuildhouseItems.SelectMany(s => s.EffectTargets).GroupBy(k => k.GUID).Select(s => s.First()).OrderBy(o => o.ToString()).ToList();
        result.Insert(0, new EffectTarget());
        return result;
      }
    }
    public IEnumerable<AssetGuildhouseItem> Items {
      get {
        if (!this.IsLoaded || this.ComboBoxAllocations.SelectedItem == null) return null;
        var selectedAllocation = (Allocation)this.ComboBoxAllocations.SelectedItem;
        var selectedType = this.ComboBoxTypes.SelectedIndex;
        var selectedTarget = (EffectTarget)this.ComboBoxTargets.SelectedItem;
        var result = App.AssetGuildhouseItems.AsEnumerable();
        result = result.Where(w => w.Allocation.GUID == selectedAllocation.GUID);
        result = result.Where(w => w.IsSpecialist == (selectedType == 1));
        if (this.CheckBoxFactoryUpgrades.IsChecked.HasValue && this.CheckBoxFactoryUpgrades.IsChecked.Value) result = result.Where(w => w.HasFactoryUpgrade);
        if (this.CheckBoxBuildingUpgrades.IsChecked.HasValue && this.CheckBoxBuildingUpgrades.IsChecked.Value) result = result.Where(w => w.HasBuildingUpgrade);
        if (this.CheckBoxPopulationUpgrade.IsChecked.HasValue && this.CheckBoxPopulationUpgrade.IsChecked.Value) result = result.Where(w => w.HasPopulationUpgrade);
        if (this.CheckBoxResidenceUpgrade.IsChecked.HasValue && this.CheckBoxResidenceUpgrade.IsChecked.Value) result = result.Where(w => w.HasResidenceUpgrade);
        if (selectedTarget != null && selectedTarget.GUID != 0) result = result.Where(w => w.EffectTargets.Select(s => s.GUID).Contains(selectedTarget.GUID));
        return result.OrderBy(o => o.ToString());
      }
    }
    public AssetGuildhouseItem SelectedItem { get; set; }
    public Boolean HasResult {
      get { return this.ListBoxItems.Items.Count > 0; }
    }
    #endregion

    #region Constructor
    public GuildhouseItem() {
      this.InitializeComponent();
      this.DataContext = this;
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
    }
    #endregion

    #region Private Methods
    private void GuildhouseItem_OnLoaded(Object sender, RoutedEventArgs e) {
      this.ComboBoxAllocations.SelectedItem = this.ComboBoxAllocations.Items.OfType<Allocation>().Single(w => w.GUID == 0);
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

  }

}