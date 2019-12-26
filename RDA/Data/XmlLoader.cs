using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RDA {

  public static class XmlLoader {

    #region Methods

    public static XElement LoadXml(string path) {
      if (File.Exists(path)) {
        var root = XDocument.Load(path).Root;
        var files = new DirectoryInfo(Path.GetDirectoryName(path));
        if (files.EnumerateFiles().FirstOrDefault(f => f.Name == "templates.xml") is FileInfo info) {
          var templates = XDocument.Load(info.FullName).Root.Descendants("Template").ToLookup(t => t.Element("Name")?.Value ?? "trash");
          foreach (var asset in root.Descendants("Asset")) {
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
        for (int i = 0; i < groupArray.Length; i++) {
          var property = groupArray[i];
          if (asset.Elements(group.Key).Count() > i && asset.Elements(property.Name.LocalName).ElementAt(i) is XElement current) {
            if (property.HasElements) {
              AddStandardValues(current, property);
            }
          }
          else {
            asset.Add(property);
          }
        }
      }
    }

    #endregion Methods
  }
}