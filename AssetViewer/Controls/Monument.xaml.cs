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
    public IEnumerable<SelectableItem> Categories {
      get {
        return this.AssetCategory.XPathSelectElements("Asset/Values")
          .Select(s => new SelectableItem {
            GUID = s.XPathSelectElement("Standard/GUID").Value,
            Value = s.XPathSelectElement("Description"),
            IconFilename = s.XPathSelectElement("Standard/IconFilename").Value
          });
      }
    }
    public IEnumerable<SelectableItem> Events {
      get {
        if (this.ComboBoxCategories.SelectedItem == null) return null;
        var assetCategory = ((SelectableItem)this.ComboBoxCategories.SelectedItem).GUID;
        var assetEvents = this.AssetCategory.XPathSelectElements($"Asset/Values/Standard[GUID={assetCategory}]/../MonumentEventCategory/Events/Item/Event").Select(s => s.Value).ToArray();
        return this.AssetEvent.Elements()
          .Where(w => assetEvents.Contains(w.XPathSelectElement("Values/Standard/GUID").Value))
          .Select(s => new SelectableItem {
            GUID = s.XPathSelectElement("Values/Standard/GUID").Value,
            Value = s.XPathSelectElement("Values/Description"),
            IconFilename = s.XPathSelectElement("Values/Standard/IconFilename").Value
          });
      }
    }
    public IEnumerable<SelectableItem> Thresholds {
      get {
        if (this.ComboBoxEvents.SelectedItem == null) return null;
        var assetEvent = ((SelectableItem)this.ComboBoxEvents.SelectedItem).GUID;
        var assetRewards = this.AssetEvent.XPathSelectElements($"Asset/Values/Standard[GUID={assetEvent}]/../MonumentEvent/RewardThresholds/Item/Reward").Select(s => s.Value).ToArray();
        return this.AssetReward.Elements()
          .Where(w => assetRewards.Contains(w.XPathSelectElement("Values/Standard/GUID").Value))
          .Select(s => new SelectableItem {
            GUID = s.XPathSelectElement("Values/Standard/GUID").Value,
            Value = s.XPathSelectElement("Values/Description"),
            IconFilename = s.XPathSelectElement("Values/Standard/IconFilename").Value
          });
      }
    }
    public IEnumerable<SelectableItem> Rewards {
      get {
        if (this.ComboBoxThresholds.SelectedItem == null) return null;
        var assetThreshold = ((SelectableItem)this.ComboBoxThresholds.SelectedItem).GUID;
        return this.AssetReward.XPathSelectElements($"Asset/Values/Standard[GUID={assetThreshold}]/../Reward/Asset")
          .Select(s => new SelectableItem {
            GUID = s.XPathSelectElement("Values/Standard/GUID").Value,
            Value = s.XPathSelectElement("Values/Description"),
            IconFilename = s.XPathSelectElement("Values/Standard/IconFilename").Value
          });
      }
    }
    #endregion

    #region Fields
    private readonly XElement AssetCategory;
    private readonly XElement AssetEvent;
    private readonly XElement AssetReward;
    #endregion

    #region Constructor
    public Monument() {
      this.InitializeComponent();
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.Assets_MonumentEventCategory.xml")) {
        using (var reader = new StreamReader(stream)) {
          this.AssetCategory = XDocument.Parse(reader.ReadToEnd()).Root;
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.Assets_MonumentEvent.xml")) {
        using (var reader = new StreamReader(stream)) {
          this.AssetEvent = XDocument.Parse(reader.ReadToEnd()).Root;
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.Assets_MonumentEventReward.xml")) {
        using (var reader = new StreamReader(stream)) {
          this.AssetReward = XDocument.Parse(reader.ReadToEnd()).Root;
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
      var reward = this.AssetReward.XPathSelectElement($"Asset/Values/Reward/Asset/Values/Standard[GUID={selectedItem.GUID}]/../..");
      switch (reward.XPathSelectElement("Template").Value) {
        case "BuildPermitBuilding":
          this.Presenter.Content = new TemplateBuildPermitBuilding(reward);
          break;
        case "GuildhouseItem":
          this.Presenter.Content = new TemplateGuildhouseItem(reward);
          break;
        default:
          this.Presenter.Content = new TemplateGuildhouseItem(reward);
          //this.Presenter.Content = null;
          break;
      }
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
  }

}