using AssetViewer.Data;
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
  public partial class ThirdParty : UserControl, INotifyPropertyChanged {

    #region Properties

    public IEnumerable<TemplateAsset> Items {
      get {
        var thirdParty = this.ComboBoxThirdParty.SelectedItem as Tuple<String, String>;
        var progression = this.ComboBoxProgressions.SelectedItem as Tuple<Progression, Description>;
        if (thirdParty == null || progression == null) {
          return Array.Empty<TemplateAsset>();
        }

        var result = this.Assets.Single(w => w.ID == thirdParty.Item1).OfferingItems.Single(w => w.Progression == progression.Item1).Items.GetItemsById();
        return result.OrderBy(o => o.Text.CurrentLang);
      }
    }

    public IEnumerable<Tuple<String, String>> ThirdParties {
      get {
        return this.Assets.Select(s => new Tuple<String, String>(s.ID, $"{s.Text.CurrentLang} - {s.ID}")).OrderBy(o => o.Item2);
      }
    }

    public IEnumerable<Tuple<Progression, Description>> Progressions {
      get {
        var thirdParty = this.ComboBoxThirdParty.SelectedItem as Tuple<String, String>;
        if (thirdParty == null) {
          return Array.Empty<Tuple<Progression, Description>>();
        }

        var result = this.Assets.Single(w => w.ID == thirdParty.Item1).OfferingItems.Select(oi => new Tuple<Progression, Description>(oi.Progression, oi.ProgressionDescription));
        return result.OrderBy(o => o.Item1);
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

    public String ImageThirdParty {
      get {
        if (this.ComboBoxThirdParty.SelectedItem is Tuple<String, String> thirdParty) {
          return this.Assets.Single(w => w.ID == thirdParty.Item1).Text.Icon.Filename;
        }
        else {
          return null;
        }
      }
    }

    #endregion Properties

    #region Constructors

    public ThirdParty() {
      this.InitializeComponent();
      this.Assets = new List<TemplateThirdParty>();

      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ThirdParty.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.Assets.AddRange(document.Elements().Select(s => new TemplateThirdParty(s)));
        }
      }
      this.DataContext = this;
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Fields

    private readonly List<TemplateThirdParty> Assets;
    private TemplateAsset selectedAsset;

    #endregion Fields

    #region Methods

    private void ThirdParty_OnLoaded(Object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed += this.ComboBoxLanguage_SelectionChanged;
      this.ComboBoxThirdParty.SelectedIndex = 0;
      this.ComboBoxProgressions.SelectedIndex = 0;
    }

    private void ComboBoxLanguage_SelectionChanged() {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThirdParties)));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progressions)));
      this.ComboBoxThirdParty.SelectedIndex = 0;
      this.ComboBoxProgressions.SelectedIndex = 0;
    }

    private void ComboBoxThirdParty_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageThirdParty)));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progressions)));
      this.ComboBoxProgressions.SelectedIndex = 0;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
    }

    private void ComboBoxProgressions_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
    }

    private void ListBoxItems_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      if (e.AddedItems.Count == 0) {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAsset)));
      }
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed -= this.ComboBoxLanguage_SelectionChanged;
    }

    #endregion Methods
  }
}