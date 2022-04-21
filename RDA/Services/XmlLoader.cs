using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;

namespace RDA.Services {

  public static class XmlLoader {

    #region Public Methods

    public static XElement LoadXml(string path, GameTypes gameType = GameTypes.Anno_1800) {
      if (File.Exists(path)) {
        var root = XDocument.Load(path).Root;
        var files = new DirectoryInfo(Path.GetDirectoryName(path));
        if (files.EnumerateFiles().FirstOrDefault(f => f.Name == "templates.xml") is FileInfo info) {
          var templates = XDocument.Load(info.FullName).Root.Descendants("Template").ToLookup(t => t.Element("Name")?.Value ?? "trash");
          foreach (var asset in root.Descendants("Asset")) {
            asset.Add(new XAttribute("GameType", (int)gameType));
            if (templates.Contains(asset.Element("Template")?.Value ?? "")) {
              var standards = templates[asset.Element("Template").Value];
              foreach (var standard in standards) {
                asset.Element("Values").AddStandardValues(standard.Element("Properties"));
              }
            }
          }
        }
        return root;
      }
      return null;
    }

    public static XElement LoadSzenarioXml(string path, GameTypes gameType = GameTypes.Anno_1800) {
      if (File.Exists(path)) {
        var root = XDocument.Load(path).Root;
        var files = new DirectoryInfo(Path.GetDirectoryName(path));
        if (files.EnumerateFiles().FirstOrDefault(f => f.Name == "templates.xml") is FileInfo info) {
          var templates = XDocument.Load(info.FullName).Root.Descendants("Template").ToLookup(t => t.Element("Name")?.Value ?? "trash");
          foreach (var asset in root.Descendants("Asset").ToArray()) {
            if (asset.XPathSelectElement("Values/Standard/GUID")?.Value == null) {
              if (asset.XPathSelectElement("ScenarioBaseAssetGUID")?.Value != null) {
                asset.XPathSelectElement("Values/Standard").Add(new XElement("AVGUID", $"{asset.XPathSelectElement("ScenarioBaseAssetGUID").Value} ({gameType.ToString()})"));
              }
            }
            asset.Add(new XAttribute("GameType", (int)gameType));
            if (templates.Contains(asset.Element("Template")?.Value ?? "")) {
              var standards = templates[asset.Element("Template").Value];
              foreach (var standard in standards) {
                asset.Element("Values").AddStandardValues(standard.Element("Properties"));
              }
            }
          }
        }
        return root;
      }
      return null;
    }

    public static void AddStandardValues(this XElement asset, XElement standarts) {
      foreach (var group in standarts.Elements().GroupBy(e => e.Name.LocalName)) {
        var groupArray = group.ToArray();
        for (var i = 0; i < groupArray.Length; i++) {
          var property = groupArray[i];
          if (asset.Elements(group.Key).Count() > i && asset.Elements(property.Name.LocalName).ElementAt(i) is XElement current) {
            if (property.HasElements) {
              current.AddStandardValues(property);
            }
          }
          else {
            asset.Add(property);
          }
        }
      }
    }

    #endregion Public Methods
  }
}