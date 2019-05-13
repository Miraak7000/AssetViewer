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
  public partial class Monument : UserControl, INotifyPropertyChanged {

    #region Properties
    public IEnumerable<Asset> Categories {
      get { return this.AssetCategory.AsEnumerable(); }
    }
    public IEnumerable<Asset> Events {
      get {
        var monumentCategory = this.ComboBoxCategories.SelectedItem as Asset;
        if (monumentCategory == null) return null;
        return this.AssetEvent.Where(w => monumentCategory.MonumentEvents.Contains(w.ID));
      }
    }
    public IEnumerable<Asset> Thresholds {
      get {
        var monumentEvent = this.ComboBoxEvents.SelectedItem as Asset;
        if (monumentEvent == null) return null;
        return this.AssetReward.Where(w => monumentEvent.MonumentThresholds.Contains(w.ID));
      }
    }
    public IEnumerable<SelectableItem> Rewards {
      get {
        return null;
        //if (this.ComboBoxThresholds.SelectedItem == null) return null;
        //var assetThreshold = ((SelectableItem)this.ComboBoxThresholds.SelectedItem).GUID;
        //return this.AssetReward.XPathSelectElements($"Asset/Values/Standard[GUID={assetThreshold}]/../Reward/Asset")
        //  .Select(s => new SelectableItem {
        //    GUID = s.XPathSelectElement("Values/Standard/GUID").Value,
        //    Value = s.XPathSelectElement("Values/Description"),
        //    IconFilename = s.XPathSelectElement("Values/Standard/IconFilename").Value
        //  });
      }
    }
    #endregion

    #region Fields
    private readonly List<Asset> AssetCategory;
    private readonly List<Asset> AssetEvent;
    private readonly List<Asset> AssetReward;
    #endregion

    #region Constructor
    public Monument() {
      this.InitializeComponent();
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      this.AssetCategory = new List<Asset>();
      this.AssetEvent = new List<Asset>();
      this.AssetReward = new List<Asset>();
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentEventCategory.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.AssetCategory.AddRange(document.Elements().Select(s => new Asset(s)));
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentEvent.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.AssetEvent.AddRange(document.Elements().Select(s => new Asset(s)));
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentEventReward.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.AssetReward.AddRange(document.Elements().Select(s => new Asset(s)));
        }
      }
      this.DataContext = this;
    }
    #endregion

    #region Private Methods
    private void Monument_OnLoaded(Object sender, RoutedEventArgs e) {
      this.ComboBoxCategories.SelectedIndex = 0;
    }
    private void ComboBoxLanguage_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.ComboBoxCategories.SelectedItem = null;
      this.ComboBoxEvents.SelectedItem = null;
      this.ComboBoxThresholds.SelectedItem = null;
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
      if (this.ListBoxItems.SelectedItem == null) {
        this.Presenter.Content = null;
        return;
      }
      var selectedItem = (SelectableItem)this.ListBoxItems.SelectedItem;
      //var reward = this.AssetReward.XPathSelectElement($"Asset/Values/Reward/Asset/Values/Standard[GUID={selectedItem.GUID}]/../..");
      //switch (reward.XPathSelectElement("Template").Value) {
      //  case "BuildPermitBuilding":
      //    this.Presenter.Content = new TemplateBuildPermitBuilding(reward);
      //    break;
      //  case "GuildhouseItem":
      //    this.Presenter.Content = new TemplateGuildhouseItem(reward);
      //    break;
      //  default:
      //    this.Presenter.Content = new TemplateGuildhouseItem(reward);
      //    //this.Presenter.Content = null;
      //    break;
      //}
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
  }

}