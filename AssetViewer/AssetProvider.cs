using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using AssetViewer.Data;
using AssetViewer.Extensions;

namespace AssetViewer {
  public static class AssetProvider {

    #region Public Properties

    public static Dictionary<int, TemplateAsset> Items { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, TemplateAsset> Buildings { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, TemplateAsset> ItemSets { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, TemplateAsset> FestivalBuffs { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, TemplateAsset> AllBuffs { get; } = new Dictionary<int, TemplateAsset>();
    public static Dictionary<int, Pool> Pools { get; } = new Dictionary<int, Pool>();
    public static ObjectCache Cache { get; set; } = MemoryCache.Default;
    public static Dictionary<int, string> Descriptions { get; } = new Dictionary<int, string>();
    public static bool CountMode { get; set; } = true;
    public static List<Languages> PossibleLanguages { get; } = new List<Languages>();

    public static Data.Languages Language {
      get => language;
      private set { language = value; NotifyStaticPropertyChanged(); }
    }
    public static int MaxRerollCosts {
      get => maxRerollCosts;
      set {
        if (value != maxRerollCosts) {
          maxRerollCosts = value;
          NotifyStaticPropertyChanged();
          OnRerollCostsChanged?.Invoke(maxRerollCosts);

        }
      }
    }

    #endregion Public Properties

    #region Public Events

    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    public static event Action<IEnumerable<TemplateAsset>> OnAssetCountChanged;
    public static event Action<int> OnRerollCostsChanged;

    public static event Action OnLanguage_Changed;

    #endregion Public Events

    #region Public Constructors

    static AssetProvider() {
      //var comp = RarityComparer.Default;
      LoadPosibleLanguages();
      SetSystemLanguage();
      LoadLanguageFile();
      LoadPools();
      LoadItems();
      LoadBuildungs();
      LoadItemSets();
      LoadFestivals();
      LoadAllBuffs();
    }

    #endregion Public Constructors

    #region Public Methods

    public static void SetLanguage(Languages lang) {
      if (Language != lang && PossibleLanguages.Contains(lang)) {
        Language = lang;
        LoadLanguageFile();
        OnLanguage_Changed?.Invoke();
      }
    }

    public static IEnumerable<TemplateAsset> GetItemsById(this IEnumerable<int> ids) {
      return ids.SelectMany(l => GetItemsById(l)).Distinct() ?? Enumerable.Empty<TemplateAsset>();
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
      lock (LockChangedCountItems) {
        ChangedCountItems.Add(asset);
        Timer.Change(1000, Timeout.Infinite);
      }
    }

    public static void LoadLanguageFile() {
      Descriptions.Clear();
      var resource = $"AssetViewer.Resources.Assets.Texts_{Language:G}.xml";
      if (!Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(resource)) {
        Language = Data.Languages.English;
        resource = $"AssetViewer.Resources.Assets.Texts_{Language:G}.xml";
      }

      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements()) {
          if (int.TryParse(item.Attribute("ID")?.Value, out var id)) {
            Descriptions.Add(id, item.Value);
          }
        }
      }
    }

    #endregion Public Methods

    #region Private Methods

    private static void OnTimerTick(object state) {
      lock (LockChangedCountItems) {
        OnAssetCountChanged?.Invoke(ChangedCountItems.ToList());
        ChangedCountItems.Clear();
      }
    }

    private static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = null) {
      StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }

    private static void LoadFestivals() {
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.FestivalBuff.xml"))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
          FestivalBuffs.Add(item.ID, item);
        }
      }
    }

