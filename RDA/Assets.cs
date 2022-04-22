using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;
using RDA.Services;

namespace RDA {

  public static class Assets {

    #region Public Properties

    public static XElement TourismFeatureAsset { get; private set; }
    public static XElement ResearchFeatureAsset { get; private set; }

    #endregion Public Properties

    #region Public Constructors

    static Assets() {
      BaseGame = XmlLoader.LoadXml(Program.PathRoot + @"\Original\assets.xml");
      Eden_Burning = XmlLoader.LoadSzenarioXml(Program.PathRoot + @"\Original\Eden Burning\assets.xml", GameTypes.Eden_Burning);
      Seasons_of_Silver = XmlLoader.LoadSzenarioXml(Program.PathRoot + @"\Original\Seasons of Silver\assets.xml", GameTypes.Seasons_of_Silver);
      All = new XElement("Root");
      All.Add(BaseGame);
      All.Add(Eden_Burning);
      All.Add(Seasons_of_Silver);
    }

    #endregion Public Constructors

    #region Public Methods

    public static void Init(string version = "Release") {
      Version = version;
      LoadDefaultValues();
      Program.ConsoleWriteHeadline("Load Asset.xml");

      SolveXmlInheritance();
      SolveAllReferences();
      LoadDescriptions();
      LoadCustomDescriptions();
      SetTextDictionarys();
      SetIcons();
      SetTourismThresholds();

      TourismFeatureAsset = GUIDs["2001173", GameTypes.Anno_1800];
      ResearchFeatureAsset = GUIDs["120244", GameTypes.Anno_1800];

      SetBuffs();
    }

    public static string GetDescriptionID(string id) {
      return KeyToIdDict[id];
    }

    public static bool TryGetDescriptionID(string id, out string value) {
      return KeyToIdDict.TryGetValue(id, out value);
    }

    #endregion Public Methods

    #region Internal Fields

    internal readonly static XElement BaseGame;
    internal readonly static XElement All;
    internal readonly static XElement Eden_Burning;
    internal readonly static XElement Seasons_of_Silver;

    internal readonly static Dictionary<string, XElement> DefaultValues = new Dictionary<string, XElement>();

    internal readonly static Dictionary<string, Dictionary<Languages, string>> Descriptions = new Dictionary<string, Dictionary<Languages, string>>();

    internal readonly static Dictionary<string, Dictionary<Languages, string>> CustomDescriptions = new Dictionary<string, Dictionary<Languages, string>>();

    internal readonly static Dictionary<string, HashSet<XElement>> References = new Dictionary<string, HashSet<XElement>>();

    internal readonly static GUIDList GUIDs = new GUIDList();

    internal readonly static Dictionary<string, string> TourismThresholds = new Dictionary<string, string>();

    internal readonly static Dictionary<string, Asset> Buffs = new Dictionary<string, Asset>();

    internal readonly static Dictionary<string, SourceWithDetails> ResearchableItems = new Dictionary<string, SourceWithDetails>();

    internal readonly static IconList Icons = new IconList();
    private readonly static Dictionary<string, string> KeyToIdDict = new Dictionary<string, string>();
    internal readonly static Dictionary<string, string> ExpeditionRegionToIdDict = new Dictionary<string, string>();
    internal static string Version = "Release";

    internal readonly static HashSet<string> templatesResearchableItems = new HashSet<string> {

                "GuildhouseItem",
                "HarborOfficeItem",
                "TownhallItem",
                "VehicleItem",
                "ShipSpecialist",
                "CultureItem",
                "ItemSpecialAction",
                "ActiveItem",
                "ItemSpecialActionVisualEffect",

  };

    #endregion Internal Fields

    #region Private Methods

    private static void LoadDefaultValues() {
      Program.ConsoleWriteHeadline("Load Standart Values");
      var xml = XmlLoader.LoadXml(Program.PathRoot + @"\Original\properties.xml");
      foreach (var item in xml.Descendants("DefaultValues").SelectMany(dv => dv.Elements())) {
        DefaultValues.Add(item.Name.LocalName, item);
      }
    }

