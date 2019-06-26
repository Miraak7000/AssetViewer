using AssetViewer.Data;
using AssetViewer.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace AssetViewer.Comparer {
  public class RarityComparer : IComparer<string> {
    public static RarityComparer Default { get; } = new RarityComparer();
    private static List<string> RaritiesEN = new List<string>() { "Quest Item", "Common", "Uncommon", "Rare", "Epic", "Legendary" };
    private static List<string> RaritiesDE = new List<string>() { "Aufgaben-Item", "Gewöhnlich", "Ungewöhnlich", "Selten", "Episch", "Legendär" };
    public int Compare(string x, string y) {
      if (App.Language == Languages.German) {
        return RaritiesDE.IndexOf(x).CompareTo(RaritiesDE.IndexOf(y));
      }
      else {
        return RaritiesEN.IndexOf(x).CompareTo(RaritiesEN.IndexOf(y));
      }

    }
  }
}
