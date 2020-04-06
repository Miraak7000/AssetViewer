using AssetViewer.Data;
using AssetViewer.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Xml.Linq;

namespace AssetViewer {

  public static class AssetProvider {

    #region Public Properties

    public static Dictionary<int, TemplateAsset> Items { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, TemplateAsset> Buildings { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, TemplateAsset> ItemSets { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, TemplateAsset> FestivalBuffs { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, Pool> Pools { get; } = new Dictionary<int, Pool>();

    public static ObjectCache Cache { get; set; } = MemoryCache.Default;

    #endregion Public Properties

    #region Public Constructors

    static AssetProvider() {
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.RewardPools.xml"))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements().Select(s => s.FromXElement<Pool>())) {
          Pools.Add(item.ID, item);
        }
      }

      var items = new[] {
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
                "AssetViewer.Resources.Assets.ItemConstructionPlan.xml",
            };

      foreach (var str in items) {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str))
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
            Items.Add(item.ID, item);
          }
        }
      }
      var buildings = new[] {
                "AssetViewer.Resources.Assets.BuildPermitBuilding.xml",
                "AssetViewer.Resources.Assets.BuildPermitModules.xml",
                "AssetViewer.Resources.Assets.CultureModule.xml",
                "AssetViewer.Resources.Assets.OrnamentalModule.xml",
                "AssetViewer.Resources.Assets.BridgeBuilding.xml",
                "AssetViewer.Resources.Assets.CampaignQuestObject.xml",
                "AssetViewer.Resources.Assets.CampaignUncleMansion.xml",
                "AssetViewer.Resources.Assets.CityInstitutionBuilding.xml",
                "AssetViewer.Resources.Assets.CultureBuilding.xml",
                "AssetViewer.Resources.Assets.FactoryBuilding7.xml",
                "AssetViewer.Resources.Assets.FactoryBuilding7_Arctic.xml",
                "AssetViewer.Resources.Assets.FarmBuilding.xml",
                "AssetViewer.Resources.Assets.FarmBuilding_Arctic.xml",
                "AssetViewer.Resources.Assets.Farmfield.xml",
                "AssetViewer.Resources.Assets.FreeAreaBuilding.xml",
                "AssetViewer.Resources.Assets.FreeAreaBuilding_Arctic.xml",
                "AssetViewer.Resources.Assets.Guildhouse.xml",
                "AssetViewer.Resources.Assets.HarborBuildingAttacker.xml",
                "AssetViewer.Resources.Assets.HarborDepot.xml",
                "AssetViewer.Resources.Assets.HarborLandingStage7.xml",
                "AssetViewer.Resources.Assets.HarborOffice.xml",
                "AssetViewer.Resources.Assets.HarborPropObject.xml",
                "AssetViewer.Resources.Assets.HarborWarehouse7.xml",
                "AssetViewer.Resources.Assets.HarborWarehouseSlot7.xml",
                "AssetViewer.Resources.Assets.HarborWarehouseStrategic.xml",
                "AssetViewer.Resources.Assets.Heater_Arctic.xml",
                "AssetViewer.Resources.Assets.HeavyFactoryBuilding.xml",
                "AssetViewer.Resources.Assets.HeavyFreeAreaBuilding.xml",
                "AssetViewer.Resources.Assets.HeavyFreeAreaBuilding_Arctic.xml",
                "AssetViewer.Resources.Assets.ItemCrafterBuilding.xml",
                "AssetViewer.Resources.Assets.Market.xml",
                "AssetViewer.Resources.Assets.Monument.xml",
                "AssetViewer.Resources.Assets.Monument_with_Shipyard.xml",
                "AssetViewer.Resources.Assets.OilPumpBuilding.xml",
                "AssetViewer.Resources.Assets.OrnamentalBuilding.xml",
                "AssetViewer.Resources.Assets.PowerplantBuilding.xml",
                "AssetViewer.Resources.Assets.PublicServiceBuilding.xml",
                "AssetViewer.Resources.Assets.QuestLighthouse.xml",
                "AssetViewer.Resources.Assets.RepairCrane.xml",
                "AssetViewer.Resources.Assets.ResidenceBuilding7.xml",
                "AssetViewer.Resources.Assets.ResidenceBuilding7_Arctic.xml",
                "AssetViewer.Resources.Assets.Shipyard.xml",
                "AssetViewer.Resources.Assets.SimpleBuilding.xml",
                "AssetViewer.Resources.Assets.Slot.xml",
                "AssetViewer.Resources.Assets.SlotFactoryBuilding7.xml",
                "AssetViewer.Resources.Assets.SlotFactoryBuilding7_Arctic.xml",
                "AssetViewer.Resources.Assets.Street.xml",
                "AssetViewer.Resources.Assets.StreetBuilding.xml",
                "AssetViewer.Resources.Assets.VisitorPier.xml",
                "AssetViewer.Resources.Assets.VisualBuilding_NoLogic.xml",
                "AssetViewer.Resources.Assets.Warehouse.xml",
                "AssetViewer.Resources.Assets.WorkAreaSlot.xml",
                "AssetViewer.Resources.Assets.WorkforceConnector.xml",
                "AssetViewer.Resources.Assets.Palace.xml",
                "AssetViewer.Resources.Assets.PalaceMinistry.xml",
                "AssetViewer.Resources.Assets.PalaceModule.xml",
      };

      foreach (var str in buildings) {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str))
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
            Buildings.Add(item.ID, item);
          }
        }
      }

      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ItemSet.xml"))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
          ItemSets.Add(item.ID, item);
        }
      }
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.FestivalBuff.xml"))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
          FestivalBuffs.Add(item.ID, item);
        }
      }
    }

    #endregion Public Constructors

    #region Public Events

    public static event Action<IEnumerable<TemplateAsset>> OnAssetCountChanged;

    #endregion Public Events

    #region Public Methods

    public static IEnumerable<TemplateAsset> GetItemsById(this IEnumerable<int> ids) {
      return ids.SelectMany(l => AssetProvider.GetItemsById(l)).Distinct() ?? Enumerable.Empty<TemplateAsset>();
    }

    public static IEnumerable<TemplateAsset> GetItemsById(this int id) {
      foreach (var item in SearchItems(id).Distinct()) {
        yield return item;
      }

      IEnumerable<TemplateAsset> SearchItems(int searchid) {
        if (Items.ContainsKey(searchid)) {
          yield return Items[searchid];
        }
        else if (Buildings.ContainsKey(searchid)) {
          yield return Buildings[searchid];
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

    public static void RaiseAssetCountChanged(TemplateAsset asset) {
      var policy = new CacheItemPolicy {
        RemovedCallback = (cera => OnAssetCountChanged?.Invoke((cera.CacheItem.Value as IEnumerable<TemplateAsset>).Distinct())),
        SlidingExpiration = TimeSpan.FromMilliseconds(1500)
        ,
        AbsoluteExpiration = DateTimeOffset.MaxValue
      };

      Collection<TemplateAsset> list = null;
      if (Cache.GetCacheItem("CountAssets") is CacheItem cacheItem) {
        list = (Collection<TemplateAsset>)cacheItem.Value;
        list.Add(asset);
        //Cache.Set("CountAssets", list, policy);
      }
      else {
        list = new Collection<TemplateAsset>();
        list.Add(asset);
        Cache.Set("CountAssets", list, policy);
      }
    }

    #endregion Public Methods

    #region Private Methods

    private static void ukz(CacheEntryUpdateArguments arguments) {
    }

    #endregion Private Methods
  }
}