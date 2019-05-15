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

      // Helper
      //Helper.ExtractTextEnglish(Program.PathRoot + @"\Original\texts_english.xml");
      //Helper.ExtractTextGerman(Program.PathRoot + @"\Original\texts_german.xml");
      //Helper.ExtractTemplateNames(Program.PathRoot + @"\Original\assets.xml");

      // Descriptions
      Program.DescriptionEN = XDocument.Load(Program.PathRoot + @"\Modified\Texts_English.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
      Program.DescriptionDE = XDocument.Load(Program.PathRoot + @"\Modified\Texts_German.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);

      // World Fair
      //Monument.Create();

      // Assets
      //Program.ProcessingItems("GuildhouseItem");
      //Program.ProcessingItems("TownhallItem");
      //Program.ProcessingItems("HarborOfficeItem");
      //Program.ProcessingItems("VehicleItem");
      //Program.ProcessingItems("ShipSpecialist");
      //Program.ProcessingItems("CultureItem");
      //Program.ProcessingThirdParty();

      // Quests
      //Program.QuestGiver();
      //Program.Quests();

      // Expeditions
      //Program.Expeditions();

      // Mod
      //Program.RemoveThirdPartyMessages();
      //Program.RemoveIncident1();
      //Program.RemoveIncident2();
      //Program.RemoveExplosions();
      //Program.NewspaperBalancing();
      Program.ModifyItemsEliBleakworth();

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
        if (!asset.XPathSelectElements("Values/Trader/Progression/*/OfferingItems").Any()) return;
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
    //private static void MadameKahina() {
    //  var document = XDocument.Load(@"C:\Users\Andreas\Downloads\Anno 1800\Schiff-Zoom-Einfluss-Mod\data0\data\config\export\main\asset\assets.xml");
    //  var rewardPool_MidGame = document.XPathSelectElement($"//Asset[Values/Standard/GUID=190556]/Values/RewardPool/ItemsPool");
    //  foreach (var item in rewardPool_MidGame.Elements().ToArray()) {
    //    if (item.Element("ItemLink").Value != "192925") item.Remove();
    //  }
    //  //
    //  var rewardPool_CommonCultural = document.XPathSelectElement($"//Asset[Values/Standard/GUID=192925]/Values/RewardPool/ItemsPool");
    //  foreach (var item in rewardPool_CommonCultural.Elements().ToArray()) {
    //    if (item.Element("ItemLink").Value != "192768") item.Remove();
    //  }
    //  //
    //  var rewardPool_EuropeanAnimals = document.XPathSelectElement($"//Asset[Values/Standard/GUID=192768]/Values/RewardPool/ItemsPool");
    //  rewardPool_EuropeanAnimals.Elements().Remove();
    //  rewardPool_EuropeanAnimals.Add(new XElement("Item", new XElement("ItemLink", "190746")));
    //  rewardPool_EuropeanAnimals.Add(new XElement("Item", new XElement("ItemLink", "190063")));
    //  rewardPool_EuropeanAnimals.Add(new XElement("Item", new XElement("ItemLink", "190748")));
    //  rewardPool_EuropeanAnimals.Add(new XElement("Item", new XElement("ItemLink", "191380")));
    //  rewardPool_EuropeanAnimals.Add(new XElement("Item", new XElement("ItemLink", "191381")));
    //  rewardPool_EuropeanAnimals.Add(new XElement("Item", new XElement("ItemLink", "192443")));
    //  //
    //}
    private static void RemoveThirdPartyMessages() {
      var files = new[] {
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\assets.xml",
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data10\data\config\export\main\asset\assets.xml"
      };
      var triggers = new[] {
        "ClickKontor",
        "ActiveTradeConfirmation",
        "OpenActiveTradeMenu",
        "ClickNPCShip",
        "ClickNPCBuilding",
        "MenuIdleMessage"
      };
      foreach (var file in files) {
        var document = XDocument.Load(file);
        var assets = document.Root.XPathSelectElements($"//Asset[Template='Profile_3rdParty']").ToArray();
        foreach (var assetProfile in assets) {
          var idMessage = assetProfile.XPathSelectElement("Values/ParticipantMessageObject/ParticipantMessages").Value;
          var message = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID={idMessage}]");
          foreach (var trigger in triggers) {
            var items = message.XPathSelectElements($"Values/ParticipantMessages/MessageTriggers/{trigger}/Reactions"); //.Remove();
            items.Remove();
          }
        }
        document.Save(file);
        var lines = File.ReadAllLines(file).Skip(1);
        File.WriteAllLines(file, lines);
      }
    }
    private static void RemoveIncident1() {
      var file = @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\properties.xml";
      var document = XDocument.Load(file);
      var items = document.Root.XPathSelectElements($"//Groups/Group/DefaultValues/GeneralIncidentConfiguration/ProgressConfig/*/PerIncidentConfig/*/TargetInfectionFactor").ToArray();
      foreach (var item in items) {
        item.Value = "0";
      }
      document.Save(file);
      var lines = File.ReadAllLines(file).Skip(1);
      File.WriteAllLines(file, lines);
    }
    private static void RemoveIncident2() {
      var files = new[] {
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\assets.xml",
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data10\data\config\export\main\asset\assets.xml"
      };
      foreach (var file in files) {
        var document = XDocument.Load(file);
        var items = document.Root.XPathSelectElements($"//IncidentInfectable").ToArray();
        foreach (var item in items) {
          item.Value = String.Empty;
        }
        document.Save(file);
        var lines = File.ReadAllLines(file).Skip(1);
        File.WriteAllLines(file, lines);
      }
    }
    private static void RemoveExplosions() {
      var files = new[] {
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\assets.xml",
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data10\data\config\export\main\asset\assets.xml"
      };
      foreach (var file in files) {
        var document = XDocument.Load(file);
        var configuration = document.Root.XPathSelectElement("//Asset[Template='GeneralIncidentConfiguration']");
        foreach (var item in configuration.XPathSelectElements("Values/GeneralIncidentConfiguration/ProgressConfig/*/PerIncidentConfig/Explosion/TargetInfectionFactor")) {
          item.Value = "0";
        }
        foreach (var item in configuration.XPathSelectElements("Values/GeneralIncidentConfiguration/PerRegionUnlocks/*/IncidentUnlocks/Explosion/PerAreaRequiredAmount")) {
          item.Value = "999999";
        }
        var idExplosion = configuration.XPathSelectElement("Values/GeneralIncidentConfiguration/IncidentTypesConfig/Explosion/Config").Value;
        var explosion = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID={idExplosion}]");
        explosion.XPathSelectElement("Values/Incident/InitialInfectionFactor").Value = "0";
        //
        document.Save(file);
        var lines = File.ReadAllLines(file).Skip(1);
        File.WriteAllLines(file, lines);
      }
    }
    private static void NewspaperBalancing() {
      var files = new[] {
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\assets.xml",
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data10\data\config\export\main\asset\assets.xml"
      };
      foreach (var file in files) {
        var document = XDocument.Load(file);
        var configuration = document.Root.XPathSelectElement("//Asset[Template='NewspaperBalancing']");
        configuration.XPathSelectElement("Values/NewspaperBalancing/MinNewspaperInterval").Value = "99999990";
        configuration.XPathSelectElement("Values/NewspaperBalancing/MaxNewspaperInterval").Value = "99999999";
        //
        document.Save(file);
        var lines = File.ReadAllLines(file).Skip(1);
        File.WriteAllLines(file, lines);
      }
    }
    private static void ModifyItemsArchibaldBlake() {
      var files = new[] {
        @"D:\Sonstiges\Privat\GitHub\AssetViewer\RDA\Original\assets.xml",
        //@"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\assets.xml",
        //@"C:\Users\Andreas\Downloads\Anno 1800\Mod\data10\data\config\export\main\asset\assets.xml"
      };
      foreach (var file in files) {
        var document = XDocument.Load(file);
        var profile = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID=45]");
        // remove war ships
        profile.XPathSelectElements("Values/Trader/Progression/*/ShipsForSale/Item[ShipAsset=100437]").Remove();
        profile.XPathSelectElements("Values/Trader/Progression/*/ShipsForSale/Item[ShipAsset=1010062]").Remove();
        profile.XPathSelectElements("Values/Trader/Progression/*/ShipsForSale/Item[ShipAsset=100443]").Remove();
        profile.XPathSelectElements("Values/Trader/Progression/*/ShipsForSale/Item[ShipAsset=100442]").Remove();
        // OfferingItems
        var offeringItems = profile.XPathSelectElements("Values/Trader/Progression/*/OfferingItems");
        foreach (var offeringItem in offeringItems.ToArray()) {
          var poolMain = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID={offeringItem.Value}]");
          foreach (var itemLinkMain in poolMain.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").ToArray()) {
            var poolSub1 = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID={itemLinkMain.Value}]");
            foreach (var itemLinkSub1 in poolSub1.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").ToArray()) {
              var poolSub2 = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID={itemLinkSub1.Value}]");
              foreach (var itemLinkSub2 in poolSub2.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").ToArray()) {
                var item = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID={itemLinkSub2.Value}]");
                switch (item.Element("Template").Value) {
                  case "ActiveItem":
                  case "ItemSpecialAction":
                  case "HarborOfficeItem":
                    itemLinkSub2.Parent.Remove();
                    break;
                  case "VehicleItem":
                  case "GuildhouseItem":
                  case "TownhallItem":
                    // ignore
                    break;
                  default:
                    throw new NotImplementedException(item.Element("Template").Value);
                }
              }
              if (!poolSub2.XPathSelectElements("Values/RewardPool/ItemsPool/Item").Any()) {
                itemLinkSub1.Parent.Remove();
              }
            }
            if (!poolSub1.XPathSelectElements("Values/RewardPool/ItemsPool/Item").Any()) {
              itemLinkMain.Parent.Remove();
            }
          }
        }
        //
        document.Save(@"D:\Sonstiges\Privat\GitHub\AssetViewer\RDA\Original\assets_copy.xml");
        var lines = File.ReadAllLines(@"D:\Sonstiges\Privat\GitHub\AssetViewer\RDA\Original\assets_copy.xml").Skip(1);
        File.WriteAllLines(@"D:\Sonstiges\Privat\GitHub\AssetViewer\RDA\Original\assets_copy.xml", lines);
      }
    }
    private static void ModifyItemsEliBleakworth() {
      var files = new[] {
        @"D:\Sonstiges\Privat\GitHub\AssetViewer\RDA\Original\assets.xml",
        //@"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\assets.xml",
        //@"C:\Users\Andreas\Downloads\Anno 1800\Mod\data10\data\config\export\main\asset\assets.xml"
      };
      foreach (var file in files) {
        var document = XDocument.Load(file);
        var profile = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID=46]");
        // OfferingItems
        var progessions = profile.XPathSelectElements("Values/Trader/Progression/*");
        foreach (var maxItemsToOffer in progessions.Elements("MaxItemsToOffer")) {
          maxItemsToOffer.Value = "6";
        }
        foreach (var offeringItem in progessions.Elements("OfferingItems")) {
          var poolMain = document.Root.XPathSelectElement($"//Asset[Values/Standard/GUID={offeringItem.Value}]");
          // reduce to only one pool
          poolMain.XPathSelectElements("Values/RewardPool/ItemsPool/Item").Skip(1).Remove();
          poolMain.XPathSelectElement("Values/RewardPool/ItemsPool/Item/ItemLink").Value = "192905";
          poolMain.XPathSelectElement("Values/RewardPool/ItemsPool/Item/Weight").Value = "100";
          // sub pool
          var poolSub = document.Root.XPathSelectElement("//Asset[Values/Standard/GUID=192905]");
          poolSub.XPathSelectElements("Values/RewardPool/ItemsPool/Item").Skip(1).Remove();
          poolSub.XPathSelectElement("Values/RewardPool/ItemsPool/Item/ItemLink").Value = "193090";
          // product pool
          var poolProduct = document.Root.XPathSelectElement("//Asset[Values/Standard/GUID=193090]");
          poolProduct.XPathSelectElements("Values/RewardPool/ItemsPool/Item").Remove();
          // add own products
          var itemsPool = poolProduct.XPathSelectElement("Values/RewardPool/ItemsPool");
          {
            // Fisheries/Pearl Farm
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190622"))); // Captain Moby, Old Dog of the Sea
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190621"))); // Mother of Pearls
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190904"))); // Brixham Bottom Trawl
          }
          {
            // Crop Farms
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "193187"))); // Alex the Ramblers
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190752"))); // Cosimo Ridolfi - The Agronomics Inventor
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "192299"))); // Miraculous steel plough
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "192443"))); // Sir Lewis Brindley
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "191383"))); // Arthur Guiness - The Famous Beer Producer
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190896"))); // Electric Powered Pumps
          }
          {
            // Sheep Farms
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "191329"))); // Lucile - The Ladies' Designer
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "191327"))); // Tailor
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "191328"))); // Fashion Designer
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "192450"))); // Feras Alsarami
          }
          {
            // Spectacle Factory
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "191310"))); // Gerhard Fuchs, of the Patent Eyeglass
          }
          {
            // Electricity
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190237"))); // Angela "Meg" Iver, The Polyvalent
          }
          {
            // Expedition
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "192074"))); // Jack of all Traits
          }
          {
            // Ships
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "191624"))); // Ermenegilda Di Mercante, Purveyor of Tall Ships
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "191620"))); // Exporter of Goods
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "192091"))); // Geordy Duff
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "191626"))); // Hans Eichendorf, A Wholesale Success
          }
          {
            // Wood/Hunting
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190858"))); // 
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190857"))); // 
            itemsPool.Add(new XElement("Item", new XElement("ItemLink", "190739"))); // 
          }
        }
        //
        document.Save(@"D:\Sonstiges\Privat\GitHub\AssetViewer\RDA\Original\assets_copy.xml");
        var lines = File.ReadAllLines(@"D:\Sonstiges\Privat\GitHub\AssetViewer\RDA\Original\assets_copy.xml").Skip(1);
        File.WriteAllLines(@"D:\Sonstiges\Privat\GitHub\AssetViewer\RDA\Original\assets_copy.xml", lines);
      }
    }
    private static void ShipStackLimit() {
      var files = new[] {
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\assets.xml",
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data10\data\config\export\main\asset\assets.xml"
      };
      foreach (var file in files) {
        var document = XDocument.Load(file);
        // oil ship
        var asset = document.Root.XPathSelectElement("//Asset[Values/Standard/GUID=100853]");
        asset.XPathSelectElement("Values/ItemContainer/StackLimit").Value = "500";
        // schooner
        asset = document.Root.XPathSelectElement("//Asset[Values/Standard/GUID=100438]");
        asset.XPathSelectElement("Values/ItemContainer").Add(new XElement("StackLimit", "100"));
        //
        document.Save(file);
        var lines = File.ReadAllLines(file).Skip(1);
        File.WriteAllLines(file, lines);
      }
    }
    private static void HarborDepot() {
      var files = new[] {
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data0\data\config\export\main\asset\assets.xml",
        @"C:\Users\Andreas\Downloads\Anno 1800\Mod\data10\data\config\export\main\asset\assets.xml"
      };
      foreach (var file in files) {
        var document = XDocument.Load(file);
        // Oil Storage
        var asset = document.Root.XPathSelectElement("//Asset[Values/Standard/GUID=100784]");
        asset.XPathSelectElement("Values/Warehouse/WarehouseStorage/StorageMax").Value = "500";
        // Depot
        asset = document.Root.XPathSelectElement("//Asset[Values/Standard/GUID=1010519]");
        asset.XPathSelectElement("Values/Warehouse/WarehouseStorage/StorageMax").Value = "100";
        //
        document.Save(file);
        var lines = File.ReadAllLines(file).Skip(1);
        File.WriteAllLines(file, lines);
      }
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
      var assets = Program.Original.XPathSelectElements("//Asset[Template='Expedition']").ToList();
      assets.ForEach((asset) => {
        Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value);
        var item = new Expedition(asset);
        result.Add(item);
      });
      var document = new XDocument();
      document.Add(new XElement("Expeditions"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_Expeditions.xml");
    }
    #endregion

  }

}