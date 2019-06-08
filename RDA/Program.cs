using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;
using RDA.Templates;

namespace RDA {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  internal static class Program {

    #region Fields
    private static readonly Dictionary<Int32, String> Descriptions = new Dictionary<Int32, String>();
    internal static XDocument Original;
    internal static String PathRoot;
    internal static String PathViewer;
    private static readonly List<XElement> RewardPoolList = new List<XElement>();
    internal static XDocument TextDE;
    internal static Dictionary<String, String> DescriptionEN;
    internal static Dictionary<String, String> DescriptionDE;
    internal static string Version = "Release";
    internal static Dictionary<string, XElement> TourismStati = new Dictionary<string, XElement>();
    #endregion

    #region Private Methods
    private static void Main(String[] args) {
      Program.PathViewer = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.Parent.FullName + @"\AssetViewer";
      Program.PathRoot = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.FullName;
      Program.Original = XDocument.Load(Program.PathRoot + @"\Original\assets.xml");
      SetTourismStati();
      // Helper
      //Helper.ExtractTextEnglish(Program.PathRoot + @"\Original\texts_english.xml");
      //Helper.ExtractTextGerman(Program.PathRoot + @"\Original\texts_german.xml");
      //Helper.ExtractTemplateNames(Program.PathRoot + @"\Original\assets.xml");

      // Descriptions
      Program.DescriptionEN = XDocument.Load(Program.PathRoot + @"\Modified\Texts_English.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
      Program.DescriptionDE = XDocument.Load(Program.PathRoot + @"\Modified\Texts_German.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
      Version = "Update 03";
      // World Fair
      //Monument.Create();

      // Assets
      //Program.ProcessingItems("ActiveItem");
      //Program.ProcessingItems("ItemSpecialActionVisualEffect");
      //Program.ProcessingItems("ItemSpecialAction");
      //Program.ProcessingItems("GuildhouseItem");
      //Program.ProcessingItems("TownhallItem");
      //Program.ProcessingItems("HarborOfficeItem");
      //Program.ProcessingItems("VehicleItem");
      //Program.ProcessingItems("ShipSpecialist");
      //Program.ProcessingItems("CultureItem");
      //Program.ProcessingItems("BuildPermitBuilding");
      //Program.ProcessingItems("Product");

      //Program.ProcessingRewardPools();

      //Third Party
      //Program.ProcessingThirdParty();

      // Quests
      //Program.QuestGiver();
      //Program.Quests();

      // Expeditions
      //Program.Expeditions();
      //Program.ProcessingExpeditionEvents();

      ////Tourism
      Program.ProcessingTourism();
    }

    private static void SetTourismStati() {
      var TourismAsset = Program.Original.Root.Descendants("Asset").FirstOrDefault(l => l.Element("Template")?.Value == "TourismFeature");
      var CityStatis = TourismAsset.XPathSelectElement("Values/TourismFeature/CityStati").Elements().ToList();
      TourismStati = new Dictionary<string, XElement>();
      for (var i = 1; i < CityStatis.Count; i++) {
        TourismStati[i.ToString()] = CityStatis[i];
      }
    }

    public static void ProcessingItems(String template, bool findSources = true) {
      var result = new List<Asset>();
      var oldAssets = new Dictionary<string, XElement>();
      if (File.Exists($@"{Program.PathRoot}\Modified\Assets_{template}.xml")) {
        var doc = XDocument.Load($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
        oldAssets = doc.Root.Elements().ToDictionary(e => e.Attribute("ID").Value);
      }
      var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}']").ToList();
      Console.WriteLine(template + "  Total: " + assets.Count);
      var count = 0;
     assets.AsParallel().ForAll(asset => {
        Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value + " - " + count++);
        var item = new Asset(asset, findSources);
        if (oldAssets.ContainsKey(item.ID)) {
          item.ReleaseVersion = oldAssets[item.ID].Attribute("Release")?.Value ?? "Release";
        }
        else {
          item.ReleaseVersion = Version;
        }
        result.Add(item);
      });
      var document = new XDocument();
      document.Add(new XElement(template));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
      document.Save($@"{Program.PathViewer}\Resources\Assets\{template}.xml");
    }
    private static void ProcessingThirdParty() {
      var result = new List<ThirdParty>();
      var assets = Program
         .Original
         .XPathSelectElements($"//Asset[Template='Profile_3rdParty' or Template='Profile_3rdParty_Pirate']")
         .Concat(new[] { Program.Original.Descendants("Asset").FirstOrDefault(a => a.XPathSelectElement("Values/Standard/GUID")?.Value == "220") })
         .ToList()
         .AsParallel();

      assets.ForAll((asset) => {
        if (!asset.XPathSelectElements("Values/Trader/Progression/*/OfferingItems").Any() && !asset.XPathSelectElements("Values/ConstructionAI/ItemTradeConfig/ItemPools/*/Pool").Any())
          return;
        Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value);
        var item = new ThirdParty(asset);
        result.Add(item);
      });
      var document = new XDocument();
      document.Add(new XElement("ThirdParties"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_ThirdParty.xml");
      document.Save($@"{Program.PathViewer}\Resources\Assets\ThirdParty.xml");
    }
    private static void QuestGiver() {
      var result = new List<QuestGiver>();
      var questGivers = Program.Original.Root.XPathSelectElements("//Asset[Template='Quest']/Values/Quest/QuestGiver").Select(s => s.Value).Distinct().ToList();
      questGivers.ForEach((id) => {
        Console.WriteLine(id);
        var questGiver = Program.Original.Root.XPathSelectElement($"//Asset[Values/Standard/GUID={id}]");
        var item = new QuestGiver(questGiver);
        result.Add(item);
      });
      var document = new XDocument();
      document.Add(new XElement("QuestGivers"));
      document.Root.Add(result.Select(s => s.ToXml()));
    }
    private static void Quests() {
      var result = new List<Quest>();
      var assets = Program.Original.XPathSelectElements("//Asset[Template='Quest']").ToList();
      assets.ForEach((asset) => {
        Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value);
        var item = new Quest(asset);
        result.Add(item);
      });
      var document = new XDocument();
      document.Add(new XElement("Quests"));
      document.Root.Add(result.Select(s => s.ToXml()));
    }
    private static void Expeditions() {
      var result = new List<Expedition>();
      var assets = Program.Original.XPathSelectElements("//Asset[Template='Expedition']").ToList().AsParallel();
      assets.ForAll((asset) => {
        if (asset.XPathSelectElement("Values/Standard/Name").Value.Contains("Test"))
          return;
        Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value);
        var item = new Expedition(asset);
        if (item.Rewards.SelectMany(s => s.RewardPool).SelectMany(s => s.Items).Any()) {
          result.Add(item);
        }
      });
      var document = new XDocument();
      document.Add(new XElement("Expeditions"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_Expeditions.xml");
      document.Save($@"{Program.PathViewer}\Resources\Assets\Expeditions.xml");
    }
    private static void ProcessingRewardPools() {
      var RewardItemPoolsAssets = Program.Original
         .XPathSelectElements($"//Asset[Template='RewardPool']")
         .Concat(Program.Original.XPathSelectElements($"//Asset[Template='RewardItemPool']"))
         .ToList();
      var xRewardPools = new XElement("RewardPools");
      foreach (var RewardPool in RewardItemPoolsAssets) {
        var xPool = new XElement("Pool");
        xRewardPools.Add(xPool);
        xPool.Add(new XAttribute("ID", RewardPool.XPathSelectElement("Values/Standard/GUID").Value));
        if (RewardPool.XPathSelectElement("Values/RewardPool/ItemsPool")?.HasElements ?? false) {
          var xItems = new XElement("Items");
          xPool.Add(xItems);
          foreach (var item in RewardPool.XPathSelectElements("Values/RewardPool/ItemsPool/Item")) {
            var itemId = item.XPathSelectElement("ItemLink")?.Value ?? item.XPathSelectElement("ItemGroup")?.Value;
            if (itemId != null) {
              var xItem = new XElement("Item");
              xItems.Add(xItem);
              xItem.Add(new XAttribute("ID", itemId));
              xItem.Add(new XAttribute("Weight", item.XPathSelectElement("Weight")?.Value ?? "100"));
            }
          }
        }
      }
      var document = new XDocument(xRewardPools);
      document.Save($@"{Program.PathRoot}\Modified\Assets_RewardPools.xml");
      document.Save($@"{Program.PathViewer}\Resources\Assets\RewardPools.xml");
    }
    private static void ProcessingExpeditionEvents() {
      var decicions = Program.Original
          .XPathSelectElements("//Asset[Template='ExpeditionDecision']")
          .Where(f => f.XPathSelectElement("Values/Reward")?.HasElements ?? false)
          .ToList();
      var template = "ExpeditionEvents";
      //var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}' and Values/Standard/GUID=191507]").ToList();
      Console.WriteLine("Total: " + decicions.Count);
      var count = 0;
      var ResultEvents = new Dictionary<XElement, List<HashSet<XElement>>>();
      foreach (var decicion in decicions) {
        count++;
        Console.WriteLine(decicion.XPathSelectElement("Values/Standard/GUID").Value + " - " + count);
        var events = VerasFindExpeditionEvents(decicion.XPathSelectElement("Values/Standard/GUID").Value, new Details { decicion });
        foreach (var item in events) {
          if (ResultEvents.ContainsKey(item.Source)) {
            ResultEvents[item.Source].Add(item.Details);
          }
          else {
            ResultEvents.Add(item.Source, new List<HashSet<XElement>> { item.Details });
          }
        }
      }
      var document = new XDocument();
      document.Add(new XElement(template));
      document.Root.Add(ResultEvents.Select(s => ToXml(s)));
      document.Save($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
      document.Save($@"{Program.PathViewer}\Resources\Assets\{template}.xml");

      //local method ExpeditionEvents ToXml
      XElement ToXml(KeyValuePair<XElement, List<HashSet<XElement>>> events) {
        var xRoot = new XElement("ExpeditionEvent");
        xRoot.Add(new XAttribute("ID", events.Key.XPathSelectElement("Values/Standard/GUID").Value));
        xRoot.Add(new Description(events.Key.XPathSelectElement("Values/Standard/GUID").Value).ToXml("Name"));
        var xPaths = new XElement("Paths");
        xRoot.Add(xPaths);
        foreach (var path in events.Value) {
          var xPath = new XElement("Path");
          xPaths.Add(xPath);
          xPath.Add(new XAttribute("ID", path.First().XPathSelectElement("Values/Standard/Name").Value.Split(' ').Last()));

          var xRewards = new XElement("Rewards");
          xPath.Add(xRewards);
          var rewards = path.First().XPathSelectElements("Values/Reward/RewardAssets/Item");
          foreach (var reward in rewards) {
            var xReward = new XElement("Item");
            xRewards.Add(xReward);
            xReward.Add(new XAttribute("ID", reward.XPathSelectElement("Reward").Value));
            if (reward.XPathSelectElement("Amount") != null) {
              xReward.Add(new XAttribute("Amount", reward.XPathSelectElement("Amount").Value));
            }
          }

          var xOptions = new XElement("Options");
          xPath.Add(xOptions);
          foreach (var option in path) {
            if (option.XPathSelectElement("Template").Value == "ExpeditionOption") {
              var xOption = new XElement("Option");
              xOptions.AddFirst(xOption);
              xOption.Add(new XAttribute("ID", option.XPathSelectElement("Values/Standard/GUID").Value));
              xOption.Add(new Description(option.XPathSelectElement("Values/Standard/GUID").Value).ToXml("Text"));
              if (option.XPathSelectElement("Values/ExpeditionOption/OptionAttribute") != null) {
                xOption.Add(new XAttribute("OptionAttribute", option.XPathSelectElement("Values/ExpeditionOption/OptionAttribute").Value));
              }
              if (option.XPathSelectElement("Values/ExpeditionOption/Requirements")?.HasElements == true) {
                var xRequirements = new XElement("Requirements");
                xOption.Add(xRequirements);
                foreach (var requirement in option.XPathSelectElements("Values/ExpeditionOption/Requirements/Item")) {
                  var xItem = new XElement("Item");
                  xRequirements.Add(xItem);
                  if (requirement.XPathSelectElement("NeededAttribute") != null) {
                    xItem.Add(new XAttribute("NeededAttribute", requirement.XPathSelectElement("NeededAttribute").Value));
                  }
                  if (requirement.XPathSelectElement("ItemOrProduct") != null) {
                    xItem.Add(new XAttribute("ID", requirement.XPathSelectElement("ItemOrProduct").Value));
                  }
                  if (requirement.XPathSelectElement("Amount") != null) {
                    xItem.Add(new XAttribute("Amount", requirement.XPathSelectElement("Amount").Value));
                  }
                  if (requirement.XPathSelectElement("ItemGroup") != null) {
                    xItem.Add(new XAttribute("ID", requirement.XPathSelectElement("ItemGroup").Value));
                  }
                }
              }
            }
          }
        }
        return xRoot;
      }

      //local method Find Expedition Events
      SourceWithDetailsList VerasFindExpeditionEvents(string id, Details mainDetails = default, SourceWithDetailsList inResult = default) {
        mainDetails = (mainDetails == default) ? new Details() : mainDetails;
        mainDetails.PreviousIDs.Add(id);
        var mainResult = inResult ?? new SourceWithDetailsList();
        var resultstoadd = new List<SourceWithDetailsList>();
        var links = Program.Original.Root.XPathSelectElements($"//*[text()={id} and not(self::GUID)]").ToArray();
        if (links.Length > 0) {
          for (var i = 0; i < links.Length; i++) {
            var element = links[i];
            while (element.Name.LocalName != "Asset" || !element.HasElements) {
              element = element.Parent;
            }
            var Details = new Details(mainDetails);
            var result = mainResult.Copy();
            var key = element.XPathSelectElement("Values/Standard/GUID").Value;
            if (element.Element("Template") == null || mainDetails.PreviousIDs.Contains(key)) {
              continue;
            }
            switch (element.Element("Template").Value) {
              case "ExpeditionDecision":
              case "ExpeditionOption":
              case "ExpeditionTrade":
                Details.Add(element);
                VerasFindExpeditionEvents(key, Details, result);
                break;
              case "ExpeditionEvent":
                if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                  result.AddSourceAsset(element, Details.Items);
                }
                break;

              default:
                //ignore
                break;
            }
            resultstoadd.Add(result);
          }
        }

        foreach (var item in resultstoadd) {
          mainResult.AddSourceAsset(item);
        }
        return mainResult;
      }
    }
    private static void ProcessingTourism() {
      var TourismAsset = Program.Original.Root.Descendants("Asset").FirstOrDefault(l => l.Element("Template")?.Value == "TourismFeature");
      var xRoot = new XElement("CityStati");
      foreach (var pool in TourismAsset.Descendants("SpecialistPools").FirstOrDefault().Elements()) {
        var id = pool.Element("CityStatus").Value;
        var xStatus = new XElement("Status");
        xRoot.Add(xStatus);
        var desc = new Description(TourismStati[id].Element("AttractivenessThreshold").Value + " Attractiveness", TourismStati[id].Element("AttractivenessThreshold").Value + " Attraktivität");
        xStatus.Add(new XAttribute("Pool", pool.Element("Pool").Value));
        xStatus.Add(desc.ToXml("Requirement"));
        xStatus.Add(new Description(TourismStati[id].Element("CityStatusFluff").Value).ToXml("Text"));
      }

      foreach (var pool in TourismAsset.Descendants("UnlockablePools").SelectMany(p => p.Elements())) {
        var xStatus = new XElement("Status");
        xRoot.Add(xStatus);
        xStatus.Add(new XElement(new Description(pool.Element("UnlockingSpecialist").Value).ToXml("Requirement")));
        xStatus.Add(new XAttribute("Pool", pool.Element("Pool").Value));
      }

      foreach (var pool in TourismAsset.Descendants("SpecialistPoolsThroughSets").SelectMany(p => p.Elements())) {
        var xStatus = new XElement("Status");
        xRoot.Add(xStatus);
        xStatus.Add(new XElement(new Description(pool.Element("UnlockingSetBuff").Value).ToXml("Requirement")));
        xStatus.Add(new XAttribute("Pool", pool.Element("Pool").Value));
      }
      var document = new XDocument(xRoot);
      document.Save($@"{Program.PathRoot}\Modified\Assets_Tourism.xml");
      document.Save($@"{Program.PathViewer}\Resources\Assets\Tourism.xml");
    }
    #endregion

  }

}