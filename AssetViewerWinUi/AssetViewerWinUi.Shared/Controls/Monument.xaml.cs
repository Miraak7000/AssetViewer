using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using AssetViewer.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AssetViewer.Controls {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  public partial class Monument : UserControl, INotifyPropertyChanged {

    #region Public Properties

    public IEnumerable<TemplateAsset> Categories => AssetCategory.AsEnumerable();

    public IEnumerable<TemplateAsset> Events {
      get {
        if (!(ComboBoxCategories.SelectedItem is TemplateAsset monumentCategory)) {
          return Array.Empty<TemplateAsset>();
        }

        return AssetEvent.Where(w => monumentCategory.MonumentEvents.Contains(w.ID));
      }
    }

    public IEnumerable<TemplateAsset> Thresholds {
      get {
        if (!(ComboBoxEvents.SelectedItem is TemplateAsset monumentEvent)) {
          return Array.Empty<TemplateAsset>();
        }

        return AssetThreshold.Where(w => monumentEvent.MonumentThresholds.Contains(w.ID));
      }
    }

    public IEnumerable<TemplateAsset> Rewards {
      get {
        if (!(ComboBoxThresholds.SelectedItem is TemplateAsset monumentThreshold)) {
          return Array.Empty<TemplateAsset>();
        }

        return monumentThreshold.MonumentRewards.GetItemsById();
      }
    }

    public TemplateAsset SelectedAsset {
      get => selectedAsset;
      set {
        if (selectedAsset != value) {
          selectedAsset = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAsset)));
        }
      }
    }

    #endregion Public Properties

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Constructors

    public Monument() {
      InitializeComponent();

      AssetCategory = new List<TemplateAsset>();
      AssetEvent = new List<TemplateAsset>();
      AssetThreshold = new List<TemplateAsset>();
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentCategory.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          AssetCategory.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentEvent.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          AssetEvent.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentThreshold.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          AssetThreshold.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
        }
      }
      DataContext = this;
    }

    #endregion Public Constructors

    #region Private Methods

    //private readonly List<TemplateAsset> AssetReward;
    private void Monument_OnLoaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed += ComboBoxLanguage_SelectionChanged;
      ComboBoxCategories.SelectedIndex = 0;
    }

    private void ComboBoxLanguage_SelectionChanged() {
      ComboBoxCategories.SelectedItem = null;
      ComboBoxEvents.SelectedItem = null;
      ComboBoxThresholds.SelectedItem = null;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
      ComboBoxCategories.SelectedIndex = 0;
    }

    private void ComboBoxCategories_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Events)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      ComboBoxEvents.SelectedIndex = 0;
    }

    private void ComboBoxEvents_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thresholds)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      ComboBoxThresholds.SelectedIndex = 0;
    }

    private void ComboBoxThresholds_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rewards)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
    }

    private void ListBoxItems_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (e.AddedItems.Count == 0) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAsset)));
      }

      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RarityBrush"));
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed -= ComboBoxLanguage_SelectionChanged;
    }

    #endregion Private Methods

    #region Private Fields

    private readonly List<TemplateAsset> AssetCategory;
    private readonly List<TemplateAsset> AssetEvent;
    private readonly List<TemplateAsset> AssetThreshold;
    private TemplateAsset selectedAsset;

    #endregion Private Fields
  }
}