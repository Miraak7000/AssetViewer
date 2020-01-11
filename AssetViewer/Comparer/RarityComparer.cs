using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Comparer {

  public class RarityComparer : IComparer<int> {

    #region Properties

    public static RarityComparer Default { get; } = new RarityComparer();

    #endregion Properties

    #region Constructors

    public RarityComparer() {
      SetConverter();
    }

    #endregion Constructors

    #region Methods

    public int Compare(int x, int y) {
      return RaritiesEN.IndexOf(RaritiesToENConverter.TryGetValue(x, out var Xrarity) ? Xrarity : "").CompareTo(RaritiesEN.IndexOf(RaritiesToENConverter.TryGetValue(y, out var Yrarity) ? Yrarity : ""));
    }

    #endregion Methods

    #region Fields

    private readonly Dictionary<int, string> RaritiesToENConverter = new Dictionary<int, string>();
    private readonly List<string> RaritiesEN = new List<string>() { "Narrative", "Quest", "Common", "Uncommon", "Rare", "Epic", "Legendary" };

    #endregion Fields

    private void SetConverter() {
      RaritiesToENConverter.Clear();
      foreach (var item in RaritiesEN) {
        var rarity = AssetProvider.Items.Values.FirstOrDefault(i => i.RarityType == item)?.Rarity;
        if (rarity != null) {
          RaritiesToENConverter.Add(rarity.ID, item);
        }
      }
    }
  }
}