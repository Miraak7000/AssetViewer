using AssetViewer.Library;
using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssetViewer.Controls {

  public partial class GuildhouseItem : UserControl, INotifyPropertyChanged {

    #region Properties

    public void SetItems() {
      var rarity = this.ComboBoxRarities.SelectedItem as String;
      var type = this.ComboBoxTypes.SelectedItem as String;
      var target = this.ComboBoxTargets.SelectedItem as String;
      var equipped = this.ComboBoxEquipped.SelectedItem as String;
      var source = this.ComboBoxSources.SelectedItem as String;
      var upgrade = this.ComboBoxUpgrades.SelectedItem as String;
      var release = this.ComboBoxReleases.SelectedItem as String;
      var detailedSources = this.ComboBoxDetailedSources.SelectedItem as string;
      var result = ItemProvider.Items.Values.AsQueryable();
      if (!String.IsNullOrEmpty(type))
        result = result.Where(w => w.ItemType == type);
      switch (App.Language) {
        case Languages.German:
          if (!String.IsNullOrEmpty(rarity))
            result = result.Where(w => w.Rarity.DE == rarity);
          if (!String.IsNullOrEmpty(target))
            result = result.Where(w => w.EffectTargets != null && w.EffectTargets.Select(s => s.DE).Contains(target));
          if (!String.IsNullOrEmpty(equipped))
            result = result.Where(w => w.Allocation != null && w.Allocation.Text.DE == equipped);
          if (!String.IsNullOrEmpty(upgrade))
            result = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text.DE == upgrade));
          if (!String.IsNullOrEmpty(source))
            result = result.Where(w => w.Sources != null && w.Sources.Any(l => l.Text.DE == source));
          if (!String.IsNullOrEmpty(detailedSources))
            result = result.Where(w => w.Sources != null && w.Sources.SelectMany(s => s.Additionals).Any(l => l.Text.DE == detailedSources));
          if (!String.IsNullOrEmpty(release))
            result = result.Where(w => w.ReleaseVersion == release);
          result = result.OrderBy(o => o.Text.DE);
          break;

        default:
          if (!String.IsNullOrEmpty(rarity))
            result = result.Where(w => w.Rarity.EN == rarity);
          if (!String.IsNullOrEmpty(target))
            result = result.Where(w => w.EffectTargets != null && w.EffectTargets.Select(s => s.EN).Contains(target));
          if (!String.IsNullOrEmpty(equipped))
            result = result.Where(w => w.Allocation != null && w.Allocation.Text.EN == equipped);
          if (!String.IsNullOrEmpty(upgrade))
            result = result.Where(w => w.AllUpgrades != null && w.AllUpgrades.Any(l => l.Text != null && l.Text.EN == upgrade));
          if (!String.IsNullOrEmpty(source))
            result = result.Where(w => w.Sources != null && w.Sources.Any(l => l.Text.EN == source));
          if (!String.IsNullOrEmpty(detailedSources))
            result = result.Where(w => w.Sources != null && w.Sources.SelectMany(s => s.Additionals).Any(l => l.Text.EN == detailedSources));
          if (!String.IsNullOrEmpty(release))
            result = result.Where(w => w.ReleaseVersion == release);
          result = result.OrderBy(o => o.Text.EN);
          break;
      }
      if (!String.IsNullOrEmpty(this.Search)) {
        result = result.Where(w => w.ID.StartsWith(this.Search, StringComparison.InvariantCultureIgnoreCase) || (App.Language == Languages.English ? w.Text.EN.ToLower().Contains(this.Search.ToLower()) : w.Text.DE.ToLower().Contains(this.Search.ToLower())));
      }
      Items = result.ToList();
    }

    public List<TemplateAsset> Items { get; set; }
    private bool IsRefreshingUi;
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
          //var result = Items.Select(s => s.Rarity.DE).Distinct().Where(l => !string.IsNullOrWhiteSpace(l)).OrderBy(o => o).ToList();
          //result.Insert(0, String.Empty);
          //return result;
          default:
            return new[] {
              String.Empty,
              "Common",
              "Uncommon",
              "Rare",
              "Epic",
              "Legendary"
            };
            //result = Items.Select(s => s.Rarity.EN).Distinct().Where(l => !string.IsNullOrWhiteSpace(l)).OrderBy(o => o).ToList();
            //result.Insert(0, String.Empty);
            //return result;
        }
      }
    }

    public IEnumerable<String> ItemTypes {
      get {
        var result = Items.Select(s => s.ItemType).Distinct().Where(l => !string.IsNullOrWhiteSpace(l)).OrderBy(o => o).ToList();
        result.Insert(0, String.Empty);
        return result;
      }
    }

    public IEnumerable<String> ReleaseVersions {
      get {
        var result = Items.Select(s => s.ReleaseVersion).Distinct().OrderBy(o => o).ToList();
        result.Insert(0, String.Empty);
        return result;
      }
    }

    public IEnumerable<String> Targets {
      get {
        List<String> result;
        switch (App.Language) {
          case Languages.German:
            result = Items.SelectMany(s => s.EffectTargets).Select(s => s.DE).Distinct().OrderBy(o => o).ToList();
            break;

          default:
            result = Items.SelectMany(s => s.EffectTargets).Select(s => s.EN).Distinct().OrderBy(o => o).ToList();
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
            result = Items.Select(s => s.Allocation?.Text.DE).Distinct().OrderBy(o => o).ToList();
            break;

          default:
            result = Items.Select(s => s.Allocation?.Text.EN).Distinct().OrderBy(o => o).ToList();
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
            result = Items.SelectMany(s => s.Sources).Select(s => s.Text.DE).Distinct().OrderBy(o => o).ToList();
            break;

          default:
            result = Items.SelectMany(s => s.Sources).Select(s => s.Text.EN).Distinct().OrderBy(o => o).ToList();
            break;
        }
        result.Insert(0, String.Empty);
        return result;
      }
    }

    public IEnumerable<String> DetailedSources {
      get {
        List<String> result;
        switch (App.Language) {
          case Languages.German:
            result = Items.SelectMany(s => s.Sources).SelectMany(s => s.Additionals).Select(s => s.Text.DE).Distinct().OrderBy(o => o).ToList();
            break;

          default:
            result = Items.SelectMany(s => s.Sources).SelectMany(s => s.Additionals).Select(s => s.Text.EN).Distinct().OrderBy(o => o).ToList();
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
            result = Items.SelectMany(s => s.AllUpgrades).Select(s => s.Text?.DE).Distinct().OrderBy(o => o).ToList();
            break;

          default:
            result = Items.SelectMany(s => s.AllUpgrades).Select(s => s.Text?.EN).Distinct().OrderBy(o => o).ToList();
            break;
        }
        result.Insert(0, String.Empty);
        return result;
      }
    }

    public String SearchText {
      get {
        return this.Search;
      }
      set {
        this.Search = value;
        UpdateUI();
      }
    }

    #endregion Properties

    #region Constructors

    public GuildhouseItem() {
      this.InitializeComponent();
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      SetItems();
      this.DataContext = this;
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Fields


    private String Search = String.Empty;

    #endregion Fields

    #region Methods
    private void BtnResetFilters_Click(object sender, RoutedEventArgs e) {
      IsRefreshingUi = true;
      this.ComboBoxRarities.SelectedItem = string.Empty;
      this.ComboBoxTypes.SelectedItem = string.Empty;
      this.ComboBoxTargets.SelectedItem = string.Empty;
      this.ComboBoxEquipped.SelectedItem = string.Empty;
      this.ComboBoxSources.SelectedItem = string.Empty;
      this.ComboBoxUpgrades.SelectedItem = string.Empty;
      this.ComboBoxReleases.SelectedItem = string.Empty;
      this.ComboBoxDetailedSources.SelectedItem = string.Empty;
      IsRefreshingUi = false;
      this.SearchText = string.Empty;
    }
    private string[] Filters = new[]{
      nameof(Rarities),
      nameof(ItemTypes),
      nameof(ReleaseVersions),
      nameof(Targets),
      nameof(Equipped),
      nameof(Sources),
      nameof(DetailedSources),
      nameof(Upgrades),
      nameof(SearchText)
    };
    private void UpdateUI() {
      if (!IsRefreshingUi) {
        SetItems();
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        foreach (var filter in Filters) {
          this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(filter));
        }
      }
    }
    private void GuildhouseItem_OnLoaded(Object sender, RoutedEventArgs e) {
      IsRefreshingUi = true;
      this.ComboBoxRarities.SelectedIndex = 0;
      this.ComboBoxTypes.SelectedIndex = 0;
      this.ComboBoxTargets.SelectedIndex = 0;
      this.ComboBoxEquipped.SelectedIndex = 0;
      this.ComboBoxSources.SelectedItem = 0;
      this.ComboBoxUpgrades.SelectedItem = 0;
      this.ComboBoxReleases.SelectedItem = 0;
      this.ComboBoxDetailedSources.SelectedItem = 0;
      this.ListBoxItems.SelectedIndex = 0;
      IsRefreshingUi = false;
    }
    private void ComboBoxFilter_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      UpdateUI();
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
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
    }


    #endregion Methods
  }
}