    private static void LoadAllBuffs() {
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.Buffs.xml"))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
          AllBuffs.Add(item.ID, item);
        }
      }
    }

    private static void LoadItemSets() {
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ItemSet.xml"))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
          ItemSets.Add(item.ID, item);
        }
      }
    }

    private static void LoadBuildungs() {
      var buildings = new[] {
                "AssetViewer.Resources.Assets.AdditionalModule.xml",
                "AssetViewer.Resources.Assets.BridgeBuilding.xml",
                "AssetViewer.Resources.Assets.BuffFactory.xml",
                "AssetViewer.Resources.Assets.BuildPermitBuilding.xml",
                "AssetViewer.Resources.Assets.BuildPermitModules.xml",
                "AssetViewer.Resources.Assets.Busstop.xml",
                "AssetViewer.Resources.Assets.CampaignQuestObject.xml",
                "AssetViewer.Resources.Assets.CampaignUncleMansion.xml",
                "AssetViewer.Resources.Assets.CityInstitutionBuilding.xml",
                "AssetViewer.Resources.Assets.CityInstitutionBuilding_Africa.xml",
                "AssetViewer.Resources.Assets.CultureBuilding.xml",
                "AssetViewer.Resources.Assets.CultureBuildingColony.xml",
                "AssetViewer.Resources.Assets.CultureModule.xml",
                "AssetViewer.Resources.Assets.DocklandItemModule.xml",
                "AssetViewer.Resources.Assets.DocklandMain.xml",
                "AssetViewer.Resources.Assets.DocklandModule.xml",
                "AssetViewer.Resources.Assets.DocklandModuleRepair.xml",
                "AssetViewer.Resources.Assets.DocklandPierModule.xml",
                "AssetViewer.Resources.Assets.DocklandStorageModule.xml",
                "AssetViewer.Resources.Assets.FactoryBuilding7.xml",
                "AssetViewer.Resources.Assets.FactoryBuilding7_Arctic.xml",
                "AssetViewer.Resources.Assets.FactoryBuilding7_BuildPermit.xml",
                "AssetViewer.Resources.Assets.FarmBuilding.xml",
                "AssetViewer.Resources.Assets.FarmBuilding_Arctic.xml",
                "AssetViewer.Resources.Assets.FarmBuilding_BuildPermit.xml",
                "AssetViewer.Resources.Assets.Farmfield.xml",
                "AssetViewer.Resources.Assets.FertilizerBaseBuilding.xml",
                "AssetViewer.Resources.Assets.FertilizerBaseModule.xml",
                "AssetViewer.Resources.Assets.FreeAreaBuilding.xml",
                "AssetViewer.Resources.Assets.FreeAreaBuilding_Arctic.xml",
                "AssetViewer.Resources.Assets.FreeAreaBuilding_BuildPermit.xml",
                "AssetViewer.Resources.Assets.FreeAreaRecipeBuilding.xml",
                "AssetViewer.Resources.Assets.Guildhouse.xml",
                "AssetViewer.Resources.Assets.HarborBuildingAttacker.xml",
                "AssetViewer.Resources.Assets.HarborDepot.xml",
                "AssetViewer.Resources.Assets.HarborLandingStage7.xml",
                "AssetViewer.Resources.Assets.HarborLandingStage7_BuildPermit.xml",
                "AssetViewer.Resources.Assets.HarborOffice.xml",
                "AssetViewer.Resources.Assets.HarborOrnament.xml",
                "AssetViewer.Resources.Assets.HarborPropObject.xml",
                "AssetViewer.Resources.Assets.HarborWarehouse7.xml",
                "AssetViewer.Resources.Assets.HarborWarehouseSlot7.xml",
                "AssetViewer.Resources.Assets.HarborWarehouseStrategic.xml",
                "AssetViewer.Resources.Assets.Heater_Arctic.xml",
                "AssetViewer.Resources.Assets.HeavyFactoryBuilding.xml",
                "AssetViewer.Resources.Assets.HeavyFreeAreaBuilding.xml",
                "AssetViewer.Resources.Assets.HeavyFreeAreaBuilding_Arctic.xml",
                "AssetViewer.Resources.Assets.IrrigationPropagationSource.xml",
                "AssetViewer.Resources.Assets.ItemCrafterBuilding.xml",
                "AssetViewer.Resources.Assets.ItemCrafterHarbor.xml",
                "AssetViewer.Resources.Assets.Mall.xml",
                "AssetViewer.Resources.Assets.Market.xml",
                "AssetViewer.Resources.Assets.Monument.xml",
                "AssetViewer.Resources.Assets.Monument_with_Shipyard.xml",
                "AssetViewer.Resources.Assets.Multifactory.xml",
                "AssetViewer.Resources.Assets.OilPumpBuilding.xml",
                "AssetViewer.Resources.Assets.OrnamentalBuilding.xml",
                "AssetViewer.Resources.Assets.OrnamentalModule.xml",
                "AssetViewer.Resources.Assets.Palace.xml",
                "AssetViewer.Resources.Assets.PalaceMinistry.xml",
                "AssetViewer.Resources.Assets.PalaceModule.xml",
                "AssetViewer.Resources.Assets.PowerplantBuilding.xml",
                "AssetViewer.Resources.Assets.PublicServiceBuilding.xml",
                "AssetViewer.Resources.Assets.PublicServiceBuildingWithBus.xml",
                "AssetViewer.Resources.Assets.QuestLighthouse.xml",
                "AssetViewer.Resources.Assets.QuestObjectHarborBuildingAttacker.xml",
                "AssetViewer.Resources.Assets.QuestObjectInfectable.xml",
                "AssetViewer.Resources.Assets.RepairCrane.xml",
                "AssetViewer.Resources.Assets.ResearchCenter.xml",
                "AssetViewer.Resources.Assets.ResidenceBuilding.xml",
                "AssetViewer.Resources.Assets.ResidenceBuilding7_Arctic.xml",
                "AssetViewer.Resources.Assets.ResidenceBuilding7_BuildPermit.xml",
                "AssetViewer.Resources.Assets.ResidenceBuilding7_Colony.xml",
                "AssetViewer.Resources.Assets.ResidenceBuilding7_Hotel.xml",
                "AssetViewer.Resources.Assets.ResidenceBuilding-Unique.xml",
                "AssetViewer.Resources.Assets.Resource.xml",
                "AssetViewer.Resources.Assets.Restaurant.xml",
                "AssetViewer.Resources.Assets.ScenarioRuin.xml",
                "AssetViewer.Resources.Assets.Shipyard.xml",
                "AssetViewer.Resources.Assets.SimpleBuilding.xml",
                "AssetViewer.Resources.Assets.Slot.xml",
                "AssetViewer.Resources.Assets.SlotFactoryBuilding7.xml",
                "AssetViewer.Resources.Assets.SlotFactoryBuilding7_Arctic.xml",
                "AssetViewer.Resources.Assets.Street.xml",
                "AssetViewer.Resources.Assets.StreetBuilding.xml",
                "AssetViewer.Resources.Assets.TowerRestaurant.xml",
                "AssetViewer.Resources.Assets.VisitorPier.xml",
                "AssetViewer.Resources.Assets.VisitorPierColony.xml",
                "AssetViewer.Resources.Assets.VisualBuilding_NoLogic.xml",
                "AssetViewer.Resources.Assets.Warehouse.xml",
                "AssetViewer.Resources.Assets.WorkAreaSlot.xml",
                "AssetViewer.Resources.Assets.WorkforceConnector.xml",
      };

      foreach (var str in buildings) {
        Debug.WriteLine(str);
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str))
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
            Buildings.Add(item.ID, item);
          }
        }
      }
    }

    private static void LoadItems() {
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
    }

    private static void LoadPools() {
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.RewardPools.xml"))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements().Select(s => s.FromXElement<Pool>())) {
          Pools.Add(item.ID, item);
        }
      }
    }

    private static void SetSystemLanguage() {
      switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName) {
        case "pt":
          Language = Data.Languages.Brazilian;
          break;

        case "zh":
          Language = Data.Languages.Chinese;
          break;

        case "en":
          Language = Data.Languages.English;
          break;

        case "fr":
          Language = Data.Languages.French;
          break;

        case "de":
          Language = Data.Languages.German;
          break;

        case "it":
          Language = Data.Languages.Italian;
          break;

        case "ja":
          Language = Data.Languages.Japanese;
          break;

        case "ko":
          Language = Data.Languages.Korean;
          break;

        case "pl":
          Language = Data.Languages.Polish;
          break;

        case "ru":
          Language = Data.Languages.Russian;
          break;

        case "es":
          Language = Data.Languages.Spanish;
          break;
        //case "pt": Language = Library.Languages.Portuguese; break;
        //case "zh	": Language = Library.Languages.Taiwanese; break;
        default:
          Language = Data.Languages.English;
          break;
      }
    }

    private static void LoadPosibleLanguages() {
      foreach (var lang in Enum.GetValues(typeof(Languages))) {
        var currLang = (Languages)lang;
        var resource = $"AssetViewer.Resources.Assets.Texts_{currLang:G}.xml";
        if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(resource)) {
          PossibleLanguages.Add(currLang);
        }
      }
    }

    #endregion Private Methods

    #region Private Fields

    private static readonly Timer Timer = new Timer(OnTimerTick);
    private static readonly object LockChangedCountItems = new HashSet<TemplateAsset>();
    private static readonly HashSet<TemplateAsset> ChangedCountItems = new HashSet<TemplateAsset>();

    private static Data.Languages language = Data.Languages.English;
    private static int maxRerollCosts;

    #endregion Private Fields
  }
}