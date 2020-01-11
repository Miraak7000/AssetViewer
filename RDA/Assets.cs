using RDA.Library;
using RDA.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA {
  public static class Assets {
    #region Methods
    static Assets() {
      Original = XmlLoader.LoadXml(Program.PathRoot + @"\Original\assets.xml");
    }

    public static void Init(string version = "Release") {
      Version = version;
      LoadDefaultValues();
      Program.ConsoleWriteHeadline("Load Asset.xml");
      
      SolveXmlInheritance();
      LoadDescriptions();
      LoadCustomDescriptions();
      SetTextDictionarys();
      SetIcons();
      SetTourismStati();
      SetBuffs();
    }

    public static string GetDescriptionID(string id) {
      return KeyToIdDict[id];
    }

    #endregion Methods

    #region Fields

    internal readonly static XElement Original;

    internal readonly static Dictionary<string, XElement> DefaultValues = new Dictionary<string, XElement>();

    internal readonly static Dictionary<string, Dictionary<Languages, string>> Descriptions = new Dictionary<string, Dictionary<Languages, string>>();

    internal readonly static Dictionary<string, Dictionary<Languages, string>> CustomDescriptions = new Dictionary<string, Dictionary<Languages, string>>();

    internal static string Version = "Release";

    internal readonly static Dictionary<string, XElement> TourismStati = new Dictionary<string, XElement>();

    internal readonly static Dictionary<string, Asset> Buffs = new Dictionary<string, Asset>();

    internal readonly static Dictionary<string, string> Icons = new Dictionary<string, string>();

    internal readonly static Dictionary<string, string> KeyToIdDict = new Dictionary<string, string>();

    #endregion Fields

    private static void LoadDefaultValues() {
      Program.ConsoleWriteHeadline("Load Standart Values");
      var xml = XmlLoader.LoadXml(Program.PathRoot + @"\Original\properties.xml");
      foreach (var item in xml.Descendants("DefaultValues").SelectMany(dv => dv.Elements())) {
        DefaultValues.Add(item.Name.LocalName, item);
      }
    }

    private static int InheritDepth(this XElement ele) {
      int depth = 0;
      var search = ele.Element("BaseAssetGUID")?.Value;
      while (search != null) {
        var founded = Original.Descendants("Asset").FirstOrDefault(a => a.XPathSelectElement("Values/Standard/GUID")?.Value == search);
        if (founded != null) {
          depth++;
          search = founded.Element("BaseAssetGUID")?.Value;
        }
      }
      return depth;
    }

    private static void SolveXmlInheritance() {
      Program.ConsoleWriteHeadline("Solve Xml Inheritance");
      var InheritHelper = Original.Descendants("Asset").OrderBy(a => a.InheritDepth()).ToArray();
      foreach (var item in InheritHelper) {
        var baseGuid = item.Element("BaseAssetGUID")?.Value;
        if (baseGuid != null) {
          var baseAsset = Original.Descendants("Asset").FirstOrDefault(a => a.XPathSelectElement("Values/Standard/GUID")?.Value == baseGuid);
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

          // walkaround to fix the problem that rarity "common" and "uncommon" are translated to the same Chinese word
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
        var filepath = Program.PathRoot + $@"\Modified\LanguageFiles\Texts_Custom_{language.ToString("G")}.json";
        if (File.Exists(filepath)) {
          dynamic dic = js.Deserialize<dynamic>(File.ReadAllText(filepath));
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
      var asset = Original
         .Descendants("Asset")
         .FirstOrDefault(a => a.Element("Template")?.Value == "ItemBalancing")?
         .Element("Values")
         .Element("ItemConfig");
      //AllocationIcons
      foreach (var item in asset.Element("AllocationIcons").Elements()) {
        if (item.Element("AllocationIcon")?.Value is string val) {
          Icons[item.Element("Allocation").Value] = val;
        }
      }

      var texts = Original
        .Descendants("Asset")
        .Where(a => a.XPathSelectElement("Values/Text/LocaText")?.HasElements == true && a.XPathSelectElement("Values/Standard/IconFilename")?.Value != null).Select(e => e.Element("Values").Element("Standard"));
      //TextIcons
      foreach (var item in texts) {
        Icons[item.Element("GUID").Value] = item.Element("IconFilename").Value;
      }

      Icons.Add("Ship", "data/ui/2kimages/main/3dicons/icon_ship.png");
      Icons.Add("Warship", "data/ui/2kimages/main/3dicons/ships/icon_ship_battlecruiser.png");
      Icons.Add("RadiusBuilding", "data/ui/2kimages/main/3dicons/icon_guildhouse.png");
      Icons.Add("12508", "data/ui/2kimages/main/icons/icon_electricity.png");         //Need Elektrizität
      Icons.Add("15798", "data/ui/2kimages/main/icons/icon_generic_expedition.png");  //PerkMale
      Icons.Add("15797", "data/ui/2kimages/main/icons/icon_generic_expedition.png");  //PerkFeMale
    }

    private static void SetTextDictionarys() {
      Program.ConsoleWriteHeadline("Setting up Descriptions");
      var asset = Original
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

      asset = Original
       .Descendants("Asset")
       .FirstOrDefault(a => a.Element("Template")?.Value == "ExpeditionFeature")?
       .Element("Values")
       .Element("ExpeditionFeature");
     
      //ExpeditionRegions
      //foreach (var item in asset.Element("ExpeditionRegions").Elements()) {
      //  KeyToIdDict.Add(item.Name.LocalName, item.Element("Region").Value);
      //}

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
      //
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

      //Override Allocation Tradeship
      KeyToIdDict["Tradeship"] = "12006";
    }

    private static void SetTourismStati() {
      Program.ConsoleWriteHeadline("Setting up Tourism");
      var TourismAsset = Original.Descendants("Asset").FirstOrDefault(l => l.Element("Template")?.Value == "TourismFeature");
      var CityStatis = TourismAsset.XPathSelectElement("Values/TourismFeature/CityStati").Elements().ToList();
      for (var i = 1; i < CityStatis.Count; i++) {
        TourismStati[i.ToString()] = CityStatis[i - 1];
      }
    }

    private static void SetBuffs() {
      Program.ConsoleWriteHeadline("Setting up Buffs");
      var buffs = Original
         .Descendants("Asset")
         .Where(l => l.Element("Template")?.Value.EndsWith("Buff") ?? false)
         .Select(l => new Asset(l, false));
      foreach (var item in buffs) {
        Buffs[item.ID] = item;
      }
    }
  }
}