using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;
using AssetViewer.Templates;

namespace AssetViewer.Controls {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  public partial class ThirdParty : UserControl, INotifyPropertyChanged {

    #region Properties
    public IEnumerable<Asset> Items {
      get {
        //var rarity = this.ComboBoxRarities.SelectedItem as String;
        //var type = this.ComboBoxTypes.SelectedItem as String;
        //var target = this.ComboBoxTargets.SelectedItem as String;
        //var equipped = this.ComboBoxEquipped.SelectedItem as String;
        var result = this.Assets.AsQueryable();
        //if (!String.IsNullOrEmpty(type)) result = result.Where(w => w.ItemType == type);
        //switch (App.Language) {
        //  case Languages.German:
        //    if (!String.IsNullOrEmpty(rarity)) result = result.Where(w => w.Rarity.DE == rarity);
        //    if (!String.IsNullOrEmpty(target)) result = result.Where(w => w.EffectTargets.Select(s => s.DE).Contains(target));
        //    if (!String.IsNullOrEmpty(equipped)) result = result.Where(w => w.Allocation.Text.DE == equipped);
        //    result = result.OrderBy(o => o.Text.DE);
        //    break;
        //  default:
        //    if (!String.IsNullOrEmpty(rarity)) result = result.Where(w => w.Rarity.EN == rarity);
        //    if (!String.IsNullOrEmpty(target)) result = result.Where(w => w.EffectTargets.Select(s => s.EN).Contains(target));
        //    if (!String.IsNullOrEmpty(equipped)) result = result.Where(w => w.Allocation.Text.EN == equipped);
        //    result = result.OrderBy(o => o.Text.EN);
        //    break;
        //}
        //if (!String.IsNullOrEmpty(this.Search)) {
        //  result = result.Where(w => w.ID.StartsWith(this.Search, StringComparison.InvariantCultureIgnoreCase) || (App.Language == Languages.English ? w.Text.EN.ToLower().Contains(this.Search.ToLower()) : w.Text.DE.ToLower().Contains(this.Search.ToLower())));
        //}
        return result;
      }
    }
    #endregion

    #region Fields
    private readonly List<Asset> Assets;
    #endregion

    #region Constructor
    public ThirdParty() {
      this.InitializeComponent();
      this.Assets = new List<Asset>();
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ThirdParty.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.Assets.AddRange(document.XPathSelectElements("ThirdParty/OfferingItems/Asset").Select(s => new Asset(s)));
        }
      }
      this.DataContext = this;
    }
    #endregion

    #region Private Methods
    private void ThirdParty_OnLoaded(Object sender, RoutedEventArgs e) {
      this.ComboBoxCategories.SelectedIndex = 0;
    }
    private void ComboBoxLanguage_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Categories"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Events"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Thresholds"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rewards"));
      this.ComboBoxCategories.SelectedIndex = 0;
    }
    private void ComboBoxCategories_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Events"));
      this.ComboBoxEvents.SelectedIndex = 0;
    }
    private void ComboBoxEvents_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Thresholds"));
      this.ComboBoxThresholds.SelectedIndex = 0;
    }
    private void ComboBoxThresholds_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rewards"));
      this.ListBoxItems.SelectedIndex = 0;
    }
    private void ListBoxItems_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
  }

}