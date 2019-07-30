using AssetViewer.Data;
using AssetViewer.Templates;
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
        var progression = this.ComboBoxProgressions.SelectedItem as Tuple<Progression, String>;
        if (thirdParty == null || progression == null)
          return new TemplateAsset[0];
        var result = this.Assets.Single(w => w.ID == thirdParty.Item1).OfferingItems.Single(w => w.Progression == progression.Item1).Items.GetItemsById();
        result.OrderBy(o => o.Text.CurrentLang);
        return result;
      }
    }

    public IEnumerable<Tuple<String, String>> ThirdParties {
      get {
        return this.Assets.Select(s => new Tuple<String, String>(s.ID, $"{s.Text.CurrentLang} - {s.ID}")).OrderBy(o => o.Item2);
      }
    }

    public IEnumerable<Tuple<Progression, String>> Progressions {
      get {
        return new[] {
              new Tuple<Progression, String>(Progression.EarlyGame, App.Descriptions["-6"]),
              new Tuple<Progression, String>(Progression.EarlyMidGame, App.Descriptions["-7"]),
              new Tuple<Progression, String>(Progression.MidGame, App.Descriptions["-8"]),
              new Tuple<Progression, String>(Progression.LateMidGame, App.Descriptions["-9"]),
              new Tuple<Progression, String>(Progression.LateGame, App.Descriptions["-10"]),
              new Tuple<Progression, String>(Progression.EndGame, App.Descriptions["-11"])
            };
      }
    }

    public TemplateAsset SelectedAsset { get; set; }

    public String ImageThirdParty {
      get {
        var thirdParty = this.ComboBoxThirdParty.SelectedItem as Tuple<String, String>;
        if (thirdParty != null) {
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

    #endregion Fields

    #region Methods

    private void ThirdParty_OnLoaded(Object sender, RoutedEventArgs e) {
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      this.ComboBoxThirdParty.SelectedIndex = 0;
      this.ComboBoxProgressions.SelectedIndex = 0;
    }
    private void ComboBoxLanguage_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThirdParties"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Progressions"));
      this.ComboBoxThirdParty.SelectedIndex = 0;
      this.ComboBoxProgressions.SelectedIndex = 0;
    }
    private void ComboBoxThirdParty_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageThirdParty"));
    }
    private void ComboBoxProgressions_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
    }
    private void ListBoxItems_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
      if (e.AddedItems.Count == 0)
        this.ListBoxItems.SelectedIndex = 0;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedAsset"));
    }
    private void UserControl_Unloaded(object sender, RoutedEventArgs e) {
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged -= this.ComboBoxLanguage_SelectionChanged;
    }

    #endregion Methods
  }
}