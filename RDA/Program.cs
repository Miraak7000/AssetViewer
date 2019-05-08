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
      //Helper.ExtractTextEnglish(Program.PathRoot + @"\Original\assets.xml");
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
      Program.ProcessingItems("HarborOfficeItem");
      Program.ProcessingThirdParty();
    }
    private static void ProcessingItems(String template) {
      var result = new List<Asset>();
      //var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}']").ToArray();
      var assets = Program.Original.XPathSelectElements($"//Asset[Template='{template}' and Values/Standard/GUID=191886]").Take(20).ToArray();
      foreach (var asset in assets) {
        if (asset.XPathSelectElement("Values/Item/HasAction")?.Value == "1") continue;
        Console.WriteLine(asset.XPathSelectElement("Values/Standard/GUID").Value);
        var item = new Asset(asset);
        result.Add(item);
      }
      var document = new XDocument();
      document.Add(new XElement(template));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_{template}.xml");
      document.Save($@"{Program.PathViewer}\Resources\Assets\{template}.xml");
    }
    private static void ProcessingThirdParty() {
      var result = new List<ThirdParty>();
      var assets = Program.Original.XPathSelectElements($"//Asset[Template='Profile_3rdParty']").ToArray();
      foreach (var asset in assets) {
        if (!asset.XPathSelectElements("Values/Trader/Progression/*/OfferingItems").Any()) continue;
        var item = new ThirdParty(asset);
        result.Add(item);
      }
      var document = new XDocument();
      document.Add(new XElement("ThirdParties"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_ThirdParty.xml");
      document.Save($@"{Program.PathViewer}\Resources\Assets\ThirdParty.xml");
    }
    private static IEnumerable<Asset> GetItems(String id) {
      var result = new List<Asset>();
      var asset = Program.Original.XPathSelectElement($"//Asset[Values/Standard/GUID={id}]");
      var template = asset.Element("Template").Value;
      switch (template) {
        case "RewardItemPool":
        case "RewardPool":
          var links = asset.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").ToArray();
          foreach (var link in links) {
            result.AddRange(Program.GetItems(link.Value));
          }
          break;
        case "ActiveItem":
        case "ItemSpecialAction":
        case "VehicleItem":
          // TODO: needs to be implemented first
          break;
        case "GuildhouseItem":
        case "HarborOfficeItem":
          var item = new Asset(asset);
          if (result.All(w => w.ID != item.ID)) result.Add(item);
          break;
        default:
          throw new NotImplementedException(template);
      }
      return result;
    }
    #endregion

  }

}