    private static int InheritDepth(this XElement ele) {
      var depth = 0;
      var search = ele.Element("BaseAssetGUID")?.Value ?? ele.Element("ScenarioBaseAssetGUID")?.Value;
      while (search != null) {
        var found = Assets.All.Descendants("Asset").FirstOrDefault(a=> a.XPathSelectElement("Values/Standard/GUID")?.Value == search);
        if (found != null) {
          depth++;
          search = found.Element("BaseAssetGUID")?.Value ?? found.Element("ScenarioBaseAssetGUID")?.Value;
        }
      }
      return depth;
    }

    private static void SolveAllReferences() {
      Program.ConsoleWriteHeadline("Solve All References");
      Regex rgx = new Regex("\\A\\d\\d\\d+\\Z");

      foreach (var node in All.Descendants()) {
        var asset = node;
        while (asset.Parent != null &&
            (!asset.Name.LocalName.Equals("Asset") || !asset.HasElements)) {
          asset = asset.Parent;
        }

        if ("GUID".Equals(node.Name.LocalName) &&
            (node == asset.XPathSelectElement("Values/Standard/GUID") || node == asset.XPathSelectElement("Values/Standard/GUID"))) {
          GUIDs.Add(node.Value, asset);
        }
        else if (!node.HasElements && rgx.IsMatch(node.Value)) {
          if (asset.Name != "Asset")
            continue;

          if (References.ContainsKey(node.Value))
            References[node.Value].Add(asset);
          else
            References.Add(node.Value, new HashSet<XElement> { asset });
        }
      }
    }

    //private static void processResearchPool(string id, SourceWithDetails parentDetails, AssetWithWeight parentCategory) {
    //  GUIDs.TryGetValue(id, out var poolOrAsset);

    //  if (poolOrAsset == null)
    //    return;

    //  var pools = poolOrAsset.XPathSelectElements("Values/AssetPool/AssetList/Item/Asset | Values/RewardPool/ItemsPool/Item/ItemLink").ToList();

    //  if (pools.Count == 0) {
    //    if (!"RewardPool".Equals(poolOrAsset.XPathSelectElement("Template").Value)) {// ignore empty reward pools

    //      if (!ResearchableItems.ContainsKey(id)) {

    //        var details = parentDetails.Copy();
    //        details.Details.Add(parentCategory);
    //        ResearchableItems.Add(id, details);
    //      }
    //      else {
    //        foreach (var d in ResearchableItems[id].Details) {
    //          d.Weight += parentDetails.Details.First().Weight;
    //        }
    //      }
    //    }
    //  }

    //  var itemList = pools.Select(p => p.XPathSelectElement("../Probability | ../Weight"));
    //  var weightSum = itemList.Sum(item => item?.Value is string str ? double.Parse(str) : 1.0F);

    //  foreach (var pool in pools) {
    //    var prob = pool.XPathSelectElement("../Probability | ../Weight");
    //    if (prob != null && (prob.Value == "" || prob.Value == "0"))
    //      continue;

    //    var details = parentDetails.Copy();

    //    details.Details.First().Weight *= (prob?.Value is string str ? double.Parse(str) : 1.0F) / weightSum;


    //    processResearchPool(pool.Value, details, parentCategory);
    //  }
    //}

    private static void SolveXmlInheritance() {
      Program.ConsoleWriteHeadline("Solve Xml Inheritance");
      var InheritHelper = All.Descendants("Asset").OrderBy(a => a.InheritDepth()).ToArray();
      foreach (var item in InheritHelper) {
        var baseGuid = item.Element("BaseAssetGUID")?.Value ?? item.Element("ScenarioBaseAssetGUID")?.Value;
        if (baseGuid != null) {
          var baseAsset = Assets.All.Descendants("Asset").FirstOrDefault(a => a.XPathSelectElement("Values/Standard/GUID")?.Value == baseGuid); ;
          if (baseAsset != null) {
            item.AddStandardValues(baseAsset);
          }
        }
      }
    }

