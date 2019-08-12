using AssetViewer.Data;
using AssetViewer.Extensions;
using AssetViewer.Templates;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace AssetViewer {

  public static class ItemProvider {

    #region Properties

    public static Dictionary<string, TemplateAsset> Items { get; } = new Dictionary<string, TemplateAsset>();
    public static Dictionary<string, Pool> Pools { get; } = new Dictionary<string, Pool>();

    #endregion Properties

    #region Constructors

    static ItemProvider() {
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.RewardPools.xml"))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements().Select(s => s.FromXElement<Pool>())) {
          Pools.Add(item.ID, item);
        }
      }

      var arr = new[] {
                "AssetViewer.Resources.Assets.GuildhouseItem.xml",
                "AssetViewer.Resources.Assets.HarborOfficeItem.xml",
                "AssetViewer.Resources.Assets.TownhallItem.xml",
                "AssetViewer.Resources.Assets.VehicleItem.xml",
                "AssetViewer.Resources.Assets.ShipSpecialist.xml",
                "AssetViewer.Resources.Assets.CultureItem.xml",
                "AssetViewer.Resources.Assets.Product.xml",
                "AssetViewer.Resources.Assets.ItemSpecialAction.xml",
                "AssetViewer.Resources.Assets.ActiveItem.xml",
                "AssetViewer.Resources.Assets.ItemSpecialActionVisualEffect.xml",
                "AssetViewer.Resources.Assets.ItemWithUI.xml",
                "AssetViewer.Resources.Assets.FluffItem.xml",
                "AssetViewer.Resources.Assets.QuestItemMagistrate.xml",
                "AssetViewer.Resources.Assets.StartExpeditionItem.xml",
                "AssetViewer.Resources.Assets.QuestItem.xml",
                "AssetViewer.Resources.Assets.BuildPermitBuilding.xml",
                "AssetViewer.Resources.Assets.ItemSet.xml"
            };

      foreach (var str in arr) {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str))
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
            Items.Add(item.ID, item);
          }
        }
      }
    }

    #endregion Constructors

    #region Methods

    public static IEnumerable<TemplateAsset> GetItemsById(this IEnumerable<string> ids) {
      return ids.SelectMany(l => ItemProvider.GetItemsById(l)).Distinct() ?? Enumerable.Empty<TemplateAsset>();
    }

    public static IEnumerable<TemplateAsset> GetItemsById(this string id) {
      foreach (var item in SearchItems(id).Distinct()) {
        yield return item;
      }

      IEnumerable<TemplateAsset> SearchItems(string searchid) {
        if (searchid == null) {
        }
        else if (Items.ContainsKey(searchid)) {
          yield return Items[searchid];
        }
        else if (Pools.ContainsKey(searchid)) {
          foreach (var item in Pools[searchid].Items) {
            if (item.Weight > 0) {
              foreach (var item2 in SearchItems(item.ID)) {
                yield return item2;
              }
            }
          }
        }
        else {
          Debug.WriteLine(searchid);
        }
      }
    }

    #endregion Methods
  }
}