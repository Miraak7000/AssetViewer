using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using AssetViewer.Library;
using AssetViewer.Templates;

namespace AssetViewer.Controls {

  public partial class GuildhouseItem : UserControl, INotifyPropertyChanged {

    #region Properties
    public IEnumerable<TemplateAsset> Items {
      get {
        var rarity = this.ComboBoxRarities.SelectedItem as String;
        var type = this.ComboBoxTypes.SelectedItem as String;
        var target = this.ComboBoxTargets.SelectedItem as String;
        var equipped = this.ComboBoxEquipped.SelectedItem as String;
        var source = this.ComboBoxSources.SelectedItem as String;
        var upgrade = this.ComboBoxUpgrades.SelectedItem as String;
        var result = ItemProvider.Items.Values.AsQueryable();
        if (!String.IsNullOrEmpty(type))
          result = result.Where(w => w.ItemType == type);
        switch (App.Language) {
          case Languages.German:
            if (!String.IsNullOrEmpty(rarity))
              result = result.Where(w => w.Rarity.DE == rarity);
            if (!String.IsNullOrEmpty(target))
              result = result.Where(w => w.EffectTargets.Select(s => s.DE).Contains(target));
            if (!String.IsNullOrEmpty(equipped))
              result = result.Where(w => w.Allocation.Text.DE == equipped);
            if (!String.IsNullOrEmpty(upgrade))
              result = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text.DE == upgrade));
            if (!String.IsNullOrEmpty(source))
              result = result.Where(w => w.Sources != null && w.Sources.Any(l => l.Text.DE == source));
            result = result.OrderBy(o => o.Text.DE);
            break;
          default:
            if (!String.IsNullOrEmpty(rarity))
              result = result.Where(w => w.Rarity.EN == rarity);
            if (!String.IsNullOrEmpty(target))
              result = result.Where(w => w.EffectTargets.Select(s => s.EN).Contains(target));
            if (!String.IsNullOrEmpty(equipped))
              result = result.Where(w => w.Allocation.Text.EN == equipped);
            if (!String.IsNullOrEmpty(upgrade))
              result = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text.EN == upgrade));
            if (!String.IsNullOrEmpty(source))
              result = result.Where(w => w.Sources != null && w.Sources.Any(l => l.Text.EN == source));
            result = result.OrderBy(o => o.Text.EN);
            break;
        }
        if (!String.IsNullOrEmpty(this.Search)) {
          result = result.Where(w => w.ID.StartsWith(this.Search, StringComparison.InvariantCultureIgnoreCase) || (App.Language == Languages.English ? w.Text.EN.ToLower().Contains(this.Search.ToLower()) : w.Text.DE.ToLower().Contains(this.Search.ToLower())));
        }
        return result;
      }
    }
    public TemplateAsset SelectedAsset { get; set; }
    public IEnumerable<String> Rarities {
      get {
        switch (App.Language) {
          case Languages.German:
            return new[] {
              String.Empty,
              "Gewöhnlich",
              "Ungewöhnlich",
              "Selten",
              "Episch",
              "Legendär"
            };
          default:
            return new[] {
              String.Empty,
              "Common",
              "Uncommon",
              "Rare",
              "Epic",
              "Legendary"
            };
        }
      }
    }
    public IEnumerable<String> ItemTypes {
      get {
        var result = ItemProvider.Items.Values.Select(s => s.ItemType).Distinct().Where(l => !string.IsNullOrWhiteSpace(l)).OrderBy(o => o).ToList();
        result.Insert(0, String.Empty);
        return result;
      }
    }
    public IEnumerable<String> Targets {
      get {
        List<String> result;
        switch (App.Language) {
          case Languages.German:
            result = ItemProvider.Items.Values.SelectMany(s => s.EffectTargets).Select(s => s.DE).Distinct().OrderBy(o => o).ToList();
            break;
          default:
            result = ItemProvider.Items.Values.SelectMany(s => s.EffectTargets).Select(s => s.EN).Distinct().OrderBy(o => o).ToList();
            break;
        }
        result.Insert(0, String.Empty);
        return result;
      }
    }
    public IEnumerable<String> Equipped {
      get {
        List<String> result;
        switch (App.Language) {
          case Languages.German:
            result = ItemProvider.Items.Values.Select(s => s.Allocation?.Text.DE).Distinct().OrderBy(o => o).ToList();
            break;
          default:
            result = ItemProvider.Items.Values.Select(s => s.Allocation?.Text.EN).Distinct().OrderBy(o => o).ToList();
            break;
        }
        result.Insert(0, String.Empty);
        return result;
      }
    }
    public IEnumerable<String> Sources {
      get {
        List<String> result;
        switch (App.Language) {
          case Languages.German:
            result = ItemProvider.Items.Values.SelectMany(s => s.Sources).Select(s => s.Text.DE).Distinct().OrderBy(o => o).ToList();
            break;
          default:
            result = ItemProvider.Items.Values.SelectMany(s => s.Sources).Select(s => s.Text.EN).Distinct().OrderBy(o => o).ToList();
            break;
        }
        result.Insert(0, String.Empty);
        return result;
      }
    }
    public IEnumerable<String> Upgrades {
      get {
        List<String> result;
        switch (App.Language) {
          case Languages.German:
            result = ItemProvider.Items.Values.SelectMany(s => s.AllUpgrades).Select(s => s.Text.DE).Distinct().OrderBy(o => o).ToList();
            break;
          default:
            result = ItemProvider.Items.Values.SelectMany(s => s.AllUpgrades).Select(s => s.Text.EN).Distinct().OrderBy(o => o).ToList();
            break;
        }
        result.Insert(0, String.Empty);
        return result;
      }
    }
    public String SearchText {
      get { return this.Search; }
      set {
        this.Search = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      }
    }
    #endregion

    #region Fields
    private String Search = String.Empty;
    #endregion

    #region Constructor
    public GuildhouseItem() {
      this.InitializeComponent();
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      this.DataContext = this;
    }
    #endregion

    #region Private Methods
    private void GuildhouseItem_OnLoaded(Object sender, RoutedEventArgs e) {
      this.ComboBoxRarities.SelectedIndex = 0;
      this.ComboBoxTypes.SelectedIndex = 0;
      this.ComboBoxTargets.SelectedIndex = 0;
      this.ComboBoxEquipped.SelectedIndex = 0;
      this.ListBoxItems.SelectedIndex = 0;
    }
    private void ComboBoxRarities_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
    }
    private void ComboBoxTypes_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
    }
    private void ComboBoxTargets_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
    }
    private void ComboBoxEquipped_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
    }
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
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rarities"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemTypes"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Targets"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Equipped"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedAsset"));
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
  }

}