    private static void LoadDescriptions() {
      Program.ConsoleWriteHeadline("Load Descriptions");
      foreach (Languages language in Enum.GetValues(typeof(Languages))) {
        var dic = XDocument
            .Load(Program.PathRoot + $@"\Original\texts_{language.ToString("G").ToLower()}.xml")
            .Root
            .Element("Texts")
            .Elements()
            .ToDictionary(k => k.Element("GUID").Value, e => e.Element("Text").Value);
        foreach (var item in dic) {
          var str = item.Value;
          //Delete some trash
          var removes = new[] {
            HttpUtility.HtmlDecode("&lt;font overrideTextColor=\"true\" color='#e3ce10'&gt;&lt;b&gt;"),
            HttpUtility.HtmlDecode("&lt;/b&gt;&lt;/font&gt;"),
            HttpUtility.HtmlDecode("&lt;font overrideTextColor=\"true\" color='#e1e1e1'&gt;&lt;b&gt;"),
            HttpUtility.HtmlDecode("&lt;font overrideTextColor=\"true\" color='#d8556a'&gt;&lt;b&gt;"),
            HttpUtility.HtmlDecode("&lt;br /&gt;"),
            "<br>",
            "<b>",
            "</b>",
          };
          foreach (var remove in removes) {
            str = str.Replace(remove, "");
          }

          // walkaround to fix the problem that rarity "common" and "uncommon" are translated to the
          // same Chinese word
          if (language == Languages.Chinese && item.Key == "118002" && str == "普通") {
            str = "普通（白色）";
          }
          if (language == Languages.Chinese && item.Key == "118003" && str == "普通") {
            str = "普通（绿色）";
          }

          if (Descriptions.ContainsKey(item.Key)) {
            Descriptions[item.Key].Add(language, str);
          }
          else {
            Descriptions.Add(item.Key, new Dictionary<Languages, string> { { language, str } });
          }
        }
      }
    }

    private static void LoadCustomDescriptions() {
      Program.ConsoleWriteHeadline("Load Custom Descriptions");
      var js = new JavaScriptSerializer();
      foreach (Languages language in Enum.GetValues(typeof(Languages))) {
        var filepath = Program.PathRoot + $@"\Modified\LanguageFiles\Texts_Custom_{language:G}.json";
        if (File.Exists(filepath)) {
          var dic = js.Deserialize<dynamic>(File.ReadAllText(filepath));
          foreach (var item in dic) {
            if (CustomDescriptions.ContainsKey(item.Key)) {
              CustomDescriptions[item.Key].Add(language, item.Value);
            }
            else {
              CustomDescriptions.Add(item.Key, new Dictionary<Languages, string> { { language, item.Value } });
            }
          }
        }
      }
    }

    private static void SetIcons() {
      Program.ConsoleWriteHeadline("Setting up Icons");

      //AllocationIcons
      var assets = All
         .Descendants("Asset")
         .Where(a => a.Element("Template")?.Value == "ItemBalancing");

      foreach (var asset in assets) {
        var gameType = (GameTypes)Enum.Parse(typeof(GameTypes), asset.Attribute("GameType").Value);
        var itemconfig = asset.Element("Values").Element("ItemConfig");
        foreach (var item in itemconfig.Element("AllocationIcons").Elements()) {
          if (item.Element("AllocationIcon")?.Value is string val) {
            Icons.Add(item.Element("Allocation").Value, gameType, val);
          }
        }
      }
         
      assets = All
        .Descendants("Asset")
        .Where(a =>/* a.XPathSelectElement("Values/Text")?.HasElements == true &&*/ a.XPathSelectElement("Values/Standard/IconFilename")?.Value != null)/*.Select(e => e.Element("Values").Element("Standard"))*/;
        //.Where(a => a.XPathSelectElement("Values/Text")?.HasElements == true && a.XPathSelectElement("Values/Standard/IconFilename")?.Value != null).Select(e => e.Element("Values").Element("Standard"));
        //.Where(a => a.XPathSelectElement("Values/Text/LocaText")?.HasElements == true && a.XPathSelectElement("Values/Standard/IconFilename")?.Value != null).Select(e => e.Element("Values").Element("Standard"));
      //TextIcons
      foreach (var asset in assets) {
        var gameType = (GameTypes)Enum.Parse(typeof(GameTypes), asset.Attribute("GameType").Value);
        Icons.Add(asset.XPathSelectElement("Values/Standard/GUID").Value, gameType, asset.XPathSelectElement("Values/Standard/IconFilename").Value);
      }

      Icons.Add("Ship", GameTypes.Anno_1800, "data/ui/2kimages/main/3dicons/icon_ship.png");
      Icons.Add("Warship", GameTypes.Anno_1800, "data/ui/2kimages/main/3dicons/ships/icon_ship_battlecruiser.png");
      Icons.Add("RadiusBuilding", GameTypes.Anno_1800, "data/ui/2kimages/main/3dicons/icon_guildhouse.png");
      Icons.Add("12508", GameTypes.Anno_1800, "data/ui/2kimages/main/icons/icon_electricity.png");         //Need Elektrizität
      Icons.Add("15798", GameTypes.Anno_1800, "data/ui/2kimages/main/icons/icon_generic_expedition.png");  //PerkMale
      Icons.Add("15797", GameTypes.Anno_1800, "data/ui/2kimages/main/icons/icon_generic_expedition.png");  //PerkFeMale
    }

