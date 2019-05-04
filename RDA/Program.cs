using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using DmitryBrant.ImageFormats;
using RDA.Data;

namespace RDA {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  internal static class Program {

    #region Fields
    private static String PathRoot;
    private static XDocument Original;
    private static XDocument Modified;
    private static XDocument TextDE;
    private static readonly List<XElement> RewardPoolList = new List<XElement>();
    private static readonly List<Asset> MonumentEventCategories = new List<Asset>();
    private static List<GuildhouseItem> GuildhouseItems = new List<GuildhouseItem>();
    private static Dictionary<Int32, String> Descriptions = new Dictionary<Int32, String>();
    #endregion

    #region Private Methods
    private static void Main(String[] args) {
      Program.PathRoot = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.FullName;
      Program.Original = XDocument.Load(Program.PathRoot + @"\Original\assets.xml");
      Program.TextDE = XDocument.Load(Program.PathRoot + @"\Original\texts_german.xml");
      // DecodeImage
      //Program.DecodeImage("data/ui/2kimages/main/3dicons/icon_architecture.png");
      // World Fair
      //Program.MonumentEventCategory();
      //Program.MonumentEvent();
      Program.MonumentEventReward();
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
    private static void Processing(String pathRoot, String template) {
      Program.Modified = new XDocument();
      Program.Modified.Add(new XElement(template));
      foreach (var element in Program.Original.Root.Elements().ToArray()) {
        Program.Find(element, template);
      }
      Program.Modified.Save(pathRoot + $@"\Modified\Assets_{template}.xml");
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
        if (element.Name == "Assets" || element.Name == "Group" || element.Name == "Groups") {
          element.Remove();
        }
      }
    }
    private static void Blacklist(XElement root) {
      var parent = root.Parent;
      var blacklist = new String[] { "AssetPool", "NotificationConfiguration", "GlobalSoundBankConfiguration", "GameParameter", "TownhallBuff", "SessionModerateRandom", "SessionSouthAmerica", "TriggerCampaign", "PopulationLevel7", "Transporter", "PlayerCounterContextPool", "LockableNotification", "NonCriticalError", "UplayProduct", "Achievement", "MapTemplate", "SoundBank", "SessionModerate", "MainQuest", "PaMSy_Base", "TrafficFeedbackUnit", "PopulationGroup7", "Product", "QuestObjectAttractiveness", "WorldMapShip", "TestData_Prop", "FeedbackVehicle", "Projectile", "MetaGameObjectReference", "FishShoal", "Inhabitant", "Bird", "AudioText", "TextPool", "Notification", "CriticalError", "UplayReward", "UplayAction", "Video", "RFX", "Audio", "BridgeBuilding", "HarborPropObject", "BuildPermitBuilding", "OrnamentalBuilding", "WorkforceConnector", "HarborWarehouseStrategic", "VisitorPier", "HarborLandingStage7", "RepairCrane", "HarborBuildingAttacker", "Shipyard", "HarborDepot", "HarborWarehouse7", "StreetBuilding", "Street", "Monument", "CultureModule", "CultureBuilding", "Warehouse", "Market", "CityInstitutionBuilding", "PublicServiceBuilding", "OilPumpBuilding", "WorkArea", "Slot", "Farmfield", "TextSourceFormatting", "NoLocaText", "TrackingValue", "ResidenceBuilding7", "Fish", "VisualSoundEmitter", "VisualObjectEditor", "VisualObject", "Herd", "Camera", "AudioSpots", "CameraSequenceMeta", "SimpleVehicle", "Collectable", "Portrait", "Dying", "ExplodingProjectile", "WorldMapGlobe" };
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
      if (root.Name == "Assets" && !root.HasElements) {
        root.Remove();
      }
      if (root.Name == "Group" && !root.HasElements) {
        root.Remove();
      }
      if (root.Name == "Group" && root.Elements().Count() == 1 && root.Elements().Single().Name == "GUID") {
        root.Remove();
      }
      if (root.Name == "Groups" && !root.HasElements) {
        root.Remove();
      }
      foreach (var element in root.Elements().ToArray()) {
        Program.Cleanup(element);
      }
    }
    // Images
    private static String DecodeImage(String icon) {
      icon = icon.Substring(icon.LastIndexOf('/') + 1).Replace(".png", String.Empty);
      var pathImageExe = Program.PathRoot + @"\Library\nvdecompress.exe";
      var pathImageSource = Directory.GetFiles($@"{Program.PathRoot}\Resources\DDS", $"{icon}*.dds", SearchOption.AllDirectories).FirstOrDefault();
      if (pathImageSource != null) {
        var pathImageTemp = $@"{Program.PathRoot}\Resources\temp.tga";
        Process.Start(pathImageExe, $"\"{pathImageSource}\" \"{pathImageTemp}\"");
        Thread.Sleep(1000);
        if (File.Exists(pathImageTemp)) {
          var img = TgaReader.Load(pathImageTemp);
          File.Delete(pathImageTemp);
          String pathImageTarget;
          if (pathImageSource.Contains("3d")) {
            pathImageTarget = $@"{Program.PathRoot}\Resources\PNG\3dicons\{icon}.png";
          } else {
            pathImageTarget = $@"{Program.PathRoot}\Resources\PNG\icons\{icon}.png";
          }
          img.Save(pathImageTarget, ImageFormat.Png);
          return Convert.ToBase64String(File.ReadAllBytes(pathImageTarget));
        }
      }
      return null;
    }
    // World Fair
    private static void MonumentEventCategory() {
      var assets = Program.Original.Root.XPathSelectElements("//Asset[Template='MonumentEventCategory']").ToArray();
      foreach (var asset in assets) {
        var assetGuid = asset.XPathSelectElement("Values/Standard/GUID").Value;
        var textEN = asset.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
        var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={assetGuid}]/Text").Value;
        // modify description
        asset.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
        asset.XPathSelectElement("Values/Text").Remove();
        // modify reward
        var rewardGuid = asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={rewardGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={rewardGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/Description/DE").Add(new XElement("Short", textDE));
        // image
        //var img = Program.DecodeImage(asset.XPathSelectElement("Values/Standard/IconFilename").Value);
        //asset.XPathSelectElement("Values/Standard").Add(new XElement("Icon", img));
        //asset.XPathSelectElement("Values/Standard/IconFilename").Remove();
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentEventCategory", assets));
      document.Save(Program.PathRoot + "/Modified/Assets_MonumentEventCategory.xml");
    }
    private static void MonumentEvent() {
      var assets = Program.Original.Root.XPathSelectElements("//Asset[Template='MonumentEvent']").ToArray();
      foreach (var asset in assets) {
        var assetGuid = asset.XPathSelectElement("Values/Standard/GUID").Value;
        var textEN = asset.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
        var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={assetGuid}]/Text").Value;
        // modify description
        asset.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
        asset.XPathSelectElement("Values/Text").Remove();
        // modify selection text
        var selectionGuid = asset.XPathSelectElement("Values/MonumentEvent/SelectionText").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={selectionGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={selectionGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText/Description/DE").Add(new XElement("Short", textDE));
        // modify running text
        var runningGuid = asset.XPathSelectElement("Values/MonumentEvent/RunningText").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={runningGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={runningGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEvent/RunningText").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEvent/RunningText").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEvent/RunningText/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEvent/RunningText/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEvent/RunningText/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEvent/RunningText/Description/DE").Add(new XElement("Short", textDE));
        // modify reward text
        var rewardGuid = asset.XPathSelectElement("Values/MonumentEvent/RewardText").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={rewardGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={rewardGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEvent/RewardText").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEvent/RewardText").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEvent/RewardText/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEvent/RewardText/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEvent/RewardText/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEvent/RewardText/Description/DE").Add(new XElement("Short", textDE));
        // modify event text
        var eventGuid = asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={eventGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={eventGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/Description/DE").Add(new XElement("Short", textDE));
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentEvent", assets));
      document.Save(Program.PathRoot + "/Modified/Assets_MonumentEvent.xml");
    }
    private static void MonumentEventReward() {
      var assets = Program.Original.Root.XPathSelectElements("//Asset[Template='MonumentEventReward']").ToArray();
      foreach (var asset in assets.Skip(9).Take(1)) {
        var assetGuid = asset.XPathSelectElement("Values/Standard/GUID").Value;
        var textEN = asset.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
        var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={assetGuid}]/Text").Value;
        // modify description
        asset.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
        asset.XPathSelectElement("Values/Text").Remove();
        // find rewards
        foreach (var item in asset.XPathSelectElements("Values/Reward/RewardAssets/Item")) {
          var rewardGuid = item.XPathSelectElement("Reward").Value;
          var reward = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={rewardGuid}]/../..");
          if (reward.XPathSelectElement("Template").Value == "RewardPool") {
            var poolGuids1 = reward.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").Select(s => s.Value).ToArray();
            foreach (var poolGuid1 in poolGuids1) {
              reward = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={poolGuid1}]/../..");
              if (reward.XPathSelectElement("Template").Value == "RewardPool") {
                var poolGuids2 = reward.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").Select(s => s.Value).ToArray();
                foreach (var poolGuid2 in poolGuids2) {
                  reward = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={poolGuid2}]/../..");
                  item.XPathSelectElement("Reward").Value = String.Empty;
                  item.XPathSelectElement("Reward").Add(reward);
                }
              } else {
                item.XPathSelectElement("Reward").Value = String.Empty;
                item.XPathSelectElement("Reward").Add(reward);
              }
            }
          } else {
            item.XPathSelectElement("Reward").Value = String.Empty;
            item.XPathSelectElement("Reward").Add(reward);
          }
        }
        // modify rewards
        foreach (var item in asset.XPathSelectElements("Values/Reward/RewardAssets/Item/Reward/Asset")) {
          switch (item.XPathSelectElement("Template").Value) {
            case "BuildPermitBuilding":
              Program.TemplateBuildPermitBuilding(item);
              break;
            case "GuildhouseItem":
              Program.TemplateGuildhouseItem(item);
              break;
            default:
              throw new NotImplementedException();
          }
        }
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentEventReward", assets));
      document.Save(Program.PathRoot + "/Modified/Assets_MonumentEventReward.xml");
    }
    //
    private static void TemplateBuildPermitBuilding(XElement item) {
      // text
      var itemGuid = item.XPathSelectElement("Values/Standard/GUID").Value;
      var textEN = item.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
      var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
      item.XPathSelectElement("Values/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Text").Remove();
      // ornament
      var ornamentGuid = item.XPathSelectElement("Values/Ornament/OrnamentDescritpion").Value;
      textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={ornamentGuid}]/../Text/LocaText/English/Text").Value;
      textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={ornamentGuid}]/Text").Value;
      item.XPathSelectElement("Values/Ornament").Add(new XElement("Description"));
      item.XPathSelectElement("Values/Ornament/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Ornament/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Ornament/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Ornament/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Ornament/OrnamentDescritpion").Remove();
      // image
      var img = Program.DecodeImage(item.XPathSelectElement("Values/Standard/IconFilename").Value);
      item.XPathSelectElement("Values/Standard").Add(new XElement("Icon", img));
    }
    private static void TemplateGuildhouseItem(XElement item) {
      // text
      var itemGuid = item.XPathSelectElement("Values/Standard/GUID").Value;
      var textEN = item.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
      var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
      item.XPathSelectElement("Values/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Text").Remove();
      // info
      var infoGuid = item.XPathSelectElement("Values/Standard/InfoDescription").Value;
      textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={infoGuid}]/../Text/LocaText/English/Text").Value;
      textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Long", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Long", textDE));
      // image
      var img = Program.DecodeImage(item.XPathSelectElement("Values/Standard/IconFilename").Value);
      item.XPathSelectElement("Values/Standard").Add(new XElement("Icon", img));
    }
    //
    private static void MonumentEventCategoryOld() {
      foreach (var element in Program.Original.Root.Elements()) {
        Program.MonumentEventCategory(element);
      }
      var document = new XDocument();
      document.Add(new XElement("Monument"));
      foreach (var asset1 in Program.MonumentEventCategories) {
        var nodeCategory = Program.AddElement(document.Root, "Category");
        Program.AddElement(nodeCategory, "GUID", asset1.GUID);
        Program.AddElement(nodeCategory, "Name", asset1.Name);
        Program.AddElement(nodeCategory, "Text", asset1.Description);
        Program.AddElement(nodeCategory, "Icon", asset1.IconFilename);
        var nodeEvents = Program.AddElement(nodeCategory, "Events");
        foreach (var asset2 in asset1.Items) {
          var nodeEvent = Program.AddElement(nodeEvents, "Event");
          Program.AddElement(nodeEvent, "GUID", asset2.GUID);
          Program.AddElement(nodeEvent, "Name", asset2.Name);
          Program.AddElement(nodeEvent, "Text", asset2.Description);
          Program.AddElement(nodeEvent, "Icon", asset2.IconFilename);
          var nodeThresholds = Program.AddElement(nodeEvent, "Thresholds");
          foreach (var asset3 in asset2.Items) {
            var nodeThreshold = Program.AddElement(nodeThresholds, "Threshold");
            Program.AddElement(nodeThreshold, "GUID", asset3.GUID);
            Program.AddElement(nodeThreshold, "Name", asset3.Name);
            Program.AddElement(nodeThreshold, "Text", asset3.Description);
            Program.AddElement(nodeThreshold, "Icon", asset3.IconFilename);
            var nodeRewards = Program.AddElement(nodeThreshold, "Rewards");
            foreach (var asset4 in asset3.Items) {
              var nodeReward = Program.AddElement(nodeRewards, "Reward");
              Program.AddElement(nodeReward, "GUID", asset4.GUID);
              Program.AddElement(nodeReward, "Name", asset4.Name);
              Program.AddElement(nodeReward, "Text", asset4.Description);
              Program.AddElement(nodeReward, "Icon", asset4.IconFilename);
            }
          }
        }
      }
      document.Save(Program.PathRoot + @"\Rewards\Monument.xml");
    }
    private static void MonumentEventCategory(XElement element) {
      if (element.Name == "Asset" && element.Element("Template")?.Value == "MonumentEventCategory") {
        var monumentCategory = Program.CreateAsset(element, element.Element("Values").Element("MonumentEventCategory").Element("CategoryDescriptionFluff").Value);
        foreach (var item1 in element.Element("Values").Element("MonumentEventCategory").Element("Events").Elements()) {
          var monumentEventNode = Program.FindElement(Program.Original.Root, item1.Element("Event").Value);
          var monumentEvent = Program.CreateAsset(monumentEventNode, monumentEventNode.Element("Values").Element("MonumentEvent").Element("RewardText").Value);
          monumentCategory.Items.Add(monumentEvent);
          foreach (var item2 in monumentEventNode.Element("Values").Element("MonumentEvent").Element("RewardThresholds").Elements()) {
            var monumentEventRewardNode = Program.FindElement(Program.Original.Root, item2.Element("Reward").Value);
            var monumentEventReward = Program.CreateAsset(monumentEventRewardNode, monumentEventRewardNode.Element("Values").Element("Standard").Element("InfoDescription").Value);
            monumentEvent.Items.Add(monumentEventReward);
            foreach (var item3 in monumentEventRewardNode.Element("Values").Element("Reward").Element("RewardAssets").Elements()) {
              var rewardNode1 = Program.FindElement(Program.Original.Root, item3.Element("Reward").Value);
              if (rewardNode1.Element("Template").Value == "MonumentEventReward") {
                foreach (var item4 in rewardNode1.Element("Values").Element("Reward").Element("RewardAssets").Elements()) {
                  var rewardNode2 = Program.FindElement(Program.Original.Root, item4.Element("Reward").Value);
                  var reward = Program.CreateAsset(rewardNode2, Program.GetDescriptionID(rewardNode2));
                  monumentEventReward.Items.Add(reward);
                }
              } else if (rewardNode1.Element("Template").Value == "RewardPool") {
                foreach (var item4 in rewardNode1.Element("Values").Element("RewardPool").Element("ItemsPool").Elements()) {
                  var rewardNode2 = Program.FindElement(Program.Original.Root, item4.Element("ItemLink").Value);
                  if (rewardNode2.Element("Template").Value == "RewardPool") {
                    foreach (var item5 in rewardNode2.Element("Values").Element("RewardPool").Element("ItemsPool").Elements()) {
                      var rewardNode3 = Program.FindElement(Program.Original.Root, item5.Element("ItemLink").Value);
                      var reward = Program.CreateAsset(rewardNode3, Program.GetDescriptionID(rewardNode3));
                      monumentEventReward.Items.Add(reward);
                    }
                  } else {
                    var reward = Program.CreateAsset(rewardNode2, Program.GetDescriptionID(rewardNode2));
                    monumentEventReward.Items.Add(reward);
                  }
                }
              } else {
                var reward = Program.CreateAsset(rewardNode1, Program.GetDescriptionID(rewardNode1));
                monumentEventReward.Items.Add(reward);
              }
            }
          }
        }
        Program.MonumentEventCategories.Add(monumentCategory);
      }
      if (element.HasElements) {
        foreach (var next in element.Elements()) {
          Program.MonumentEventCategory(next);
        }
      }
    }
    private static XElement FindElement(XElement element, String guid) {
      if (element.Name == "GUID" && element.Value == guid && element.Parent.Name == "Standard") {
        return element.Parent.Parent.Parent;
      }
      foreach (var next in element.Elements()) {
        var result = Program.FindElement(next, guid);
        if (result != null) return result;
      }
      return null;
    }
    private static Asset CreateAsset(XElement element, String textID) {
      switch (element.Element("Template").Value) {
        case "GuildhouseItem":
          return Program.GuildhouseItems.Single(w => w.GUID == element.Element("Values").Element("Standard").Element("GUID").Value);
        default:
          var asset = new Asset {
            GUID = element.Element("Values").Element("Standard").Element("GUID").Value,
            Name = element.Element("Values").Element("Standard").Element("Name").Value,
            Description = textID == null ? String.Empty : Program.FindElement(Program.Original.Root, textID).Element("Values").Element("Text").Element("LocaText").Element("English").Element("Text").Value,
            IconFilename = element.Element("Values").Element("Standard").Element("IconFilename").Value,
          };
          return asset;
      }
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
    private static void GetGuildhouseItems(XElement element) {
      if (element.Name == "Asset" && element.Element("Template")?.Value == "GuildhouseItem") {
        if (element.Element("Values").Element("ItemAction").HasElements)
          return;
        var item = new GuildhouseItem {
          GUID = element.Element("Values").Element("Standard").Element("GUID").Value,
          Name = element.Element("Values").Element("Standard").Element("Name").Value,
          IconFilename = element.Element("Values").Element("Standard").Element("IconFilename").Value,
          Description = Program.GetDescription(element.Element("Values").Element("Standard").Element("InfoDescription")?.Value),
          Rarity = element.Element("Values").Element("Item").Element("Rarity")?.Value,
          //ProductivityUpgrade = element.Element("Values").Element("FactoryUpgrade") == null || !element.Element("Values").Element("FactoryUpgrade").HasElements ? null : new ProductivityUpgrade {
          //  Value = element.Element("Values").Element("FactoryUpgrade").Element("ProductivityUpgrade").Element("Value").Value,
          //  Percental = element.Element("Values").Element("FactoryUpgrade").Element("ProductivityUpgrade").Element("Percental").Value
          //}
        };
        foreach (var node in element.Element("Values").Element("ItemEffect").Element("EffectTargets").Elements()) {
          var effectTarget = Program.FindElement(Program.Original.Root, node.Element("GUID").Value);
          item.EffectTargets.Add(effectTarget.Element("Values").Element("Standard").Element("Name").Value);
        }
        if (Program.GuildhouseItems.Count(c => c.GUID == item.GUID) == 0) {
          Program.GuildhouseItems.Add(item);
        }
      } else {
        foreach (var next in element.Elements()) {
          Program.GetGuildhouseItems(next);
        }
      }
    }
    private static void WriteHtml() {
      var document = XDocument.Load(Program.PathRoot + @"\Rewards\Monument.xml");
      var countCategory = 0;
      foreach (var nodeCategory in document.Root.Elements("Category")) {
        countCategory++;
        var countEvent = 0;
        foreach (var nodeEvent in nodeCategory.Element("Events").Elements()) {
          countEvent++;
          var sb = new StringBuilder();
          sb.AppendLine("<html>");
          sb.AppendLine("<body>");
          sb.AppendLine($"<div style=\"font-weight: bold;\">{nodeEvent.Element("Name").Value}</div>");
          sb.AppendLine($"<div>{nodeEvent.Element("Text").Value}</div>");
          sb.AppendLine("<br/>");
          foreach (var nodeThreshold in nodeEvent.Element("Thresholds").Elements()) {
            sb.AppendLine($"<div style=\"font-weight: bold;\">{nodeThreshold.Element("Name").Value}</div>");
            sb.AppendLine("<table>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th style=\"width: 50px;\"></th>");
            sb.AppendLine("<th style=\"width: auto;\"></th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");
            foreach (var nodeReward in nodeThreshold.Element("Rewards").Elements()) {
              sb.AppendLine("<tr>");
              sb.AppendLine("<td>&nbsp;</td>");
              sb.AppendLine($"<td><b>{nodeReward.Element("Name").Value}</b><br /><i>{nodeReward.Element("Text").Value}</i></td>");
              sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
          }
          sb.AppendLine("</body>");
          sb.AppendLine("</html>");
          File.WriteAllText(Program.PathRoot + $@"\Rewards\Category{countCategory}_Event{countEvent}.html", sb.ToString());
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
          if (target.Element("BaseAssetGUID") != null) {
            target = Program.FindElement(Program.Original.Root, target.Element("BaseAssetGUID").Value);
          }
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