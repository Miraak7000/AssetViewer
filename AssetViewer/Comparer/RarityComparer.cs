using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssetViewer.Comparer {

  public class RarityComparer : IComparer<string> {

    #region Properties

    public static RarityComparer Default { get; } = new RarityComparer();

    #endregion Properties

    #region Constructors

    public RarityComparer() {
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += OnLanguageChanged;
      SetConverter();
    }

    #endregion Constructors

    #region Methods

    public int Compare(string x, string y) {
      return RaritiesEN.IndexOf(RaritiesToENConverter.TryGetValue(x, out var Xrarity) ? Xrarity : "").CompareTo(RaritiesEN.IndexOf(RaritiesToENConverter.TryGetValue(y, out var Yrarity) ? Yrarity : ""));
    }

    #endregion Methods

    #region Fields

    private readonly Dictionary<string, string> RaritiesToENConverter = new Dictionary<string, string>();
    private readonly List<string> RaritiesEN = new List<string>() { "Narrative", "Quest", "Common", "Uncommon", "Rare", "Epic", "Legendary" };

    #endregion Fields

    private void OnLanguageChanged(object sender, SelectionChangedEventArgs e) {
      SetConverter();
    }

    private void SetConverter() {
      RaritiesToENConverter.Clear();
      foreach (var item in RaritiesEN) {
        var rarity = ItemProvider.Items.Values.FirstOrDefault(i => i.RarityType == item)?.Rarity.CurrentLang;
        if (rarity != null) {
          RaritiesToENConverter.Add(rarity, item);
        }
      }
    }
  }
}