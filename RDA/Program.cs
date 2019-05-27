using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;
using RDA.Library;
using RDA.Templates;

namespace RDA {
    [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    internal static class Program {
        #region Fields
        private static readonly Dictionary<Int32, String> Descriptions = new Dictionary<Int32, String>();
        internal static XDocument Original;
        internal static String PathRoot;
        internal static String PathViewer;
        internal static XDocument TextDE;
        internal static Dictionary<String, String> DescriptionEN;
        internal static Dictionary<String, String> DescriptionDE;
        #endregion

        #region Private Methods
        private static void Main(String[] args) {
            Program.PathViewer = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.Parent.FullName + @"\AssetViewer";
            Program.PathRoot = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", String.Empty)).Parent.Parent.FullName;
            Program.Original = XDocument.Load(Program.PathRoot + @"\Original\assets.xml");

            // Helper
            //Helper.ExtractTextEnglish(Program.PathRoot + @"\Original\texts_english.xml");
            //Helper.ExtractTextGerman(Program.PathRoot + @"\Original\texts_german.xml");
            //Helper.ExtractTemplateNames(Program.PathRoot + @"\Original\assets.xml");

            // Descriptions
            Program.DescriptionEN = XDocument.Load(Program.PathRoot + @"\Modified\Texts_English.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
            Program.DescriptionDE = XDocument.Load(Program.PathRoot + @"\Modified\Texts_German.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);

            // World Fair
            //Monument.Create();

            //VerasExtensions.ProcessingItemSpecialAction();
            //VerasExtensions.ProcessingActiveItem();
            //VerasExtensions.ProcessingItemSpecialActionVisualEffect();
            //VerasExtensions.ProcessingProducts();

            // Assets
            //VerasExtensions.ProcessingRewardPools();
            VerasExtensions.ProcessingItems("Product");
            VerasExtensions.ProcessingItems("ActiveItem");
            VerasExtensions.ProcessingItems("ItemSpecialActionVisualEffect");
            VerasExtensions.ProcessingItems("ItemSpecialAction");
            VerasExtensions.ProcessingItems("GuildhouseItem");
            VerasExtensions.ProcessingItems("TownhallItem");
            VerasExtensions.ProcessingItems("HarborOfficeItem");
            VerasExtensions.ProcessingItems("VehicleItem");
            VerasExtensions.ProcessingItems("ShipSpecialist");
            VerasExtensions.ProcessingItems("CultureItem");
            VerasExtensions.ProcessingItems("BuildPermitBuilding");

            //Third Party
            //Program.ProcessingThirdParty();

            // Quests
            //Program.QuestGiver();
            //Program.Quests();

            // Expeditions
            //Program.Expeditions();
            //VerasExtensions.ProcessingExpeditionEvents();
        }

        private static void ProcessingItems(String template) {
            var result = new List<Asset>();
            var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}']").ToList().AsParallel();
            //var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}' and Values/Standard/GUID=191507]").ToList();
            assets.ForAll((asset) => {
                //if (asset.XPathSelectElement("Values/Item/HasAction")?.Value == "1") return;
                Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value);
                var item = new Asset(asset, true);
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
            var assets = Program.Original.XPathSelectElements($"//Asset[Template='Profile_3rdParty' or Template='Profile_3rdParty_Pirate']").ToList().AsParallel();
            assets.ForAll((asset) => {
                if (!asset.XPathSelectElements("Values/Trader/Progression/*/OfferingItems").Any())
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
        #endregion

    }
}