    private static void SetTextDictionarys() {
      Program.ConsoleWriteHeadline("Setting up Descriptions");
      var asset = All
         .Descendants("Asset")
         .FirstOrDefault(a => a.Element("Template")?.Value == "ItemBalancing")?
         .Element("Values")
         .Element("ItemConfig");
      //RarityText
      foreach (var item in asset.Element("RarityText").Elements()) {
        KeyToIdDict.Add(item.Name.LocalName, item.Element("Text").Value);
      }
      //ExclusiveGroupText
      foreach (var item in asset.Element("ExclusiveGroupText").Elements()) {
        KeyToIdDict.Add(item.Name.LocalName, item.Element("Text").Value);
      }
      //ExclusiveGroupText
      foreach (var item in asset.Element("AllocationText").Elements()) {
        KeyToIdDict.Add(item.Name.LocalName, item.Element("Text").Value);
      }

      asset = All
       .Descendants("Asset")
       .FirstOrDefault(a => a.Element("Template")?.Value == "ExpeditionFeature")?
       .Element("Values")
       .Element("ExpeditionFeature");

      //ExpeditionRegions
      foreach (var item in asset.Element("ExpeditionRegions").Elements()) {
        ExpeditionRegionToIdDict.Add(item.Name.LocalName, item.Element("Region").Value);
      }

      //AttributeNames
      foreach (var item in asset.Element("AttributeNames").Elements()) {
        KeyToIdDict.Add(item.Name.LocalName, item.Element("Name").Value);
      }

      //Custom Additions
      KeyToIdDict.Add("ProductivityUpgrade", "118000");
      KeyToIdDict.Add("AdditionalOutput", "20074");
      KeyToIdDict.Add("ReplaceInputs", "20081");
      KeyToIdDict.Add("InputAmountUpgrade", "100369");
      KeyToIdDict.Add("OutputAmountFactorUpgrade", "11989");
      KeyToIdDict.Add("NeededAreaPercentUpgrade", "15319");
      KeyToIdDict.Add("NeedsElectricity", "12508");
      KeyToIdDict.Add("PerkFemale", "15798");
      KeyToIdDict.Add("PerkMale", "15797");
      KeyToIdDict.Add("PerkSteamShip", "15795");
      KeyToIdDict.Add("AttractivenessUpgrade", "145011");
      KeyToIdDict.Add("MaintenanceUpgrade", "2320");
      KeyToIdDict.Add("WorkforceAmountUpgrade", "12337");
      KeyToIdDict.Add("ReplacingWorkforce", "12480");
      KeyToIdDict.Add("ModuleLimitPercent", "12075");
      KeyToIdDict.Add("AdditionalHappiness", "12314");
      KeyToIdDict.Add("ResidentsUpgrade", "2322");
      KeyToIdDict.Add("StressUpgrade", "2323");
      KeyToIdDict.Add("ProvideElectricity", "12485");
      KeyToIdDict.Add("ProvideIndustrialization", "12485");
      KeyToIdDict.Add("TaxModifierInPercent", "12677");
      KeyToIdDict.Add("WorkforceModifierInPercent", "12676");
      KeyToIdDict.Add("MaxHitpointsUpgrade", "2333");

      KeyToIdDict.Add("BlockBuyShare", "15802");
      KeyToIdDict.Add("BlockHostileTakeover", "15801");
      KeyToIdDict.Add("MaintainanceUpgrade", "2320");
      KeyToIdDict.Add("MoralePowerUpgrade", "15231");
      //Update 04
      KeyToIdDict.Add("MinPickupTimeUpgrade", "22219");

      KeyToIdDict.Add("MaxPickupTimeUpgrade", "22219");
      KeyToIdDict.Add("ScrapAmountLevelUpgrade", "22220");

      KeyToIdDict.Add("ConstructionCostInPercent", "12679");
      KeyToIdDict.Add("ConstructionTimeInPercent", "12678");
      KeyToIdDict.Add("PassiveTradeGoodGenUpgrade", "12920");
      KeyToIdDict.Add("AddAssemblyOptions", "12693");
      KeyToIdDict.Add("AssemblyOptions", "12693");
      KeyToIdDict.Add("MoraleDamage", "21588");
      KeyToIdDict.Add("HitpointDamage", "21587");
      KeyToIdDict.Add("SpecialUnitHappinessThresholdUpgrade", "19625");
      KeyToIdDict.Add("HappinessIgnoresMorale", "15811");
      KeyToIdDict.Add("ResolverUnitMovementSpeedUpgrade", "12014");
      KeyToIdDict.Add("IncidentIllnessIncreaseUpgrade", "12226");
      KeyToIdDict.Add("IncidentArcticIllnessIncreaseUpgrade", "22982");
      KeyToIdDict.Add("ActiveTradePriceInPercent", "15198");
      KeyToIdDict.Add("ForwardSpeedUpgrade", "2339");
      KeyToIdDict.Add("IgnoreWeightFactorUpgrade", "15261");
      KeyToIdDict.Add("IgnoreDamageFactorUpgrade", "15262");
      KeyToIdDict.Add("AttackRangeUpgrade", "12021");
      KeyToIdDict.Add("ActivateWhiteFlag", "19538");
      KeyToIdDict.Add("ActivatePirateFlag", "17392");
      KeyToIdDict.Add("Normal", "19136");
      KeyToIdDict.Add("Cannon", "19138");
      KeyToIdDict.Add("BigBertha", "19139");
      KeyToIdDict.Add("Torpedo", "19137");
      KeyToIdDict.Add("AttackSpeedUpgrade", "17230");
      KeyToIdDict.Add("SpawnProbabilityFactor", "20603");
      KeyToIdDict.Add("SelfHealUpgrade", "15195");
      KeyToIdDict.Add("SelfHealPausedTimeIfAttackedUpgrade", "15196");
      KeyToIdDict.Add("HealRadiusUpgrade", "15264");
      KeyToIdDict.Add("HealPerMinuteUpgrade", "15265");
      KeyToIdDict.Add("IncidentRiotIncreaseUpgrade", "12227");
      KeyToIdDict.Add("PublicServiceFullSatisfactionDistance", "2321");
      KeyToIdDict.Add("NeedProvideNeedUpgrade", "12315");
      KeyToIdDict.Add("AdditionalMoney", "12690");
      KeyToIdDict.Add("AdditionalHeat", "116350");
      KeyToIdDict.Add("AdditionalSupply", "22286");
      KeyToIdDict.Add("IncidentFireIncreaseUpgrade", "12225");
      KeyToIdDict.Add("IncidentExplosionIncreaseUpgrade", "21489");
      KeyToIdDict.Add("GoodConsumptionUpgrade", "21386");
      KeyToIdDict.Add("LoadingSpeedUpgrade", "15197");
      KeyToIdDict.Add("ActionDuration", "2423");
      KeyToIdDict.Add("ActionCooldown", "2424");
      KeyToIdDict.Add("IsDestroyedAfterCooldown", "2421");
      KeyToIdDict.Add("LineOfSightRangeUpgrade", "15266");
      KeyToIdDict.Add("BaseDamageUpgrade", "2334");
      KeyToIdDict.Add("AccuracyUpgrade", "12062");
      KeyToIdDict.Add("PierSpeedUpgrade", "15197");
      KeyToIdDict.Add("HeatRangeUpgrade", "2321");
      KeyToIdDict.Add("Attractiveness", "145011");
      KeyToIdDict.Add("HasPollution", "11279");
      KeyToIdDict.Add("InfluenceRadius", "12504");
      KeyToIdDict.Add("NumOfPiers", "15250");
      KeyToIdDict.Add("LoadingSpeed", "15197");
      KeyToIdDict.Add("MinLoadingTime", "12504");
      KeyToIdDict.Add("AttackRange", "12021");
      KeyToIdDict.Add("LineOfSightRange", "15266");
      KeyToIdDict.Add("ReloadTime", "17230");
      KeyToIdDict.Add("HealRadius", "12504");
      KeyToIdDict.Add("BaseDamage", "2334");
      KeyToIdDict.Add("HealPerMinute", "15196");
      KeyToIdDict.Add("MaxTrainCount", "14865");
      KeyToIdDict.Add("Moderate", "5000000");
      KeyToIdDict.Add("Colony01", "5000001");
      KeyToIdDict.Add("Arctic", "160001");
      KeyToIdDict.Add("Africa", "22146");
      KeyToIdDict.Add("AttractivenessPerSetUpgrade", "269251");
      KeyToIdDict.Add("OverrideSpecialistPool", "269570");
      KeyToIdDict.Add("ProductivityBoostUpgrade", "118000");
      KeyToIdDict.Add("StorageCapacityModifier", "23231");
      KeyToIdDict.Add("SocketCountUpgrade", "269364");
      KeyToIdDict.Add("ElectricityBoostUpgrade", "23266");
      KeyToIdDict.Add("Tractor", "269841");
      KeyToIdDict.Add("Silo", "269957");
      KeyToIdDict.Add("IrrigationCapacityUpgrade", "127395");
      KeyToIdDict.Add("AdditionalResearch", "127425");
      KeyToIdDict.Add("PipeCapacityUpgrade", "127395");
      KeyToIdDict.Add("AirRegenerationUpgrade100", "138790");
      KeyToIdDict.Add("Air", "138790");
      KeyToIdDict.Add("SoilRegenerationUpgrade100", "138789");
      KeyToIdDict.Add("Soil", "138789");
      KeyToIdDict.Add("WaterRegenerationUpgrade100", "138788");
      KeyToIdDict.Add("Water", "138788");
      KeyToIdDict.Add("DeltaValueUpgrade", "24080");
      KeyToIdDict.Add("FiniteResourceRegrowFactorUpgrade", "861");
      KeyToIdDict.Add("FiniteResourceRegrowIntervalUpgrade", "862");
      KeyToIdDict.Add("MaxDynamicFillCapacityUpgrade", "25204");
      KeyToIdDict.Add("MaxWorkerAmountUpgrade", "24047");

      // Not nessesarry with update 14
      //KeyToIdDict.Add("PerkFormerPirate", "3930");
      //KeyToIdDict.Add("PerkDiver", "3931");
      //KeyToIdDict.Add("PerkZoologist", "9998");
      //KeyToIdDict.Add("PerkPolyglot", "12266");
      //KeyToIdDict.Add("PerkHypnotist", "3929");
      //KeyToIdDict.Add("PerkAnthropologist", "3928");
      //KeyToIdDict.Add("PerkJackOfAllTraits", "12260");
      //KeyToIdDict.Add("PerkArcheologist", "12262");

      //Override Allocation Tradeship
      KeyToIdDict["Tradeship"] = "12006";
    }

    private static void SetTourismThresholds() {
      Program.ConsoleWriteHeadline("Setting up Tourism");
      var AttractivenessFeature = All.Descendants("Asset").FirstOrDefault(l => l.Element("Template")?.Value == "AttractivenessFeature");
      var CityStatis = AttractivenessFeature.XPathSelectElement("Values/AttractivenessFeature/AttractivenessLevel").Elements().ToList();
      foreach (var stati in CityStatis) {
        TourismThresholds.Add(stati.Name.LocalName, stati.Element("AttractivenessThreshold").Value);
      }
    }

    private static void SetBuffs() {
      Program.ConsoleWriteHeadline("Setting up Buffs");
      var buffs = GUIDs.Values
         .Where(l => l.Element("Template")?.Value.EndsWith("Buff") ?? false)
         .Select(l => new Asset(l, false));
      foreach (var item in buffs) {
        Buffs[item.ID] = item;
      }
    }

    #endregion Private Methods
  }
}