//using Newtonsoft.Json;
using RDA.Data;
using RDA.Templates;
using RDA.Veras;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace RDA.Library {
    internal static class VerasExtensions {
        #region Constructors

        #endregion Constructors

        #region Properties

        public static Dictionary<string, XElement> Events { get; set; } = new Dictionary<string, XElement>();

        public static Dictionary<string, RewardWithDetailsList> Sources { get; set; } = new Dictionary<string, RewardWithDetailsList>();

        #endregion Properties

        #region Methods
        public static void ProcessingRewardPools() {
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
                else {
                }
            }
            var document = new XDocument(xRewardPools);
            document.Save($@"{Program.PathRoot}\Modified\Assets_RewardPools.xml");
            document.Save($@"{Program.PathViewer}\Resources\Assets\RewardPools.xml");
        }
        public static void ProcessingExpeditionEvents() {
            var decicions = Program.Original
                .XPathSelectElements("//Asset[Template='ExpeditionDecision']")
                .Where(f => f.XPathSelectElement("Values/Reward")?.HasElements ?? false)
                .ToList();
            var template = "ExpeditionEvents";
            //var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}' and Values/Standard/GUID=191507]").ToList();
            Console.WriteLine("Total: " + decicions.Count);
            var count = 0;
            var result = new Dictionary<XElement, List<HashSet<XElement>>>();
            foreach (var decicion in decicions) {
                count++;
                //if (asset.XPathSelectElement("Values/Item/HasAction")?.Value == "1") return;
                Console.WriteLine(decicion.XPathSelectElement("Values/Standard/GUID").Value + " - " + count);
                var events = VerasFindExpeditionEvents(decicion.XPathSelectElement("Values/Standard/GUID").Value, new Details { decicion });
                // var item = new Asset(decicion, false);
                foreach (var item in events) {
                    if (result.ContainsKey(item.Root)) {
                        result[item.Root].Add(item.Details);
                    }
                    else {
                        result.Add(item.Root, new List<HashSet<XElement>> { item.Details });
                    }
                }
            }
            var document = new XDocument();
            document.Add(new XElement(template));
            document.Root.Add(result.Select(s => s.ToXml()));
            document.Save($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
            document.Save($@"{Program.PathViewer}\Resources\Assets\{template}.xml");
        }

        //ExpeditionEvents
        public static XElement ToXml(this KeyValuePair<XElement, List<HashSet<XElement>>> events) {
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

        private static RewardWithDetailsList VerasFindExpeditionEvents(string id, Details mainDetails = default, RewardWithDetailsList inResult = default) {
            mainDetails = (mainDetails == default) ? new Details() : mainDetails;
            mainDetails.PreviousIDs.Add(id);
            var mainResult = inResult ?? new RewardWithDetailsList();
            var resultstoadd = new List<RewardWithDetailsList>();
            var links = Program.Original.Root.XPathSelectElements($"//*[text()={id} and not(self::GUID)]").ToArray();
            if (links.Length > 0) {
                for (var i = 0; i < links.Length; i++) {
                    var element = links[i];
                    while (element.Name.LocalName != "Asset" || !element.HasElements) {
                        element = element.Parent;
                    }
                    if (element.Element("Template") == null) {
                        continue;
                    }
                    var key = element.XPathSelectElement("Values/Standard/GUID").Value;
                    if (mainDetails.PreviousIDs.Contains(key)) {
                        continue;
                    }
                    var Details = new Details(mainDetails);
                    var result = mainResult.Copy();
                    switch (element.Element("Template").Value) {
                        case "AssetPool":
                        case "TutorialQuest":
                        case "SettlementRightsFeature":
                        case "Profile_2ndParty":
                        case "GuildhouseItem":
                        case "HarborOfficeItem":
                        case "HarbourOfficeBuff":
                        case "MonumentEvent":

                        case "Expedition":
                        case "Profile_3rdParty":
                        case "Profile_3rdParty_Pirate":
                        case "A7_QuestEscortObject":
                        case "A7_QuestDeliveryObject":
                        case "A7_QuestDestroyObjects":
                        case "A7_QuestPickupObject":
                        case "A7_QuestFollowShip":
                        case "A7_QuestPhotography":
                        case "A7_QuestStatusQuo":
                        case "A7_QuestItemUsage":
                        case "A7_QuestSustain":
                        case "A7_QuestPicturePuzzleObject":
                        case "Quest":
                        case "CollectablePicturePuzzle":
                        case "MonumentEventReward":
                        case "TourismFeature":

                        case "RewardPool":
                        case "RewardItemPool":
                        case "ExpeditionEventPool":
                            // ignore
                            break;

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
                            throw new NotImplementedException(element.Element("Template").Value);
                    }
                    resultstoadd.Add(result);
                }
            }

            foreach (var item in resultstoadd) {
                mainResult.AddSourceAsset(item);
            }
            return mainResult;
        }

        public static void ProcessingItems(String template, bool findSources = true) {
            var result = new List<Asset>();
            var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}']").ToList();
            //var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}' and Values/Standard/GUID=191507]").ToList();
            Console.WriteLine("Total: " + assets.Count);
            var count = 0;
            foreach (var asset in assets) {
                count++;
                //if (asset.XPathSelectElement("Values/Item/HasAction")?.Value == "1") return;
                Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value + " - " + count);
                var item = new Asset(asset, findSources);
                result.Add(item);
            }
            var document = new XDocument();
            document.Add(new XElement(template));
            document.Root.Add(result.Select(s => s.ToXml()));
            document.Save($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
            document.Save($@"{Program.PathViewer}\Resources\Assets\{template}.xml");
        }

        public static RewardWithDetailsList VerasFindSourcesForItems(this String id, Details mainDetails = default, RewardWithDetailsList inResult = default) {
            mainDetails = (mainDetails == default) ? new Details() : mainDetails;
            mainDetails.PreviousIDs.Add(id);
            var mainResult = inResult ?? new RewardWithDetailsList();
            var resultstoadd = new List<RewardWithDetailsList>();
            var links = Program.Original.Root.XPathSelectElements($"//*[text()={id} and not(self::GUID)]").ToArray();
            if (links.Length > 0) {
                for (var i = 0; i < links.Length; i++) {
                    var element = links[i];
                    while (element.Name.LocalName != "Asset" || !element.HasElements) {
                        element = element.Parent;
                    }
                    var Details = new Details(mainDetails);
                    var result = mainResult.Copy();
                    if (element.Element("Template") == null) {
                        if (element.XPathSelectElement("Values/Profile/ShipDropRewardPool")?.Value == id) {
                            result.AddSourceAsset(element.GetProxy("ShipDrop"), new HashSet<XElement> { element.GetProxy("ShipDrop") });
                            resultstoadd.Add(result);
                            continue;
                        }
                        else {
                            continue;
                        }

                    }

                    var key = element.XPathSelectElement("Values/Standard/GUID").Value;
                    if (mainDetails.PreviousIDs.Contains(key)) {
                        continue;
                    }

                    switch (element.Element("Template").Value) {
                        case "AssetPool":
                        case "TutorialQuest":
                        case "SettlementRightsFeature":
                        case "Profile_2ndParty":
                        case "GuildhouseItem":
                        case "HarborOfficeItem":
                        case "HarbourOfficeBuff":
                        case "MonumentEvent":
                        case "MainQuest":
                            // ignore
                            break;

                        case "Expedition":

                            if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                                if (Details.Items.Count == 0) {
                                    Details.Add(element);
                                }

                                result.AddSourceAsset(element, Details.Items);
                            }
                            break;

                        case "Profile_3rdParty":
                        case "Profile_3rdParty_Pirate":
                            if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")
                                && element.XPathSelectElement("Standard/Profile/ShipDropRewardPool")?.Value == id) {
                                result.AddSourceAsset(element.GetProxy("ShipDrop"), new HashSet<XElement> { element.GetProxy("ShipDrop") });
                                break;
                            }

                            goto case "A7_QuestEscortObject";
                        case "A7_QuestEscortObject":
                        case "A7_QuestDeliveryObject":
                        case "A7_QuestDestroyObjects":
                        case "A7_QuestPickupObject":
                        case "A7_QuestFollowShip":
                        case "A7_QuestPhotography":
                        case "A7_QuestStatusQuo":
                        case "A7_QuestItemUsage":
                        case "A7_QuestSustain":
                        case "A7_QuestPicturePuzzleObject":
                        case "Quest":
                        case "CollectablePicturePuzzle":
                        case "MonumentEventReward":
                        case "TourismFeature":
                            if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                                result.AddSourceAsset(element, new HashSet<XElement> { element });
                            }
                            break;

                        case "ExpeditionDecision":
                            var check = false;
                            var name = element.XPathSelectElement("Values/Standard/Name")?.Value;
                            var reward = element.XPathSelectElement("Values/Reward/RewardAssets");
                            check = reward?.HasElements == true && reward.Elements("Item").Any(l => l.Element("Reward").Value == id);
                            if (check || element.XPathSelectElements($"//*[text()={id} and not(self::GUID) and (self::InsertEvent)]").Any()) {
                                Details.Add(element);
                            }
                            goto case "ExpeditionOption";
                        case "ExpeditionOption":
                        case "ExpeditionTrade":
                        case "ExpeditionEvent":
                        case "ExpeditionEventPool":
                            if (Sources.ContainsKey(key)) {
                                result.AddSourceAsset(Sources[key].Copy(), Details);
                                break;
                            }
                            goto case "RewardPool";
                        case "RewardPool":
                        case "RewardItemPool":

                            if (Sources.ContainsKey(key)) {
                                result.AddSourceAsset(Sources[key].Copy());
                                break;
                            }

                            VerasFindSourcesForItems(key, Details, result);
                            if (!Sources.ContainsKey(key)) {
                                Sources.Add(key, result.Copy());
                            }
                            break;

                        default:
                            Debug.WriteLine(element.Element("Template").Value);
                            break;
                            //throw new NotImplementedException(element.Element("Template").Value);
                    }
                    resultstoadd.Add(result);
                }
            }

            foreach (var item in resultstoadd) {
                mainResult.AddSourceAsset(item);
            }
            return mainResult;
        }

        private static XElement GetProxy(this XElement element, string proxyName) {
            var xRoot = new XElement("Proxy");
            var xTemplate = new XElement("Template");
            xTemplate.Value = proxyName;
            xRoot.Add(xTemplate);
            xRoot.Add(element);
            xRoot.Add(new XElement("Values", element.XPathSelectElement("Values/Standard")));
            return xRoot;
        }

        public static XElement VerasFindParent(this String id, string[] ParentTypes, List<String> previousIDs = null) {
            if (Events.ContainsKey(id)) {
                return Events[id];
            }
            XElement result = null;
            previousIDs = previousIDs ?? new List<string>();
            previousIDs.Add(id);
            var links = Program.Original.Root.XPathSelectElements($"//*[text()={id} and not(self::GUID) and not(self::InsertEvent)]").ToArray();
            if (links.Length > 0) {
                for (var i = 0; i < links.Length; i++) {
                    var element = links[i];
                    while (element.Name.LocalName != "Asset" || !element.HasElements) {
                        element = element.Parent;
                    }
                    if (element.Element("Template") == null) {
                        continue;
                    }

                    if (previousIDs.Contains(element.XPathSelectElement("Values/Standard/GUID").Value)) {
                        continue;
                    }

                    if (ParentTypes.Contains(element.Element("Template").Value)) {
                        return element;
                    }
                    result = VerasFindParent(element.XPathSelectElement("Values/Standard/GUID").Value, ParentTypes, previousIDs);
                    if (result != null) {
                        break;
                    }
                }
            }
            if (!Events.ContainsKey(id)) {
                Events.Add(id, result);
            }
            return result;
        }

        #endregion Methods
    }
}