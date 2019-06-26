using RDA.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA {

  public static class Assets {

    #region Methods

    public static void Init(string version = "Release") {
      Console.WriteLine("Load Asset.xml");
      Original = XDocument.Load(Program.PathRoot + @"\Original\assets.xml");
      // Descriptions
      DescriptionEN = XDocument.Load(Program.PathRoot + @"\Modified\Texts_English.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
      DescriptionDE = XDocument.Load(Program.PathRoot + @"\Modified\Texts_German.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
      SetTextDictionarys();
      SetIcons();
      SetTourismStati();
      SetBuffs();
    }

    public static string GetDescriptionID(string id) {
      return Descriptions[id];
    }

    #endregion Methods

    #region Fields

    internal static XDocument Original;
    internal static Dictionary<String, String> DescriptionEN;
    internal static Dictionary<String, String> DescriptionDE;
    internal static string Version = "Release";
    internal static Dictionary<string, XElement> TourismStati = new Dictionary<string, XElement>();
    internal static Dictionary<string, Asset> Buffs = new Dictionary<string, Asset>();
    internal static Dictionary<string, string> Icons = new Dictionary<string, string>();
    internal static Dictionary<string, string> Descriptions = new Dictionary<string, string>();

    #endregion Fields

    private static void SetIcons() {
      Console.WriteLine("Setting up Icons");
      var asset = Original
         .Root
         .Descendants("Asset")
         .FirstOrDefault(a => a.Element("Template")?.Value == "ItemBalancing")
         .Element("Values")
         .Element("ItemConfig");
      //AllocationIcons
      foreach (var item in asset.Element("AllocationIcons").Elements()) {
        Icons[item.Element("Allocation").Value] = item.Element("AllocationIcon").Value;
      }

      var texts = Original
        .Root
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
      Console.WriteLine("Setting up Descriptions");
      var asset = Original
         .Root
         .Descendants("Asset")
         .FirstOrDefault(a => a.Element("Template")?.Value == "ItemBalancing")
         .Element("Values")
         .Element("ItemConfig");
      //RarityText
      foreach (var item in asset.Element("RarityText").Elements()) {
        Descriptions.Add(item.Name.LocalName, item.Element("Text").Value);
      }
      //ExclusiveGroupText
      foreach (var item in asset.Element("ExclusiveGroupText").Elements()) {
        Descriptions.Add(item.Name.LocalName, item.Element("Text").Value);
      }
      //ExclusiveGroupText
      foreach (var item in asset.Element("AllocationText").Elements()) {
        Descriptions.Add(item.Name.LocalName, item.Element("Text").Value);
      }

      asset = Original
       .Root
       .Descendants("Asset")
       .FirstOrDefault(a => a.Element("Template")?.Value == "ExpeditionFeature")
       .Element("Values")
       .Element("ExpeditionFeature");
      //ExpeditionRegions
      foreach (var item in asset.Element("ExpeditionRegions").Elements()) {
        Descriptions.Add(item.Name.LocalName, item.Element("Region").Value);
      }
      //AttributeNames
      foreach (var item in asset.Element("AttributeNames").Elements()) {
        Descriptions.Add(item.Name.LocalName, item.Element("Name").Value);
      }

      Descriptions.Add("ProductivityUpgrade", "118000");
      Descriptions.Add("AdditionalOutput", "20074");
      Descriptions.Add("ReplaceInputs", "20081");
      Descriptions.Add("InputAmountUpgrade", "100369");
      Descriptions.Add("OutputAmountFactorUpgrade", "11989");
      Descriptions.Add("NeededAreaPercentUpgrade", "15319");
      Descriptions.Add("NeedsElectricity", "12508");
      Descriptions.Add("PerkFemale", "15798");
      Descriptions.Add("PerkMale", "15797");
      Descriptions.Add("AttractivenessUpgrade", "145011");
      Descriptions.Add("MaintenanceUpgrade", "2320");
      Descriptions.Add("WorkforceAmountUpgrade", "12337");
      Descriptions.Add("ReplacingWorkforce", "12480");
      Descriptions.Add("ModuleLimitUpgrade", "12075");
      Descriptions.Add("AdditionalHappiness", "12314");
      Descriptions.Add("ResidentsUpgrade", "2322");
      Descriptions.Add("StressUpgrade", "12227");
      Descriptions.Add("ProvideElectricity", "12485");
      Descriptions.Add("TaxModifierInPercent", "12677");
      Descriptions.Add("WorkforceModifierInPercent", "12676");
      Descriptions.Add("MaxHitpointsUpgrade", "2333");
    }
    private static void SetTourismStati() {
      Console.WriteLine("Setting up Tourism");
      var TourismAsset = Original.Root.Descendants("Asset").FirstOrDefault(l => l.Element("Template")?.Value == "TourismFeature");
      var CityStatis = TourismAsset.XPathSelectElement("Values/TourismFeature/CityStati").Elements().ToList();
      TourismStati = new Dictionary<string, XElement>();
      for (var i = 1; i < CityStatis.Count; i++) {
        TourismStati[i.ToString()] = CityStatis[i - 1];
      }
    }
    private static void SetBuffs() {
      Console.WriteLine("Setting up Buffs");
      Buffs = Original
         .Root
         .Descendants("Asset")
         .Where(l => l.Element("Template")?.Value.EndsWith("Buff") ?? false)
         .Select(l => new Asset(l, false))
         .ToDictionary(b => b.ID);
    }
  }
}