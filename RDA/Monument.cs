using RDA.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable CoVariantArrayConversion
namespace RDA {
  /// <summary>
  ///   World Fair
  /// </summary>
  public static class Monument {
    #region Methods

    public static void Create() {
      Monument.MonumentEventCategory();
      Monument.MonumentEvent();
      Monument.MonumentThreshold();
    }

    private static void MonumentEventCategory() {
      var result = new List<Asset>();
      var monumentCategories = Assets.Original.XPathSelectElements("//Asset[Template='MonumentEventCategory']").ToArray();
      Program.ConsoleWriteHeadline("Create Monument");
      foreach (var monumentCategory in monumentCategories) {
        Program.ConsoleWriteGUID(monumentCategory.XPathSelectElement("Values/Standard/GUID").Value);
        var asset = new Asset(monumentCategory, false);
        result.Add(asset);
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentCategory"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_MonumentCategory.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\MonumentCategory.xml");
    }

    private static void MonumentEvent() {
      var result = new List<Asset>();
      var monumentEvents = Assets.Original.XPathSelectElements("//Asset[Template='MonumentEvent']").ToArray();
      foreach (var monumentEvent in monumentEvents) {
        Program.ConsoleWriteGUID(monumentEvent.XPathSelectElement("Values/Standard/GUID").Value);
        var asset = new Asset(monumentEvent, false);
        result.Add(asset);
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentEvent"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_MonumentEvent.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\MonumentEvent.xml");
    }

    private static void MonumentThreshold() {
      var result = new List<Asset>();
      var monumentEvents = Assets.Original.XPathSelectElements("//Asset[Template='MonumentEventReward']").ToArray();
      foreach (var monumentEvent in monumentEvents) {
        Program.ConsoleWriteGUID(monumentEvent.XPathSelectElement("Values/Standard/GUID").Value);
        var asset = new Asset(monumentEvent, false);
        result.Add(asset);
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentEvent"));
      document.Root.Add(result.Select(s => s.ToXml()));
      document.Save($@"{Program.PathRoot}\Modified\Assets_MonumentThreshold.xml");
      document.SaveIndent($@"{Program.PathViewer}\Resources\Assets\MonumentThreshold.xml");
    }

    #endregion Methods
  }
}