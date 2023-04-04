﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;

namespace RDA {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
  internal static class Program {

    #region Public Constructors

    static Program() {
      Program.PathViewer = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty)).Parent.Parent.Parent.FullName + @"\AssetViewer";
      Program.PathRoot = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty)).Parent.Parent.FullName;
    }

    #endregion Public Constructors

    #region Public Methods

    public static void Main(string[] _) {
      // Helper  Obsolete
      //Helper.ExtractTextEnglish(Program.PathRoot + @"\Original\texts_english.xml");
      //Helper.ExtractTextGerman(Program.PathRoot + @"\Original\texts_german.xml");
      //Helper.ExtractText();
      //Helper.ExtractTemplateNames(Program.PathRoot + @"\Original\assets.xml");
      //Helper.ExtractItemTemplates(Program.PathRoot + @"\Original\assets.xml");

      Assets.Init("Update 17");

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
      Program.ProcessingItems("ItemConstructionPlan");
      Program.ProcessingItems("ItemWithUICrafting");

      Program.ProcessingItems("Product");

      //Program.ProcessingItems("Season", false);

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

      // Quests
      //Program.QuestGiver();   //Obsolete
      //Program.Quests();       //Obsolete

      // Expeditions
      //Program.Expeditions(); //Obsolete
      Program.ProcessingExpeditionEvents();

      //Tourism
      Program.ProcessingTourism();

      //Effekts
      Program.ProcessingBuffs();

      //Save Descriptions
      //Set True for fully new Set of Descriptions.
      Program.SaveDescriptions(true);
    }

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

    public static void SaveIndent(this XDocument doc, string path) {
      doc.Save(path, SaveOptions.DisableFormatting);
    }

    #endregion Public Methods

    #region Public Fields

    public static readonly object ConsoleLock = new object();

    #endregion Public Fields

    #region Internal Fields

    internal readonly static string PathRoot;
    internal readonly static string PathViewer;

    public static XDocument TextDE { get; }

    #endregion Internal Fields

    #region Private Methods

    private static void ProcessingBuildings() {
      foreach (var item in Assets.All.Descendants("Asset").Where(a => a.XPathSelectElement("Values/Building") != null).Select(a => a.Element("Template").Value).Distinct()) {
        ProcessingItems(item, false);
      }
    }

    private static void SaveDescriptions(bool resetOld = false) {
      ConsoleWriteHeadline("Save Descriptions");
      // Split Languages To single Files
      foreach (Languages language in Enum.GetValues(typeof(Languages))) {
        using (var xmlWriter = XmlWriter.Create($@"{Program.PathRoot}\Modified\Texts_{language:G}.xml", new XmlWriterSettings { Indent = false })) {
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
            new Description("113817", GameTypes.Anno_1800),   //Kosten für Umwandlung
            new Description("12723", GameTypes.Anno_1800),    //Baukosten  100008
            new Description("100006", GameTypes.Anno_1800),   //Produktion
            new Description("100007", GameTypes.Anno_1800),   //Verbrauch
            new Description("100008", GameTypes.Anno_1800),   //Baukosten
            new Description("3109", GameTypes.Anno_1800),     //Reparieren
            new Description("2001775", GameTypes.Anno_1800),  //Ausbauen
            new Description("100409", GameTypes.Anno_1800),   //Unterhaltskosten
            new Description("12725", GameTypes.Anno_1800),    //Verkaufspreis
            new Description("21731", GameTypes.Anno_1800),    //Anheuerungskosten
            new Description("20106", GameTypes.Anno_1800),     //Stadtfest
            new Description("22440", GameTypes.Anno_1800),     //Anzahl
            new Description("2363", GameTypes.Anno_1800),      //Effekte
            new Description("3963", GameTypes.Anno_1800).AppendInBraces("max.").Append(": ").ChangeID("-10000")   //Reroll Costs (max.)
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
          if (!resetOld && File.Exists($@"{Program.PathViewer}\Resources\Assets\Texts_{language:G}.xml")) {
            var doc = XDocument.Load($@"{Program.PathViewer}\Resources\Assets\Texts_{language:G}.xml").Root;
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
        if (File.Exists($@"{Program.PathViewer}\Resources\Assets\Texts_{language:G}.xml")) {
          File.Delete($@"{Program.PathViewer}\Resources\Assets\Texts_{language:G}.xml");
        }
        File.Copy($@"{Program.PathRoot}\Modified\Texts_{language:G}.xml", $@"{Program.PathViewer}\Resources\Assets\Texts_{language:G}.xml");
      }
    }

    private static void ProcessingBuffs() {
      var template = "Buffs";
      var oldAssets = new Dictionary<string, XElement>();

      if (File.Exists($@"{Program.PathRoot}\Modified\Assets_{template}.xml")) {
        var doc = XDocument.Load($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
        oldAssets = doc.Root.Elements().ToDictionary(e => e.Attribute("ID").Value);
      }

      foreach (var item in Assets.Buffs.Values) {
        if (oldAssets.ContainsKey(item.ID)) {
          item.ReleaseVersion = oldAssets[item.ID].Attribute("Release")?.Value ?? oldAssets[item.ID].Attribute("RV")?.Value ?? "Release";
        }
        else {
          item.ReleaseVersion = Assets.Version;
        }
      }

      var document = new XDocument();
      document.Add(new XElement(template));
      document.Root.Add(Assets.Buffs.Values.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\{template}.xml");
    }

    private static void ProcessingItems(string template, bool findSources = true, Action<Asset> manipulate = null) {
      var result = new ConcurrentBag<Asset>();
      var oldAssets = new Dictionary<string, XElement>();
      if (File.Exists($@"{Program.PathRoot}\Modified\Assets_{template}.xml")) {
        var doc = XDocument.Load($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
        oldAssets = doc.Root.Elements().ToDictionary(e => e.Attribute("ID").Value);
      }
      var assets = Assets.All.XPathSelectElements($"//Asset[Template='{template}']").ToList();
      ConsoleWriteHeadline(template + "  Total: " + assets.Count);
      var count = 1;
      assets.AsParallel().ForAll(asset => {
        var id = asset.XPathSelectElement("Values/Standard/GUID").Value;
        ConsoleWriteGUID(id + " - " + count++);
        var item = new Asset(asset, findSources);
        if (oldAssets.ContainsKey(item.ID)) {
          item.ReleaseVersion = oldAssets[item.ID].Attribute("Release")?.Value ?? oldAssets[item.ID].Attribute("RV")?.Value ?? "Release";
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

    private static void ProcessingItemSets() {
      ProcessingItems("ItemSet", false, asset => {
        asset.SetParts = Assets
              .All
              .Descendants("Asset")
              .Where(a => a.XPathSelectElement("Values/Item/ItemSet")?.Value == asset.ID)
              .Select(a => a.XPathSelectElement("Values/Standard/GUID").Value);
      });
    }

    private static void ProcessingFestivalBuffs() {
      ProcessingItems("FestivalBuff", false, asset => {
        asset.FestivalName = Assets
              .All
              .Descendants("Effect")
              .Where(a => a?.Value == asset.ID)
              .Select(a => new Description(a.Parent.Parent.Parent.Element("FestivalName").Value, GameTypes.Anno_1800))
              .FirstOrDefault();
      });
    }

    private static void ProcessingThirdParty() {
      ConsoleWriteHeadline("Processing Third Party");
      var result = new List<ThirdParty>();

      Assets.GUIDs.TryGetValue("220", out var assetHugo, GameTypes.Anno_1800);
      var assets = Assets
         .All
         .XPathSelectElements($"//Asset[Template='Profile_3rdParty' or Template='Profile_3rdParty_Pirate' or Template='Profile_3rdParty_ItemCrafter' or Template='Profile_3rdParty-PlayerCounter']")
         .Concat(new[] { assetHugo })
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

    //private static void QuestGiver() {
    //  var result = new List<QuestGiver>();
    //  var questGivers = Assets.All.XPathSelectElements("//Asset[Template='Quest']/Values/Quest/QuestGiver").Select(s => s.Value).Distinct().ToList();
    //  questGivers.ForEach((id) => {
    //    ConsoleWriteGUID(id);
    //    Assets.GUIDs.TryGetValue(id, out var questGiver);
    //    var item = new QuestGiver(questGiver);
    //    result.Add(item);
    //  });
    //  var document = new XDocument();
    //  document.Add(new XElement("QuestGivers"));
    //  document.Root.Add(result.Select(s => s.ToXml()));
    //}

    //private static void Quests() {
    //  var result = new List<Quest>();
    //  var assets = Assets.All.XPathSelectElements("//Asset[Template='Quest']").ToList();
    //  assets.ForEach((asset) => {
    //    ConsoleWriteGUID(asset.XPathSelectElement("Values/Standard/GUID").Value);
    //    var item = new Quest(asset);
    //    result.Add(item);
    //  });
    //  var document = new XDocument();
    //  document.Add(new XElement("Quests"));
    //  document.Root.Add(result.Select(s => s.ToXml()));
    //}

    private static void Expeditions() {
      var result = new List<Expedition>();
      var assets = Assets.All.XPathSelectElements("//Asset[Template='Expedition']").AsParallel();
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
      var xRewardPools = new XElement("RP");

      //RewardPool and RewardItemPool
      var RewardItemPoolsAssets = Assets.All
         .XPathSelectElements($"//Asset[Template='RewardPool']")
         .Concat(Assets.All.XPathSelectElements($"//Asset[Template='RewardItemPool']"))
         .ToList();
      foreach (var RewardPool in RewardItemPoolsAssets) {
        var xPool = new XElement("P");
        xRewardPools.Add(xPool);
        xPool.Add(new XAttribute("ID", RewardPool.XPathSelectElement("Values/Standard/GUID").Value));
        xPool.Add(new XAttribute("N", RewardPool.XPathSelectElement("Values/Standard/Name").Value));
        if (RewardPool.XPathSelectElement("Values/RewardPool/ItemsPool")?.HasElements ?? false) {
          var xItems = new XElement("IL");
          xPool.Add(xItems);
          foreach (var item in RewardPool.XPathSelectElements("Values/RewardPool/ItemsPool/Item")) {
            var itemId = item.XPathSelectElement("ItemLink")?.Value ?? item.XPathSelectElement("ItemGroup")?.Value;
            if (itemId != null) {
              var xItem = new XElement("I");
              xItems.Add(xItem);
              xItem.Add(new XAttribute("ID", itemId));
              xItem.Add(new XAttribute("W", item.XPathSelectElement("Weight")?.Value ?? "1"));
            }
          }
        }
      }

      //ResourcePool
      var ResourcePoolAssets = Assets.All
         .XPathSelectElements($"//Asset[Template='ResourcePool']")
         .ToList();
      foreach (var ResourcePool in ResourcePoolAssets) {
        var xPool = new XElement("P");
        xRewardPools.Add(xPool);
        xPool.Add(new XAttribute("ID", ResourcePool.XPathSelectElement("Values/Standard/GUID").Value));
        xPool.Add(new XAttribute("N", ResourcePool.XPathSelectElement("Values/Standard/Name").Value));
        if (ResourcePool.XPathSelectElement("Values/ResourceRewardPool/PossibleRewards")?.HasElements ?? false) {
          var xItems = new XElement("I");
          xPool.Add(xItems);
          foreach (var item in ResourcePool.XPathSelectElements("Values/ResourceRewardPool/PossibleRewards/Item")) {
            var itemId = item.XPathSelectElement("Resource")?.Value;
            if (itemId != null) {
              var xItem = new XElement("I");
              xItems.Add(xItem);
              xItem.Add(new XAttribute("ID", itemId));
              xItem.Add(new XAttribute("W", item.XPathSelectElement("Weight")?.Value ?? "100"));
            }
          }
        }
      }

      //AssetGroups
      var AssetGroups = Assets.BaseGame
        .Descendants("Groups")
        .Where(g => g.Element("GUID") != null)
        .Concat(Assets.BaseGame
         .Descendants("Group")
         .Where(g => g.Element("GUID") != null))
         .ToList();
      foreach (var AssetGroup in AssetGroups) {
        var childAssets = AssetGroup.Descendants("Asset").Where(a => a.XPathSelectElement("Values/Item") != null || a.XPathSelectElement("Values/Product") != null);
        if (childAssets.Any()) {
          var xPool = new XElement("P");
          xRewardPools.Add(xPool);
          xPool.Add(new XAttribute("ID", AssetGroup.Element("GUID").Value));
          var xItems = new XElement("I");
          xPool.Add(xItems);
          foreach (var item in childAssets) {
            var itemId = item.XPathSelectElement("Values/Standard/GUID")?.Value;
            if (itemId != null) {
              var xItem = new XElement("I");
              xItems.Add(xItem);
              xItem.Add(new XAttribute("ID", itemId));
              xItem.Add(new XAttribute("W", item.XPathSelectElement("Weight")?.Value ?? "100"));
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
      var ResultEvents = new ConcurrentDictionary<XElement, ConcurrentBag<HashSet<AssetWithWeight>>>();
      var count = 1;

      var decicions = Assets.All
          .XPathSelectElements("//Asset[Template='ExpeditionDecision']")
          .Where(f => f.XPathSelectElement("Values/Reward/RewardAssets")?.Elements("Item").Any(r => r.Element("Reward")?.Value != null) ?? false)
          .ToList();

      decicions.AddRange(Assets.All.XPathSelectElements("//Asset[Template='ExpeditionTrade']"));
      ConsoleWriteHeadline(template + "  Total: " + decicions.Count);

      decicions.AsParallel().ForAll(decicion => {
        ConsoleWriteGUID(decicion.XPathSelectElement("Values/Standard/GUID").Value + " - " + count++);
        foreach (var events in VerasFindExpeditionEvents(decicion.XPathSelectElement("Values/Standard/GUID").Value, new HashSet<String>(), new Details { decicion })) {
          foreach (var item in events) {
            if (ResultEvents.ContainsKey(item.Source)) {
              ResultEvents[item.Source].Add(item.Details);
            }
            else {
              ResultEvents.TryAdd(item.Source, new ConcurrentBag<HashSet<AssetWithWeight>> { item.Details });
            }
          }
        }
      });

      var document = new XDocument();
      document.Add(new XElement(template));
      document.Root.Add(ResultEvents.GroupBy(f => f.Key.XPathSelectElement("Values/Standard/Name").Value)
          .Select(g => g.First())
          .OrderBy(s => { var str = ("000" + s.Value.Count); return str.Substring(str.Length - 4) + " " + s.Key.XPathSelectElement("Values/Standard/GUID").Value; })
          .Select(s => ToXml(s)));
      document.SaveIndent($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\{template}.xml");

      //local method ExpeditionEvents ToXml
      XElement ToXml(KeyValuePair<XElement, ConcurrentBag<HashSet<AssetWithWeight>>> events) {
        var xRoot = new XElement("EE");
        xRoot.Add(new XAttribute("ID", events.Key.XPathSelectElement("Values/Standard/GUID").Value));
        xRoot.Add(new Description(events.Key).ToXml("N"));
        var xPaths = new XElement("PL");
        xRoot.Add(xPaths);
        foreach (var path in events.Value) {
          var xPath = new XElement("P");
          xPaths.Add(xPath);
          xPath.Add(new XAttribute("ID", path.First().Asset.XPathSelectElement("Values/Standard/Name").Value.Split(' ').Last()));

          var xRewards = new XElement("R");
          xPath.Add(xRewards);
          var rewards = path.First().Asset.XPathSelectElements("Values/Reward/RewardAssets/Item");
          if (rewards.Any()) {
            foreach (var reward in rewards) {
              var xReward = new XElement("I");
              xRewards.Add(xReward);
              xReward.Add(new XAttribute("ID", reward.XPathSelectElement("Reward").Value));
              if (reward.XPathSelectElement("Amount") != null) {
                xReward.Add(new XAttribute("A", reward.XPathSelectElement("Amount").Value));
              }
            }
          }
          else {
            var Products = path.First().Asset.XPathSelectElements("Values/ExpeditionTrade/AvailableGoods/Item");
            if (Products.Any()) {
              foreach (var Product in Products) {
                var xReward = new XElement("I");
                xRewards.Add(xReward);
                xReward.Add(new XAttribute("ID", Product.Element("Product").Value));
                if (Product.Element("Amount")?.Value is string value) {
                  xReward.Add(new XAttribute("A", value));
                }
              }
            }
            var Items = path.First().Asset.XPathSelectElements("Values/ExpeditionTrade/AvailableItems/Item");
            if (Items.Any()) {
              foreach (var Item in Items) {
                var xReward = new XElement("I");
                xRewards.Add(xReward);
                xReward.Add(new XAttribute("ID", Item.Element("Item").Value));
                if (Item.Element("Amount")?.Value is string value) {
                  xReward.Add(new XAttribute("A", value));
                }
              }
            }
          }

          var xOptions = new XElement("OL");
          xPath.Add(xOptions);
          foreach (var option in path) {
            if (option.Asset.XPathSelectElement("Template").Value == "ExpeditionOption" ||
                option.Asset.XPathSelectElement("Template").Value == "ExpeditionMapOption") {
              var xOption = new XElement("O");
              xOptions.AddFirst(xOption);
              xOption.Add(new XAttribute("ID", option.Asset.XPathSelectElement("Values/Standard/GUID").Value));
              var text = new Description(option.Asset);
              if (text.Languages[Data.Languages.English] == "Confirm") {
                text = new Description("145001", GameTypes.Anno_1800);
              }
              else if (text.Languages[Data.Languages.English] == "Cancel") {
                text = new Description("145002", GameTypes.Anno_1800);
              }
              xOption.Add(text.ToXml("T"));
              if (option.Asset.XPathSelectElement("Values/ExpeditionOption/OptionAttribute")?.Value != null) {
                xOption.Add(new Description(Assets.GetDescriptionID(option.Asset.XPathSelectElement("Values/ExpeditionOption/OptionAttribute").Value), GameTypes.Anno_1800).ToXml("OA"));
              }
              if (option.Asset.XPathSelectElement("Values/ExpeditionOption/Requirements")?.HasElements == true) {
                var xRequirements = new XElement("R");
                xOption.Add(xRequirements);
                foreach (var requirement in option.Asset.XPathSelectElements("Values/ExpeditionOption/Requirements/Item")) {
                  var xItem = new XElement("I");
                  xRequirements.Add(xItem);
                  if (requirement.XPathSelectElement("NeededAttribute")?.Value != null) {
                    xItem.Add(new Description(Assets.GetDescriptionID(requirement.XPathSelectElement("NeededAttribute").Value), GameTypes.Anno_1800).ToXml("NA"));
                  }
                  if (requirement.XPathSelectElement("ItemOrProduct")?.Value != null) {
                    xItem.Add(new XAttribute("ID", requirement.XPathSelectElement("ItemOrProduct").Value));
                  }
                  if (requirement.XPathSelectElement("Amount")?.Value != null) {
                    xItem.Add(new XAttribute("A", requirement.XPathSelectElement("Amount").Value));
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
      List<SourceWithDetailsList> VerasFindExpeditionEvents(string id, HashSet<string> visitedEvents = default, Details mainDetails = default) {
        mainDetails = (mainDetails == default) ? new Details() : mainDetails;
        mainDetails.PreviousIDs.Add(id);
        var mainResult = new List<SourceWithDetailsList>();

        visitedEvents.Add(id);

        if (!Assets.References.ContainsKey(id)) {
          return mainResult;
        }
        var cachedLinks = Assets.References[id];

        foreach (var asset in cachedLinks) {
          foreach (var reference in asset.Descendants()) {
            if ("GUID".Equals(reference.Name.LocalName) || !id.Equals(reference.Value) || reference.HasElements)
              continue;

            var Details = new Details(mainDetails);
            var key = asset.XPathSelectElement("Values/Standard/GUID").Value;
            if (asset.Element("Template") == null || mainDetails.PreviousIDs.Contains(key)) {
              continue;
            }
            switch (asset.Element("Template").Value) {
              case "GuildhouseItem":
                //Ignore
                break;

              case "Expedition":

                break;

              case "ExpeditionDecision":
              case "ExpeditionTrade":
                if (reference.Name.LocalName == "Reward" ||
                  reference.Name.LocalName == "Product" ||
                  reference.Name.LocalName == "Item") {
                  goto case "SearchAgain";
                }
                else if (reference.Name.LocalName != "Option" &&
                  reference.Name.LocalName != "FollowupSuccessOption" &&
                  reference.Name.LocalName != "FollowupFailOrCancelOption" &&
                  reference.Name.LocalName != "InsertEvent") {
                  break;
                }
                goto case "SearchAgain";

              case "ExpeditionMapOption":
              case "ExpeditionOption":
                if (reference.Name.LocalName == "ItemOrProduct") {
                  break;
                }
                if (reference.Name.LocalName != "Decision") {
                  break;
                }
                var name = asset.XPathSelectElement("Values/Standard/Name").Value;
                if (name == "Continue option")
                  break;
                goto case "SearchAgain";
              case "ExpeditionBribe":
                if (reference.Name.LocalName == "Item") {
                  break;
                }
                if (reference.Name.LocalName != "FollowupSuccessOption" &&
                  reference.Name.LocalName != "FollowupFailOrCancelOption") {
                  break;
                }
                goto case "SearchAgain";
              case "SearchAgain":
                Details.Add(asset);
                //if (!visitedEvents.Contains(key))
                mainResult.AddRange(VerasFindExpeditionEvents(key, visitedEvents, Details).AsEnumerable());
                break;

              case "ExpeditionEvent":
                if (reference.Name.LocalName != "StartDecision") {
                  break;
                }

                var sList = new SourceWithDetailsList();
                sList.AddSourceAsset(asset, new HashSet<AssetWithWeight>(Details.Items.Select(i => new AssetWithWeight(i))));
                mainResult.Add(sList);
                break;

              default:
                Debug.WriteLine(asset.Element("Template").Value);
                //ignore
                break;
            }
          }
        }

        return mainResult;
      }
    }

    private static void ProcessingTourism() {
      ConsoleWriteHeadline("Processing Tourism");
      var TourismAsset = Assets.All.Descendants("Asset").FirstOrDefault(l => l.Element("Template")?.Value == "TourismFeature");
      var xRoot = new XElement("CL");
      foreach (var pool in TourismAsset.Descendants("SpecialistPools").FirstOrDefault()?.Elements()) {
        var id = pool.Element("CityLevel").Value;
        var xStatus = new XElement("S");
        xRoot.Add(xStatus);
        var desc = new Description("145011", GameTypes.Anno_1800).InsertBefore(Assets.TourismThresholds[id]);
        xStatus.Add(new XAttribute("P", pool.Element("Pool").Value));
        xStatus.Add(desc.ToXml("R"));
        //xStatus.Add(new Description(Assets.TourismThresholds[id].Element("CityStatusFluff").Value).ToXml("Text"));
      }

      foreach (var pool in TourismAsset.Descendants("UnlockablePools").SelectMany(p => p.Elements())) {
        var xStatus = new XElement("S");
        xRoot.Add(xStatus);
        xStatus.Add(new XElement(new Description(pool.Element("UnlockingSpecialist").Value, GameTypes.Anno_1800).ToXml("R")));
        xStatus.Add(new XAttribute("P", pool.Element("Pool").Value));
      }

      foreach (var pool in TourismAsset.Descendants("SpecialistPoolsThroughSets").SelectMany(p => p.Elements())) {
        var xStatus = new XElement("S");
        xRoot.Add(xStatus);
        xStatus.Add(new XElement(new Description(pool.Element("UnlockingSetBuff").Value, GameTypes.Anno_1800).ToXml("R")));
        xStatus.Add(new XAttribute("P", pool.Element("Pool").Value));
      }

      foreach (var pool in Assets.All.Descendants("Asset").Where(a => a.Descendants("OverrideSpecialistPool").Any())) {
        var xStatus = new XElement("S");
        xRoot.Add(xStatus);
        xStatus.Add(new XElement(new Description(pool).ToXml("R")));
        xStatus.Add(new XAttribute("P", pool.Descendants("OverrideSpecialistPool").First().Value));
      }

      var document = new XDocument(xRoot);
      document.Save($@"{Program.PathRoot}\Modified\Assets_Tourism.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\Tourism.xml");
    }

    #endregion Private Methods
  }
}