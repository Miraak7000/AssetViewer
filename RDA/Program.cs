using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;
using RDA.Templates;

namespace RDA {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  internal static class Program {

    #region Fields
    private static readonly Dictionary<Int32, String> Descriptions = new Dictionary<Int32, String>();
    internal static XDocument Modified;
    internal static XDocument Original;
    internal static String PathRoot;
    internal static String PathViewer;
    private static readonly List<XElement> RewardPoolList = new List<XElement>();
    internal static XDocument TextDE;
    internal static Dictionary<String, String> DescriptionEN;
    internal static Dictionary<String, String> DescriptionDE;
    #endregion

    #region Private Methods
    private static void Main(String[] args) {
      Program.PathViewer = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.Parent.FullName + @"\AssetViewer";
      Program.PathRoot = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.FullName;
      Program.Original = XDocument.Load(Program.PathRoot + @"\Original\assets.xml");
      //Program.TextDE = XDocument.Load(Program.PathRoot + @"\Original\texts_german.xml");
      // Helper
      //Helper.ExtractTextEnglish(Program.PathRoot + @"\Original\assets.xml");
      //Helper.ExtractTextGerman(Program.PathRoot + @"\Original\texts_german.xml");
      //Helper.ExtractTemplateNames(Program.PathRoot + @"\Original\assets.xml");
      // Descriptions
      Program.DescriptionEN = XDocument.Load(Program.PathRoot + @"\Modified\Texts_English.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
      Program.DescriptionDE = XDocument.Load(Program.PathRoot + @"\Modified\Texts_German.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
      // World Fair
      //Monument.Create();
      // Create Assets
      Program.Processing("GuildhouseItem");
      // 
      //Program.GetDescriptions(Program.Original.Root);
      //Program.CleanupGuildhouseItems();
      //Program.WriteHtml();
      //
      //Program.GetGuildhouseItems(Program.Original.Root);
      //Program.GuildhouseItems = Program.GuildhouseItems.OrderBy(o => o.GUID).ToList();
      // 
      //Program.MonumentEventCategory();

      // Processing
      //var templates = new String[] { "GuildhouseItem" };
      //foreach (var template in templates) {
      //  Program.Processing(Program.PathRoot, template);
      //}
      //Program.ProcessExpedition(pathRoot);
      // Finish
      //Program.Blacklist(Program.Original.Root);
      //for (int x = 0; x < 20; x++) {
      //  Program.Cleanup(Program.Original.Root);
      //}
      //Program.Original.Save(pathRoot + @"\Modified\Assets.xml");
    }
    private static void Processing(String template) {
      var result = new List<Asset>();
      var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}']");
      foreach (var asset in assets) {
        if (asset.XPathSelectElement("Values/Item/HasAction")?.Value == "1") continue;
        var item = new Asset(asset);
        var o1 = item.ToString();
        result.Add(item);
      }
    }
    private static void ProcessExpedition(String pathRoot) {
      Program.Modified = new XDocument();
      Program.Modified.Add(new XElement("Expedition"));
      foreach (var element in Program.Original.Root.Elements().ToArray()) {
        Program.Find(element, "Expedition");
      }
      Program.Modified.Save(pathRoot + @"\Modified\Assets_Expedition.xml");
    }
    private static void Find(XElement element, String text) {
      if (element.Name == "Assets" && element.HasElements) {
        foreach (var asset in element.Elements("Asset").ToArray()) {
          if (asset.Element("Template")?.Value == text) {
            Program.Modified.Root.Add(asset);
            asset.Remove();
          }
        }
      }
      if (element.HasElements) {
        foreach (var next in element.Elements().ToArray()) {
          Program.Find(next, text);
        }
      } else {
        if (element.Name == "Assets" || element.Name == "Group" || element.Name == "Groups") element.Remove();
      }
    }
    private static void Blacklist(XElement root) {
      var parent = root.Parent;
      var blacklist = new[] {
        "AssetPool",
        "NotificationConfiguration",
        "GlobalSoundBankConfiguration",
        "GameParameter",
        "TownhallBuff",
        "SessionModerateRandom",
        "SessionSouthAmerica",
        "TriggerCampaign",
        "PopulationLevel7",
        "Transporter",
        "PlayerCounterContextPool",
        "LockableNotification",
        "NonCriticalError",
        "UplayProduct",
        "Achievement",
        "MapTemplate",
        "SoundBank",
        "SessionModerate",
        "MainQuest",
        "PaMSy_Base",
        "TrafficFeedbackUnit",
        "PopulationGroup7",
        "Product",
        "QuestObjectAttractiveness",
        "WorldMapShip",
        "TestData_Prop",
        "FeedbackVehicle",
        "Projectile",
        "MetaGameObjectReference",
        "FishShoal",
        "Inhabitant",
        "Bird",
        "AudioText",
        "TextPool",
        "Notification",
        "CriticalError",
        "UplayReward",
        "UplayAction",
        "Video",
        "RFX",
        "Audio",
        "BridgeBuilding",
        "HarborPropObject",
        "BuildPermitBuilding",
        "OrnamentalBuilding",
        "WorkforceConnector",
        "HarborWarehouseStrategic",
        "VisitorPier",
        "HarborLandingStage7",
        "RepairCrane",
        "HarborBuildingAttacker",
        "Shipyard",
        "HarborDepot",
        "HarborWarehouse7",
        "StreetBuilding",
        "Street",
        "Monument",
        "CultureModule",
        "CultureBuilding",
        "Warehouse",
        "Market",
        "CityInstitutionBuilding",
        "PublicServiceBuilding",
        "OilPumpBuilding",
        "WorkArea",
        "Slot",
        "Farmfield",
        "TextSourceFormatting",
        "NoLocaText",
        "TrackingValue",
        "ResidenceBuilding7",
        "Fish",
        "VisualSoundEmitter",
        "VisualObjectEditor",
        "VisualObject",
        "Herd",
        "Camera",
        "AudioSpots",
        "CameraSequenceMeta",
        "SimpleVehicle",
        "Collectable",
        "Portrait",
        "Dying",
        "ExplodingProjectile",
        "WorldMapGlobe"
      };
      if (root.Name == "Asset" && blacklist.Contains(root.Element("Template")?.Value)) {
        root.Remove();
        while (!parent.HasElements) {
          var self = parent;
          parent = parent.Parent;
          self.Remove();
        }
        return;
      }
      foreach (var element in root.Elements().ToArray()) {
        Program.Blacklist(element);
      }
    }
    private static void Cleanup(XElement root) {
      if (root.Name == "Assets" && !root.HasElements) root.Remove();
      if (root.Name == "Group" && !root.HasElements) root.Remove();
      if (root.Name == "Group" && root.Elements().Count() == 1 && root.Elements().Single().Name == "GUID") root.Remove();
      if (root.Name == "Groups" && !root.HasElements) root.Remove();
      foreach (var element in root.Elements().ToArray()) {
        Program.Cleanup(element);
      }
    }
    //
    private static XElement FindElement(XElement element, String guid) {
      if (element.Name == "GUID" && element.Value == guid && element.Parent.Name == "Standard") return element.Parent.Parent.Parent;
      foreach (var next in element.Elements()) {
        var result = Program.FindElement(next, guid);
        if (result != null) return result;
      }
      return null;
    }
    private static String GetDescriptionID(XElement element) {
      String textID = null;
      switch (element.Element("Template").Value) {
        case "BuildPermitBuilding":
          textID = element.Element("Values").Element("Ornament").Element("OrnamentDescritpion").Value;
          break;
        case "GuildhouseItem":
        case "HarborOfficeItem":
        case "VehicleItem":
        case "ActiveItem":
        case "CultureItem":
          textID = element.Element("Values").Element("Standard").Element("InfoDescription").Value;
          break;
        default:
          throw new NotImplementedException();
      }
      return textID;
    }
    private static String GetDescription(String guid) {
      if (guid == null) return String.Empty;
      return Program.Descriptions[Int32.Parse(guid)];
    }
    private static XElement AddElement(XDocument document, String node) {
      var element = new XElement(node);
      document.Add(element);
      return element;
    }
    private static XElement AddElement(XElement parent, String node, Object content = null) {
      var element = new XElement(node, content);
      parent.Add(element);
      return element;
    }
    // 
    private static void GetDescriptions(XElement element) {
      if (element.Name == "Asset" && element.Element("Template")?.Value == "Text") {
        if (!element.Element("Values").Element("Text").HasElements) return;
        if (element.Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text") == null) return;
        var guid = Int32.Parse(element.Element("Values").Element("Standard").Element("GUID").Value);
        var text = element.Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text").Value;
        if (!Program.Descriptions.ContainsKey(guid)) Program.Descriptions.Add(guid, text);
      } else {
        foreach (var next in element.Elements()) {
          Program.GetDescriptions(next);
        }
      }
    }
    // 
    private static void CleanupGuildhouseItems() {
      var document = XDocument.Load(Program.PathRoot + @"\Modified\Assets_GuildhouseItem.xml");
      foreach (var asset in document.Root.Elements().ToArray()) {
        String textEN;
        String textDE;
        String descriptionID;
        // skip unwanted items
        if (asset.Element("Values").Element("Item").Element("IsDestroyedOnUnequip") != null) {
          asset.Remove();
          continue;
        }
        if (asset.Element("Values").Element("Item").Element("HasAction") != null) {
          asset.Remove();
          continue;
        }
        if (asset.Element("Values").Element("Item").Element("Allocation")?.Value == "RadiusBuilding") {
          asset.Remove();
          continue;
        }
        // modify name and text
        textEN = asset.Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text").Value;
        asset.Element("Values").Element("Text").AddBeforeSelf(new XElement("Description"));
        asset.Element("Values").Element("Description").Add(new XElement("EN"));
        asset.Element("Values").Element("Description").Element("EN").Add(new XElement("Short", textEN));
        textDE = Program.TextDE.Root.Element("Texts").Elements().Where(w => w.Element("GUID").Value == asset.Element("Values").Element("Standard").Element("GUID").Value).Select(s => s.Element("Text").Value).SingleOrDefault();
        if (textDE != null) {
          asset.Element("Values").Element("Description").Add(new XElement("DE"));
          asset.Element("Values").Element("Description").Element("DE").Add(new XElement("Short", textDE));
        }
        descriptionID = asset.Element("Values").Element("Standard").Element("InfoDescription")?.Value;
        if (descriptionID != null) {
          var description = Program.Descriptions[Int32.Parse(descriptionID)];
          asset.Element("Values").Element("Description").Element("EN").Add(new XElement("Long", description));
          textDE = Program.TextDE.Root.Element("Texts").Elements().Where(w => w.Element("GUID").Value == descriptionID).Select(s => s.Element("Text").Value).SingleOrDefault();
          if (textDE != null) {
            asset.Element("Values").Element("Description").Add(new XElement("DE"));
            asset.Element("Values").Element("Description").Element("DE").Add(new XElement("Long", textDE));
          }
        }
        asset.Element("Values").Element("Text").Remove();
        // modify targets
        foreach (var item in asset.Element("Values").Element("ItemEffect").Element("EffectTargets").Elements()) {
          var target = Program.FindElement(Program.Original.Root, item.Element("GUID").Value);
          if (target.Element("BaseAssetGUID") != null) target = Program.FindElement(Program.Original.Root, target.Element("BaseAssetGUID").Value);
          textEN = target.Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text").Value;
          item.Add(new XElement("Description"));
          item.Element("Description").Add(new XElement("EN"));
          item.Element("Description").Element("EN").Add(new XElement("Short", textEN));
          textDE = Program.TextDE.Root.Element("Texts").Elements().Where(w => w.Element("GUID").Value == item.Element("GUID").Value).Select(s => s.Element("Text").Value).SingleOrDefault();
          if (textDE != null) {
            item.Element("Description").Add(new XElement("DE"));
            item.Element("Description").Element("DE").Add(new XElement("Short", textDE));
          }
        }
        // modify fertility
        var fertility = asset.Element("Values").Element("FactoryUpgrade")?.Element("AddedFertility");
        if (fertility != null) {
          var target = Program.FindElement(Program.Original.Root, fertility.Value);
          fertility.Value = String.Empty;
          fertility.Add(new XElement("GUID", target.Element("Values").Element("Standard").Element("GUID").Value));
          textEN = target.Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text").Value;
          fertility.Add(new XElement("Description"));
          fertility.Element("Description").Add(new XElement("EN"));
          fertility.Element("Description").Element("EN").Add(new XElement("Short", textEN));
          textDE = Program.TextDE.Root.Element("Texts").Elements().Where(w => w.Element("GUID").Value == target.Element("Values").Element("Standard").Element("GUID").Value).Select(s => s.Element("Text").Value).SingleOrDefault();
          if (textDE != null) {
            fertility.Element("Description").Add(new XElement("DE"));
            fertility.Element("Description").Element("DE").Add(new XElement("Short", textDE));
          }
        }
        // modify additional output
        var output = asset.Element("Values").Element("FactoryUpgrade")?.Element("AdditionalOutput");
        if (output != null) {
          foreach (var item in output.Elements()) {
            var target = Program.FindElement(Program.Original.Root, item.Element("Product").Value);
            item.Element("Product").Value = String.Empty;
            item.Element("Product").Add(new XElement("GUID", target.Element("Values").Element("Standard").Element("GUID").Value));
            textEN = target.Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text").Value;
            item.Element("Product").Add(new XElement("Description"));
            item.Element("Product").Element("Description").Add(new XElement("EN"));
            item.Element("Product").Element("Description").Element("EN").Add(new XElement("Short", textEN));
            textDE = Program.TextDE.Root.Element("Texts").Elements().Where(w => w.Element("GUID").Value == target.Element("Values").Element("Standard").Element("GUID").Value).Select(s => s.Element("Text").Value).SingleOrDefault();
            if (textDE != null) {
              item.Element("Product").Element("Description").Add(new XElement("DE"));
              item.Element("Product").Element("Description").Element("DE").Add(new XElement("Short", textDE));
            }
          }
        }
        // modify replace inputs
        var replacement = asset.Element("Values").Element("FactoryUpgrade")?.Element("ReplaceInputs");
        if (replacement != null) {
          foreach (var item in replacement.Elements()) {
            {
              var target = Program.FindElement(Program.Original.Root, item.Element("OldInput").Value);
              item.Element("OldInput").Value = String.Empty;
              item.Element("OldInput").Add(new XElement("GUID", target.Element("Values").Element("Standard").Element("GUID").Value));
              textEN = target.Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text").Value;
              item.Element("OldInput").Add(new XElement("Description"));
              item.Element("OldInput").Element("Description").Add(new XElement("EN"));
              item.Element("OldInput").Element("Description").Element("EN").Add(new XElement("Short", textEN));
              textDE = Program.TextDE.Root.Element("Texts").Elements().Where(w => w.Element("GUID").Value == target.Element("Values").Element("Standard").Element("GUID").Value).Select(s => s.Element("Text").Value).SingleOrDefault();
              if (textDE != null) {
                item.Element("OldInput").Element("Description").Add(new XElement("DE"));
                item.Element("OldInput").Element("Description").Element("DE").Add(new XElement("Short", textDE));
              }
            }
            {
              var target = Program.FindElement(Program.Original.Root, item.Element("NewInput").Value);
              item.Element("NewInput").Value = String.Empty;
              item.Element("NewInput").Add(new XElement("GUID", target.Element("Values").Element("Standard").Element("GUID").Value));
              textEN = target.Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text").Value;
              item.Element("NewInput").Add(new XElement("Description"));
              item.Element("NewInput").Element("Description").Add(new XElement("EN"));
              item.Element("NewInput").Element("Description").Element("EN").Add(new XElement("Short", textEN));
              textDE = Program.TextDE.Root.Element("Texts").Elements().Where(w => w.Element("GUID").Value == target.Element("Values").Element("Standard").Element("GUID").Value).Select(s => s.Element("Text").Value).SingleOrDefault();
              if (textDE != null) {
                item.Element("NewInput").Element("Description").Add(new XElement("DE"));
                item.Element("NewInput").Element("Description").Element("DE").Add(new XElement("Short", textDE));
              }
            }
          }
        }
      }
      document.Save(Program.PathRoot + @"\Modified\Assets_GuildhouseItemCleanedUp.xml");
    }
    #endregion

  }

}