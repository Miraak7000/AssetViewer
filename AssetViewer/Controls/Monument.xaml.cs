using AssetViewer.Data;
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

namespace AssetViewer.Controls {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  public partial class Monument : UserControl, INotifyPropertyChanged {

    #region Public Properties

    public IEnumerable<TemplateAsset> Categories {
      get { return this.AssetCategory.AsEnumerable(); }
    }

    public IEnumerable<TemplateAsset> Events {
      get {
        if (!(this.ComboBoxCategories.SelectedItem is TemplateAsset monumentCategory)) {
          return Array.Empty<TemplateAsset>();
        }

        return this.AssetEvent.Where(w => monumentCategory.MonumentEvents.Contains(w.ID));
      }
    }

    public IEnumerable<TemplateAsset> Thresholds {
      get {
        if (!(this.ComboBoxEvents.SelectedItem is TemplateAsset monumentEvent)) {
          return Array.Empty<TemplateAsset>();
        }

        return this.AssetThreshold.Where(w => monumentEvent.MonumentThresholds.Contains(w.ID));
      }
    }

    public IEnumerable<TemplateAsset> Rewards {
      get {
        if (!(this.ComboBoxThresholds.SelectedItem is TemplateAsset monumentThreshold)) {
          return Array.Empty<TemplateAsset>();
        }

        return monumentThreshold.MonumentRewards.GetItemsById();
      }
    }

    public TemplateAsset SelectedAsset {
      get {
        return selectedAsset;
      }
      set {
        if (selectedAsset != value) {
          selectedAsset = value;
          this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAsset)));
        }
      }
    }

    #endregion Public Properties

    #region Public Constructors

    public Monument() {
      this.InitializeComponent();

      this.AssetCategory = new List<TemplateAsset>();
      this.AssetEvent = new List<TemplateAsset>();
      this.AssetThreshold = new List<TemplateAsset>();
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentCategory.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.AssetCategory.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentEvent.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.AssetEvent.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentThreshold.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.AssetThreshold.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
        }
      }
      this.DataContext = this;
    }

    #endregion Public Constructors

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Private Fields

    private readonly List<TemplateAsset> AssetCategory;
    private readonly List<TemplateAsset> AssetEvent;
    private readonly List<TemplateAsset> AssetThreshold;
    private TemplateAsset selectedAsset;

    #endregion Private Fields

    #region Private Methods

    //private readonly List<TemplateAsset> AssetReward;
    private void Monument_OnLoaded(Object sender, RoutedEventArgs e) {
      ((MainWindow)Application.Current.MainWindow).OnLanguage_Changed += this.ComboBoxLanguage_SelectionChanged;
      this.ComboBoxCategories.SelectedIndex = 0;
    }

    private void ComboBoxLanguage_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.ComboBoxCategories.SelectedItem = null;
      this.ComboBoxEvents.SelectedItem = null;
      this.ComboBoxThresholds.SelectedItem = null;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
      this.ComboBoxCategories.SelectedIndex = 0;
    }

    private void ComboBoxCategories_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Events)));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      this.ComboBoxEvents.SelectedIndex = 0;
    }

    private void ComboBoxEvents_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thresholds)));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
      this.ComboBoxThresholds.SelectedIndex = 0;
    }

    private void ComboBoxThresholds_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rewards)));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
    }

    private void ListBoxItems_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      if (e.AddedItems.Count == 0) {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAsset)));
      }

      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RarityBrush"));
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e) {
      if (Application.Current.MainWindow is MainWindow main) {
        main.OnLanguage_Changed -= this.ComboBoxLanguage_SelectionChanged;
      }
    }

    #endregion Private Methods
  }
}