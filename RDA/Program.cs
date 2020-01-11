using RDA.Data;
using RDA.Library;
using RDA.Templates;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA {
  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  internal static class Program {
    #region Methods
    static Program() {
      Program.PathViewer = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.Parent.FullName + @"\AssetViewer";
      Program.PathRoot = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.FullName;

    }
    public static void Main(String[] _) {
      // Helper  Obsolete
      //Helper.ExtractTextEnglish(Program.PathRoot + @"\Original\texts_english.xml");
      //Helper.ExtractTextGerman(Program.PathRoot + @"\Original\texts_german.xml");
      //Helper.ExtractText();
      //Helper.ExtractTemplateNames(Program.PathRoot + @"\Original\assets.xml");

      Assets.Init("Update 06");

      // World Fair
      Monument.Create();

      //Assets
      Program.ProcessingItems("ActiveItem");
      Program.ProcessingItems("ItemSpecialActionVisualEffect");
      Program.ProcessingItems("ItemSpecialAction");
      Program.ProcessingItems("GuildhouseItem");
      Program.ProcessingItems("TownhallItem");
      Program.ProcessingItems("HarborOfficeItem");
      Program.ProcessingItems("VehicleItem");
      Program.ProcessingItems("ShipSpecialist");
      Program.ProcessingItems("CultureItem");
      Program.ProcessingItems("ItemWithUI");
      Program.ProcessingItems("FluffItem");
      Program.ProcessingItems("QuestItemMagistrate");
      Program.ProcessingItems("StartExpeditionItem");
      Program.ProcessingItems("QuestItem");
      Program.ProcessingItems("TestConstructionPlan");

      Program.ProcessingItems("Product");

      //Buildings
      Program.ProcessingBuildings();

      ////ItemsSets
      Program.ProcessingItemSets();

      //Festivals
      Program.ProcessingFestivalBuffs();

      //RewardPools
      Program.ProcessingRewardPools();

      //Third Party
      Program.ProcessingThirdParty();

      //// Quests
      ////Program.QuestGiver();   //Obsolete
      ////Program.Quests();       //Obsolete

      //// Expeditions
      ////Program.Expeditions(); //Obsolete
      Program.ProcessingExpeditionEvents();

      ////Tourism
      Program.ProcessingTourism();

      //Save Descriptions
      //Set True for fully new Set of Descriptions.
      Program.SaveDescriptions(true);
    }
    public static readonly object ConsoleLock = new object();
    public static void ConsoleWriteGUID(string str) {
      lock (ConsoleLock) {
        if (Console.CursorLeft != 0) {
          var currentleft = Console.CursorLeft;
          Console.SetCursorPosition(0, Console.CursorTop);
          Console.Write(str);
          Console.Write(new string(' ', Math.Abs(currentleft - Console.CursorLeft)));
        }
        else {
          Console.Write(str);
        }
      }

    }

    public static void ConsoleWriteHeadline(string str) {
      if (Console.CursorLeft != 0) {
        Console.WriteLine("");
      }
      Console.WriteLine(str);
    }

    #endregion Methods

    #region Fields

    internal readonly static String PathRoot;
    internal readonly static String PathViewer;
    internal static XDocument TextDE;

    #endregion Fields

    private static void ProcessingBuildings() {
      foreach (var item in Assets.Original.Descendants("Asset").Where(a => a.XPathSelectElement("Values/Building") != null).Select(a => a.Element("Template").Value).Distinct()) {
        ProcessingItems(item, false);
      }
    }

    private static void SaveDescriptions(bool resetOld = false) {
      ConsoleWriteHeadline("Save Descriptions");
      // Split Languages To single Files
      foreach (Languages language in Enum.GetValues(typeof(Languages))) {
        using (var xmlWriter = XmlWriter.Create($@"{Program.PathRoot}\Modified\Texts_{language.ToString("G")}.xml", new XmlWriterSettings() { Indent = false })) {
          xmlWriter.WriteStartElement("Texts");
          var savedIDs = new HashSet<string>();

          //Custom Descriptions
          foreach (var item in Assets.CustomDescriptions) {
            if (!savedIDs.Contains(item.Key)) {
              xmlWriter.WriteStartElement("Text");
              xmlWriter.WriteAttributeString("ID", item.Key);
              xmlWriter.WriteValue(item.Value.TryGetValue(language, out var value) ? value : item.Value.First().Value);
              xmlWriter.WriteEndElement();
              savedIDs.Add(item.Key);
            }
          }
          //Global Descriptions
          foreach (var item in Description.GlobalDescriptions.Values) {
            if (!savedIDs.Contains(item.ID)) {
              xmlWriter.WriteStartElement("Text");
              xmlWriter.WriteAttributeString("ID", item.ID);
              xmlWriter.WriteValue(item.Languages.TryGetValue(language, out var value) ? value : item.Languages.First().Value);
              xmlWriter.WriteEndElement();
              savedIDs.Add(item.ID);
            }
          }
          //Descriptions needed by the Viewer
          var needed = new[] {
            new Description("113817"),   //Kosten für Umwandlung
            new Description("12723"),    //Baukosten  100008
            new Description("100006"),    //Produktion
            new Description("100007"),    //Verbrauch
            new Description("100008"),    //Baukosten
            new Description("3109"),    //Reparieren
            new Description("2001775"),    //Ausbauen
            new Description("100409"),    //Unterhaltskosten
            new Description("12725"),   //Verkaufspreis
            new Description("21731"),    //Anheuerungskosten
            new Description("20106")    //Stadtfest
          };

          foreach (var item in needed) {
            if (!savedIDs.Contains(item.ID)) {
              xmlWriter.WriteStartElement("Text");
              xmlWriter.WriteAttributeString("ID", item.ID);
              xmlWriter.WriteValue(item.Languages.TryGetValue(language, out var value) ? value : item.Languages.First().Value);
              xmlWriter.WriteEndElement();
              savedIDs.Add(item.ID);
            }
          }

          //Load Last Descriptions to make single file updates available
          if (!resetOld && File.Exists($@"{Program.PathViewer}\Resources\Assets\Texts_{language.ToString("G")}.xml")) {
            var doc = XDocument.Load($@"{Program.PathViewer}\Resources\Assets\Texts_{language.ToString("G")}.xml").Root;
            foreach (var item in doc.Elements()) {
              if (!savedIDs.Contains(item.Attribute("ID").Value)) {
                xmlWriter.WriteStartElement("Text");
                xmlWriter.WriteAttributeString("ID", item.Attribute("ID").Value);
                xmlWriter.WriteValue(item.Value);
                xmlWriter.WriteEndElement();
              }
            }
          }
        }

        // Copy Languages To Viewer
        if (File.Exists($@"{Program.PathViewer}\Resources\Assets\Texts_{language.ToString("G")}.xml")) {
          File.Delete($@"{Program.PathViewer}\Resources\Assets\Texts_{language.ToString("G")}.xml");
        }
        File.Copy($@"{Program.PathRoot}\Modified\Texts_{language.ToString("G")}.xml", $@"{Program.PathViewer}\Resources\Assets\Texts_{language.ToString("G")}.xml");
      }
    }

    private static void ProcessingItems(String template, bool findSources = true, Action<Asset> manipulate = null) {
      var result = new List<Asset>();
      var oldAssets = new Dictionary<string, XElement>();
      if (File.Exists($@"{Program.PathRoot}\Modified\Assets_{template}.xml")) {
        var doc = XDocument.Load($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
        oldAssets = doc.Root.Elements().ToDictionary(e => e.Attribute("ID").Value);
      }
      var assets = Assets.Original.XPathSelectElements($"//Asset[Template='{template}']").ToList();
      ConsoleWriteHeadline(template + "  Total: " + assets.Count);
      var count = 1;
      assets.AsParallel().ForAll(asset => {
        ConsoleWriteGUID(asset.XPathSelectElement("Values/Standard/GUID").Value + " - " + count++);
        var item = new Asset(asset, findSources);
        if (oldAssets.ContainsKey(item.ID)) {
          item.ReleaseVersion = oldAssets[item.ID].Attribute("Release")?.Value ?? "Release";
        }
        else {
          item.ReleaseVersion = Assets.Version;
        }
        manipulate?.Invoke(item);
        result.Add(item);
      });
      var document = new XDocument();
      document.Add(new XElement(template));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\{template}.xml");
    }
    public static void SaveIndent(this XDocument doc, string path) {
      doc.Save(path, SaveOptions.DisableFormatting);

    }
    private static void ProcessingItemSets() {
      ProcessingItems("ItemSet", false, asset => {
        asset.SetParts = Assets
              .Original
              .Descendants("Asset")
              .Where(a => a.XPathSelectElement("Values/Item/ItemSet")?.Value == asset.ID)
              .Select(a => a.XPathSelectElement("Values/Standard/GUID").Value);
      });
    }

    private static void ProcessingFestivalBuffs() {
      ProcessingItems("FestivalBuff", false, asset => {
        asset.FestivalName = Assets
              .Original
              .Descendants("Effect")
              .Where(a => a?.Value == asset.ID)
              .Select(a => new Description(a.Parent.Parent.Parent.Element("FestivalName").Value))
              .FirstOrDefault();
      });
    }

    private static void ProcessingThirdParty() {
      ConsoleWriteHeadline("Processing Third Party");
      var result = new List<ThirdParty>();

      var assets = Assets
         .Original
         .XPathSelectElements($"//Asset[Template='Profile_3rdParty' or Template='Profile_3rdParty_Pirate']")
         .Concat(new[] { Assets.Original.Descendants("Asset").FirstOrDefault(a => a.XPathSelectElement("Values/Standard/GUID")?.Value == "220") })
         .Concat(Assets.Original.XPathSelectElements($"//Asset[Template='Profile_3rdParty_ItemCrafter']"))
         .AsParallel();

      assets.ForAll((asset) => {
        if (!asset.XPathSelectElements("Values/Trader/Progression/*/OfferingItems").Any() && !asset.XPathSelectElements("Values/ConstructionAI/ItemTradeConfig/ItemPools/*/Pool").Any())
          return;
        ConsoleWriteGUID(asset.XPathSelectElement("Values/Standard/GUID").Value);
        var item = new ThirdParty(asset);
        //Exclude Isabel Campain
        if (item.ID != "199" && item.ID != "200" && item.ID != "240" && item.ID != "117422") {
          result.Add(item);
        }
      });
      var document = new XDocument();
      document.Add(new XElement("ThirdParties"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_ThirdParty.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\ThirdParty.xml");
    }

    private static void QuestGiver() {
      var result = new List<QuestGiver>();
      var questGivers = Assets.Original.XPathSelectElements("//Asset[Template='Quest']/Values/Quest/QuestGiver").Select(s => s.Value).Distinct().ToList();
      questGivers.ForEach((id) => {
        ConsoleWriteGUID(id);
        var questGiver = Assets.Original.XPathSelectElement($"//Asset[Values/Standard/GUID={id}]");
        var item = new QuestGiver(questGiver);
        result.Add(item);
      });
      var document = new XDocument();
      document.Add(new XElement("QuestGivers"));
      document.Root.Add(result.Select(s => s.ToXml()));
    }

    private static void Quests() {
      var result = new List<Quest>();
      var assets = Assets.Original.XPathSelectElements("//Asset[Template='Quest']").ToList();
      assets.ForEach((asset) => {
        ConsoleWriteGUID(asset.XPathSelectElement("Values/Standard/GUID").Value);
        var item = new Quest(asset);
        result.Add(item);
      });
      var document = new XDocument();
      document.Add(new XElement("Quests"));
      document.Root.Add(result.Select(s => s.ToXml()));
    }

    private static void Expeditions() {
      var result = new List<Expedition>();
      var assets = Assets.Original.XPathSelectElements("//Asset[Template='Expedition']").AsParallel();
      assets.ForAll((asset) => {
        if (asset.XPathSelectElement("Values/Standard/Name").Value.Contains("Test"))
          return;
        ConsoleWriteGUID(asset.XPathSelectElement("Values/Standard/GUID").Value);
        var item = new Expedition(asset);
        if (item.Rewards.SelectMany(s => s.RewardPool).SelectMany(s => s.Items).Any()) {
          result.Add(item);
        }
      });
      var document = new XDocument();
      document.Add(new XElement("Expeditions"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_Expeditions.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\Expeditions.xml");
    }

    private static void ProcessingRewardPools() {
      ConsoleWriteHeadline("Processing Reward Pools");
      var xRewardPools = new XElement("RewardPools");

      //RewardPool and RewardItemPool
      var RewardItemPoolsAssets = Assets.Original
         .XPathSelectElements($"//Asset[Template='RewardPool']")
         .Concat(Assets.Original.XPathSelectElements($"//Asset[Template='RewardItemPool']"))
         .ToList();
      foreach (var RewardPool in RewardItemPoolsAssets) {
        var xPool = new XElement("Pool");
        xRewardPools.Add(xPool);
        xPool.Add(new XAttribute("ID", RewardPool.XPathSelectElement("Values/Standard/GUID").Value));
        xPool.Add(new XAttribute("Name", RewardPool.XPathSelectElement("Values/Standard/Name").Value));
        if (RewardPool.XPathSelectElement("Values/RewardPool/ItemsPool")?.HasElements ?? false) {
          var xItems = new XElement("Items");
          xPool.Add(xItems);
          foreach (var item in RewardPool.XPathSelectElements("Values/RewardPool/ItemsPool/Item")) {
            var itemId = item.XPathSelectElement("ItemLink")?.Value ?? item.XPathSelectElement("ItemGroup")?.Value;
            if (itemId != null) {
              var xItem = new XElement("Item");
              xItems.Add(xItem);
              xItem.Add(new XAttribute("ID", itemId));
              xItem.Add(new XAttribute("Weight", item.XPathSelectElement("Weight")?.Value ?? "1"));
            }
          }
        }
      }

      //ResourcePool
      var ResourcePoolAssets = Assets.Original
         .XPathSelectElements($"//Asset[Template='ResourcePool']")
         .ToList();
      foreach (var ResourcePool in ResourcePoolAssets) {
        var xPool = new XElement("Pool");
        xRewardPools.Add(xPool);
        xPool.Add(new XAttribute("ID", ResourcePool.XPathSelectElement("Values/Standard/GUID").Value));
        xPool.Add(new XAttribute("Name", ResourcePool.XPathSelectElement("Values/Standard/Name").Value));
        if (ResourcePool.XPathSelectElement("Values/ResourceRewardPool/PossibleRewards")?.HasElements ?? false) {
          var xItems = new XElement("Items");
          xPool.Add(xItems);
          foreach (var item in ResourcePool.XPathSelectElements("Values/ResourceRewardPool/PossibleRewards/Item")) {
            var itemId = item.XPathSelectElement("Resource")?.Value;
            if (itemId != null) {
              var xItem = new XElement("Item");
              xItems.Add(xItem);
              xItem.Add(new XAttribute("ID", itemId));
              xItem.Add(new XAttribute("Weight", item.XPathSelectElement("Weight")?.Value ?? "100"));
            }
          }
        }
      }

      //AssetGroups
      var AssetGroups = Assets.Original
        .Descendants("Groups")
        .Where(g => g.Element("GUID") != null)
        .Concat(Assets.Original
         .Descendants("Group")
         .Where(g => g.Element("GUID") != null))
         .ToList();
      foreach (var AssetGroup in AssetGroups) {
        var childAssets = AssetGroup.Descendants("Asset").Where(a => a.XPathSelectElement("Values/Item") != null || a.XPathSelectElement("Values/Product") != null);
        if (childAssets.Any()) {
          var xPool = new XElement("Pool");
          xRewardPools.Add(xPool);
          xPool.Add(new XAttribute("ID", AssetGroup.Element("GUID").Value));
          var xItems = new XElement("Items");
          xPool.Add(xItems);
          foreach (var item in childAssets) {
            var itemId = item.XPathSelectElement("Values/Standard/GUID")?.Value;
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
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\RewardPools.xml");
    }

    private static void ProcessingExpeditionEvents() {
      const string template = "ExpeditionEvents";
      var ResultEvents = new ConcurrentDictionary<XElement, ConcurrentBag<HashSet<XElement>>>();
      var count = 1;

      var decicions = Assets.Original
          .XPathSelectElements("//Asset[Template='ExpeditionDecision']")
          .Where(f => f.XPathSelectElement("Values/Reward/RewardAssets")?.Elements("Item").Any(r => r.Element("Reward")?.Value != null) ?? false)
          .ToList();

      decicions.AddRange(Assets.Original.XPathSelectElements("//Asset[Template='ExpeditionTrade']"));
      ConsoleWriteHeadline(template + "  Total: " + decicions.Count);

      decicions.AsParallel().ForAll(decicion => {
        ConsoleWriteGUID(decicion.XPathSelectElement("Values/Standard/GUID").Value + " - " + count++);
        var events = VerasFindExpeditionEvents(decicion.XPathSelectElement("Values/Standard/GUID").Value, new Details { decicion });
        foreach (var item in events) {
          if (ResultEvents.ContainsKey(item.Source)) {
            ResultEvents[item.Source].Add(item.Details);
          }
          else {
            ResultEvents.TryAdd(item.Source, new ConcurrentBag<HashSet<XElement>> { item.Details });
          }
        }
      });
      var document = new XDocument();
      document.Add(new XElement(template));
      document.Root.Add(ResultEvents.Select(s => ToXml(s)));
      document.SaveIndent($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\{template}.xml");

      //local method ExpeditionEvents ToXml
      XElement ToXml(KeyValuePair<XElement, ConcurrentBag<HashSet<XElement>>> events) {
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
          if (rewards.Any()) {
            foreach (var reward in rewards) {
              var xReward = new XElement("Item");
              xRewards.Add(xReward);
              xReward.Add(new XAttribute("ID", reward.XPathSelectElement("Reward").Value));
              if (reward.XPathSelectElement("Amount") != null) {
                xReward.Add(new XAttribute("Amount", reward.XPathSelectElement("Amount").Value));
              }
            }
          }
          else {
            var Products = path.First().XPathSelectElements("Values/ExpeditionTrade/AvailableGoods/Item");
            if (Products.Any()) {
              foreach (var Product in Products) {
                var xReward = new XElement("Item");
                xRewards.Add(xReward);
                xReward.Add(new XAttribute("ID", Product.Element("Product").Value));
                if (Product.Element("Amount")?.Value is string value) {
                  xReward.Add(new XAttribute("Amount", value));
                }
              }
            }
            var Items = path.First().XPathSelectElements("Values/ExpeditionTrade/AvailableItems/Item");
            if (Items.Any()) {
              foreach (var Item in Items) {
                var xReward = new XElement("Item");
                xRewards.Add(xReward);
                xReward.Add(new XAttribute("ID", Item.Element("Item").Value));
                if (Item.Element("Amount")?.Value is string value) {
                  xReward.Add(new XAttribute("Amount", value));
                }
              }
            }
          }

          var xOptions = new XElement("Options");
          xPath.Add(xOptions);
          foreach (var option in path) {
            if (option.XPathSelectElement("Template").Value == "ExpeditionOption" ||
                option.XPathSelectElement("Template").Value == "ExpeditionMapOption") {
              var xOption = new XElement("Option");
              xOptions.AddFirst(xOption);
              xOption.Add(new XAttribute("ID", option.XPathSelectElement("Values/Standard/GUID").Value));
              var text = new Description(option.XPathSelectElement("Values/Standard/GUID").Value);
              if (text.Languages[Library.Languages.English] == "Confirm") {
                text = new Description("145001");
              }
              else if (text.Languages[Library.Languages.English] == "Cancel") {
                text = new Description("145002");
              }
              xOption.Add(text.ToXml("Text"));
              if (option.XPathSelectElement("Values/ExpeditionOption/OptionAttribute")?.Value != null) {
                xOption.Add(new Description(Assets.KeyToIdDict[option.XPathSelectElement("Values/ExpeditionOption/OptionAttribute").Value]).ToXml("OptionAttribute"));
              }
              if (option.XPathSelectElement("Values/ExpeditionOption/Requirements")?.HasElements == true) {
                var xRequirements = new XElement("Requirements");
                xOption.Add(xRequirements);
                foreach (var requirement in option.XPathSelectElements("Values/ExpeditionOption/Requirements/Item")) {
                  var xItem = new XElement("Item");
                  xRequirements.Add(xItem);
                  if (requirement.XPathSelectElement("NeededAttribute")?.Value != null) {
                    xItem.Add(new Description(Assets.KeyToIdDict[requirement.XPathSelectElement("NeededAttribute").Value]).ToXml("NeededAttribute"));
                  }
                  if (requirement.XPathSelectElement("ItemOrProduct")?.Value != null) {
                    xItem.Add(new XAttribute("ID", requirement.XPathSelectElement("ItemOrProduct").Value));
                  }
                  if (requirement.XPathSelectElement("Amount")?.Value != null) {
                    xItem.Add(new XAttribute("Amount", requirement.XPathSelectElement("Amount").Value));
                  }
                  if (requirement.XPathSelectElement("ItemGroup")?.Value != null) {
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
        var resultstoadd = new ConcurrentBag<SourceWithDetailsList>();
        var links = Assets.Original.XPathSelectElements($"//*[text()={id} and not(self::GUID)]").ToArray();
        if (links.Length > 0) {
          links.AsParallel().ForAll(link => {
            foreach (var link2 in new[] { link } /*links*/) {
              var element = link2;
              var foundedElement = element;
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
                case "GuildhouseItem":
                  //Ignore
                  break;

                case "Expedition":

                  break;

                case "ExpeditionDecision":
                case "ExpeditionTrade":
                  if (foundedElement.Name.LocalName == "Reward" ||
                    foundedElement.Name.LocalName == "Product" ||
                    foundedElement.Name.LocalName == "Item") {
                    goto case "SearchAgain";
                  }
                  else if (foundedElement.Name.LocalName != "Option" &&
                    foundedElement.Name.LocalName != "FollowupSuccessOption" &&
                    foundedElement.Name.LocalName != "FollowupFailOrCancelOption" &&
                    foundedElement.Name.LocalName != "InsertEvent") {
                    break;
                  }
                  goto case "SearchAgain";

                case "ExpeditionMapOption":
                case "ExpeditionOption":
                  if (foundedElement.Name.LocalName == "ItemOrProduct") {
                    break;
                  }
                  if (foundedElement.Name.LocalName != "Decision") {
                    break;
                  }
                  goto case "SearchAgain";
                case "ExpeditionBribe":
                  if (foundedElement.Name.LocalName == "Item") {
                    break;
                  }
                  if (foundedElement.Name.LocalName != "FollowupSuccessOption" &&
                    foundedElement.Name.LocalName != "FollowupFailOrCancelOption") {
                    break;
                  }
                  goto case "SearchAgain";
                case "SearchAgain":
                  Details.Add(element);
                  VerasFindExpeditionEvents(key, Details, result);
                  break;

                case "ExpeditionEvent":
                  if (foundedElement.Name.LocalName != "StartDecision") {
                    break;
                  }
                  result.AddSourceAsset(element, Details.Items);
                  break;

                default:
                  Debug.WriteLine(element.Element("Template").Value);
                  //ignore
                  break;
              }
              resultstoadd.Add(result);
            }
          });
        }

        foreach (var item in resultstoadd) {
          mainResult.AddSourceAsset(item);
        }
        return mainResult;
      }
    }

    private static void ProcessingTourism() {
      ConsoleWriteHeadline("Processing Tourism");
      var TourismAsset = Assets.Original.Descendants("Asset").FirstOrDefault(l => l.Element("Template")?.Value == "TourismFeature");
      var xRoot = new XElement("CityStati");
      foreach (var pool in TourismAsset.Descendants("SpecialistPools").FirstOrDefault()?.Elements()) {
        var id = pool.Element("CityStatus").Value;
        var xStatus = new XElement("Status");
        xRoot.Add(xStatus);
        var desc = new Description("145011").InsertBefore(Assets.TourismStati[id].Element("AttractivenessThreshold")?.Value ?? "0");
        xStatus.Add(new XAttribute("Pool", pool.Element("Pool").Value));
        xStatus.Add(desc.ToXml("Requirement"));
        xStatus.Add(new Description(Assets.TourismStati[id].Element("CityStatusFluff").Value).ToXml("Text"));
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
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\Tourism.xml");
    }
  }
}