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
using AssetViewer.Data;

namespace AssetViewer.Controls {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  public partial class ThirdParty : UserControl, INotifyPropertyChanged {

    #region Public Properties

    public IEnumerable<TemplateAsset> Items {
      get {
        if (!(ComboBoxThirdParty.SelectedItem is Tuple<string, string> thirdParty) ||
          !(ComboBoxProgressions.SelectedItem is Tuple<Progression, Description> progression)) {
          return Array.Empty<TemplateAsset>();
        }

        var result = Assets.Single(w => w.ID == thirdParty.Item1).OfferingItems.Single(w => w.Progression == progression.Item1).Items?.GetItemsById();
        return result?.OrderBy(o => o.Text.CurrentLang);
      }
    }

    public IEnumerable<Tuple<string, string>> ThirdParties => Assets.Select(s => new Tuple<string, string>(s.ID, $"{s.Text.CurrentLang} - {s.ID}")).OrderBy(o => o.Item2);

    public IEnumerable<Tuple<Progression, Description>> Progressions {
      get {
        if (!(ComboBoxThirdParty.SelectedItem is Tuple<string, string> thirdParty)) {
          return Array.Empty<Tuple<Progression, Description>>();
        }

        var result = Assets.Single(w => w.ID == thirdParty.Item1).OfferingItems.Select(oi => new Tuple<Progression, Description>(oi.Progression, oi.ProgressionDescription));
        return result.OrderBy(o => o.Item1);
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

    public string ImageThirdParty => ComboBoxThirdParty.SelectedItem is Tuple<string, string> thirdParty ? Assets.Single(w => w.ID == thirdParty.Item1).Text.Icon.Filename : null;

    #endregion Public Properties

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Constructors

    public ThirdParty() {
      InitializeComponent();
      Assets = new List<TemplateThirdParty>();

      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ThirdParty.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          Assets.AddRange(document.Elements().Select(s => new TemplateThirdParty(s)));
        }
      }
      DataContext = this;
    }

    #endregion Public Constructors

    #region Private Methods

    private void ThirdParty_OnLoaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed += ComboBoxLanguage_SelectionChanged;
      ComboBoxThirdParty.SelectedIndex = 0;
      ComboBoxProgressions.SelectedIndex = 0;
    }

    private void ComboBoxLanguage_SelectionChanged() {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThirdParties)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progressions)));
      ComboBoxThirdParty.SelectedIndex = 0;
      ComboBoxProgressions.SelectedIndex = 0;
    }

    private void ComboBoxThirdParty_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageThirdParty)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progressions)));
      ComboBoxProgressions.SelectedIndex = 0;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
    }

    private void ComboBoxProgressions_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
    }

    private void ListBoxItems_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (e.AddedItems.Count == 0) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAsset)));
      }
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed -= ComboBoxLanguage_SelectionChanged;
    }

    #endregion Private Methods

    #region Private Fields

    private readonly List<TemplateThirdParty> Assets;
    private TemplateAsset selectedAsset;

    #endregion Private Fields
  }
}