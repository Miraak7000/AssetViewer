using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using AssetViewer.Data;
using AssetViewer.Library;
using AssetViewer.Templates;

namespace AssetViewer.Controls {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  public partial class ThirdParty : UserControl, INotifyPropertyChanged {

    #region Properties
    public IEnumerable<TemplateAsset> Items {
      get {
        var thirdParty = this.ComboBoxThirdParty.SelectedItem as Tuple<String, String>;
        var progression = this.ComboBoxProgressions.SelectedItem as Tuple<Progression, String>;
        if (thirdParty == null || progression == null) return new TemplateAsset[0];
        var result = this.Assets.Single(w => w.ID == thirdParty.Item1).OfferingItems.Single(w => w.Progression == progression.Item1).Items.GetItemsById();
        switch (App.Language) {
          case Languages.German:
            result = result.OrderBy(o => o.Text.DE);
            break;
          default:
            result = result.OrderBy(o => o.Text.EN);
            break;
        }
        return result;
      }
    }
    public IEnumerable<Tuple<String, String>> ThirdParties {
      get {
        switch (App.Language) {
          case Languages.German:
            return this.Assets.Select(s => new Tuple<String, String>(s.ID, $"{s.Text.DE} - {s.ID}")).OrderBy(o => o.Item2);
          default:
            return this.Assets.Select(s => new Tuple<String, String>(s.ID, $"{s.Text.EN} - {s.ID}")).OrderBy(o => o.Item2);
        }
      }
    }
    public IEnumerable<Tuple<Progression, String>> Progressions {
      get {
        switch (App.Language) {
          case Languages.German:
            return new[] {
              new Tuple<Progression, String>(Progression.EarlyGame, "Frühes Spiel"),
              new Tuple<Progression, String>(Progression.EarlyMidGame, "Frühes-Mittleres Spiel"),
              new Tuple<Progression, String>(Progression.MidGame, "Mittleres Spiel"),
              new Tuple<Progression, String>(Progression.LateMidGame, "Mittleres-Spätes Spiel"),
              new Tuple<Progression, String>(Progression.LateGame, "Spätes Spiel"),
              new Tuple<Progression, String>(Progression.EndGame, "Endspiel")
            };
          default:
            return new[] {
              new Tuple<Progression, String>(Progression.EarlyGame, "Early Game"),
              new Tuple<Progression, String>(Progression.EarlyMidGame, "Early-Mid Game"),
              new Tuple<Progression, String>(Progression.MidGame, "Mid Game"),
              new Tuple<Progression, String>(Progression.LateMidGame, "Late-Mid Game"),
              new Tuple<Progression, String>(Progression.LateGame, "Late Game"),
              new Tuple<Progression, String>(Progression.EndGame, "End Game")
            };
        }
      }
    }
    public TemplateAsset SelectedAsset { get; set; }
    public String ImageThirdParty {
      get {
        var thirdParty = this.ComboBoxThirdParty.SelectedItem as Tuple<String, String>;
        if (thirdParty != null) {
          return this.Assets.Single(w => w.ID == thirdParty.Item1).Icon.Filename;
        } else {
          return null;
        }

      }
    }
    #endregion

    #region Fields
    private readonly List<TemplateThirdParty> Assets;
    #endregion

    #region Constructor
    public ThirdParty() {
      this.InitializeComponent();
      this.Assets = new List<TemplateThirdParty>();
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ThirdParty.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          this.Assets.AddRange(document.Elements().Select(s => new TemplateThirdParty(s)));
        }
      }
      this.DataContext = this;
    }
    #endregion

    #region Private Methods
    private void ThirdParty_OnLoaded(Object sender, RoutedEventArgs e) {
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
      if (e.AddedItems.Count == 0) this.ListBoxItems.SelectedIndex = 0;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedAsset"));
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
  }

}