//using Newtonsoft.Json;
using RDA.Templates;
using RDA.Veras;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace RDA.Library {

    internal static class VerasExtensions {

        #region Constructors

        static VerasExtensions() {
        }

        #endregion Constructors

        #region Properties

        public static Dictionary<string, XElement> Events { get; set; } = new Dictionary<string, XElement>();

        public static Dictionary<string, RewardWithDetailsList> Sources { get; set; } = new Dictionary<string, RewardWithDetailsList>();

        #endregion Properties

        #region Methods

        public static void ProcessingItems(String template) {
            var result = new List<Asset>();
            var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}']").ToList();
            //var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}' and Values/Standard/GUID=191507]").ToList();
            Console.WriteLine("Total: " + assets.Count);
            var count = 0;
            foreach (var asset in assets) {
                count++;
                //if (asset.XPathSelectElement("Values/Item/HasAction")?.Value == "1") return;
                Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value + " - " + count);
                var item = new Asset(asset, true);
                result.Add(item);
            };
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
                            check = reward?.HasElements == true ? reward.Elements("Item").Any(l => l.Element("Reward").Value == id) : false;
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

        internal static void ProcessingExpeditionEvents() {
            var t = Program.Original.XPathSelectElements("//Asset[Template='ExpeditionDecision']").ToList();
            var o = t.Where(f => f.XPathSelectElement("Values/Reward")?.HasElements ?? false).ToList();
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

        //private static void LoadSources() {
        //    using (var file = File.OpenRead(Program.PathRoot + @"\Modified\VerasSources.xml")) {
        //        using (var textReader = new StreamReader(file)) {
        //            using (var jReader = new JsonTextReader(textReader)) {
        //                var serializer = JsonSerializer.Create(new JsonSerializerSettings { });
        //                Sources = serializer.Deserialize<Dictionary<string, RewardWithDetailsList>>(jReader);
        //            }
        //        }
        //    }
        //}

        //private static void SaveSources() {
        //    try {
        //        using (TextWriter file = File.CreateText(Program.PathRoot + @"\Modified\VerasSources.xml")) {
        //            using (var jWriter = new JsonTextWriter(file)) {
        //                jWriter.Formatting = Formatting.Indented;
        //                var serializer = JsonSerializer.Create(new JsonSerializerSettings { });
        //                serializer.Serialize(jWriter, Sources);
        //            }
        //        }
        //    }
        //    catch (Exception ex) {
        //        Console.WriteLine(ex);
        //    }
        //}
        public static XElement ToXElement<T>(this T obj) {
            using (var memoryStream = new MemoryStream()) {
                using (TextWriter streamWriter = new StreamWriter(memoryStream)) {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }
        }

        public static T FromXElement<T>(this XElement xElement) {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }
        #endregion Methods
    }
}