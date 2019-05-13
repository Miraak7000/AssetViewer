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
    internal static XDocument Sources;
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
      Program.Sources = XDocument.Load(Program.PathRoot + @"\Modified\Assets_Sources.xml");

      // Helper
      //Helper.ExtractTextEnglish(Program.PathRoot + @"\Original\texts_english.xml");
      //Helper.ExtractTextGerman(Program.PathRoot + @"\Original\texts_german.xml");
      //Helper.ExtractTemplateNames(Program.PathRoot + @"\Original\assets.xml");

      // Descriptions
      Program.DescriptionEN = XDocument.Load(Program.PathRoot + @"\Modified\Texts_English.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);
      Program.DescriptionDE = XDocument.Load(Program.PathRoot + @"\Modified\Texts_German.xml").Root.Elements().ToDictionary(k => k.Attribute("ID").Value, e => e.Value);

      // World Fair
      //Monument.Create();

      // Create Assets
      //Program.ProcessingItems("GuildhouseItem");
      //Program.ProcessingItems("TownhallItem");
      //Program.ProcessingItems("HarborOfficeItem");
      //Program.ProcessingItems("VehicleItem");
      //Program.ProcessingItems("ShipSpecialist");
      //Program.ProcessingItems("CultureItem");
      Program.ProcessingThirdParty();
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
    //  document.Save(@"C:\Users\Andreas\Downloads\Anno 1800\Schiff-Zoom-Einfluss-Mod\data0\data\config\export\main\asset\assets.xml");
    //}
    #endregion

  }

}