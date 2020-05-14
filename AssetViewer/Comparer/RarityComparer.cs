using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Comparer {

  public class RarityComparer : IComparer<int> {

    #region Public Properties

    public static RarityComparer Default { get; } = new RarityComparer();

    #endregion Public Properties

    #region Public Constructors

    public RarityComparer() {
      SetConverter();
    }

    #endregion Public Constructors

    #region Public Methods

    public int Compare(int x, int y) {
      return RaritiesEN.IndexOf(RaritiesToENConverter.TryGetValue(x, out var Xrarity) ? Xrarity : "").CompareTo(RaritiesEN.IndexOf(RaritiesToENConverter.TryGetValue(y, out var Yrarity) ? Yrarity : ""));
    }

    #endregion Public Methods

    #region Private Methods

    private void SetConverter() {
      RaritiesToENConverter.Clear();
      foreach (var item in RaritiesEN) {
        var rarity = AssetProvider.Items.Values.FirstOrDefault(i => i.RarityType == item)?.Rarity;
        if (rarity != null) {
          RaritiesToENConverter.Add(rarity.ID, item);
        }
      }
    }

    #endregion Private Methods

    #region Private Fields

    private readonly Dictionary<int, string> RaritiesToENConverter = new Dictionary<int, string>();
    private readonly List<string> RaritiesEN = new List<string> { "Narrative", "Quest", "Common", "Uncommon", "Rare", "Epic", "Legendary" };

    #endregion Private Fields
  }
}