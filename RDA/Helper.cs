using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable IdentifierTypo
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable InconsistentNaming
// ReSharper disable PossibleNullReferenceException
namespace RDA {

  public static class Helper {

    #region Internal Methods
    internal static void SetImage(XElement element) {
      var name = element.Value.Replace(".png", "_0.png");
      var source = Path.GetFullPath(Path.Combine(Program.PathRoot, "Resources", name));
      if (File.Exists(source)) {
        var destination = Path.GetFullPath(Path.Combine(Program.PathViewer, "Resources", name));
        if (!Directory.Exists(Path.GetDirectoryName(destination))) Directory.CreateDirectory(Path.GetDirectoryName(destination));
        File.Copy(source, destination, true);
        element.Value = name;
      } else
        element.Value = String.Empty;
    }
    internal static void TemplateBuildPermitBuilding(XElement item) {
      item.XPathSelectElement("Values/Standard/Name").Remove();
      item.XPathSelectElement("Values/Blocking").Remove();
      item.XPathSelectElement("Values/Building").Remove();
      item.XPathSelectElement("Values/Cost").Remove();
      item.XPathSelectElement("Values/Selection").Remove();
      item.XPathSelectElement("Values/Object").Remove();
      item.XPathSelectElement("Values/Constructable").Remove();
      item.XPathSelectElement("Values/Mesh").Remove();
      item.XPathSelectElement("Values/SoundEmitter").Remove();
      item.XPathSelectElement("Values/Locked").Remove();
      item.XPathSelectElement("Values/FeedbackController").Remove();
      item.XPathSelectElement("Values/AmbientMoodProvider").Remove();
      item.XPathSelectElement("Values/Pausable").Remove();
      item.XPathSelectElement("Values/BuildPermit").Remove();
      // text
      var itemGuid = item.XPathSelectElement("Values/Standard/GUID").Value;
      var textEN = item.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
      var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
      item.XPathSelectElement("Values/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Text").Remove();
      // ornament
      var ornamentGuid = item.XPathSelectElement("Values/Ornament/OrnamentDescritpion").Value;
      textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={ornamentGuid}]/../Text/LocaText/English/Text").Value;
      textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={ornamentGuid}]/Text").Value;
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Long", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Long", textDE));
      item.XPathSelectElement("Values/Ornament").Remove();
      // image
      Helper.SetImage(item.XPathSelectElement("Values/Standard/IconFilename"));
    }
    internal static void TemplateGuildhouseItem(XElement item) {
      item.XPathSelectElement("Values/Standard/Name").Remove();
      item.XPathSelectElement("Values/Item/MaxStackSize").Remove();
      item.XPathSelectElement("Values/Item/TradePrice").Remove();
      item.XPathSelectElement("Values/Locked").Remove();
      item.XPathSelectElement("Values/Buff").Remove();
      item.XPathSelectElement("Values/ExpeditionAttribute/FluffText")?.Remove();
      // text
      var itemGuid = item.XPathSelectElement("Values/Standard/GUID").Value;
      var textEN = item.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
      var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
      item.XPathSelectElement("Values/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Text").Remove();
      // info
      var infoGuid = item.XPathSelectElement("Values/Standard/InfoDescription").Value;
      textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={infoGuid}]/../Text/LocaText/English/Text").Value;
      textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={infoGuid}]/Text").Value;
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Long", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Long", textDE));
      item.XPathSelectElement("Values/Standard/InfoDescription").Remove();
      // EffectTargets
      foreach (var effectTarget in item.XPathSelectElements("Values/ItemEffect/EffectTargets/Item")) {
        var effectTargetGuid = effectTarget.Element("GUID").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={effectTargetGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={effectTargetGuid}]/Text").Value;
        effectTarget.Add(new XElement("Description"));
        effectTarget.XPathSelectElement("Description").Add(new XElement("EN"));
        effectTarget.XPathSelectElement("Description").Add(new XElement("DE"));
        effectTarget.XPathSelectElement("Description/EN").Add(new XElement("Short", textEN));
        effectTarget.XPathSelectElement("Description/DE").Add(new XElement("Short", textDE));
      }
      // image
      Helper.SetImage(item.XPathSelectElement("Values/Standard/IconFilename"));
    }
    internal static void TemplateCultureItem(XElement item) {
      item.XPathSelectElement("Values/Standard/Name").Remove();
      item.XPathSelectElement("Values/Item/TradePrice").Remove();
      item.XPathSelectElement("Values/Cost").Remove();
      item.XPathSelectElement("Values/Locked").Remove();
      item.XPathSelectElement("Values/ExpeditionAttribute/FluffText")?.Remove();
      item.XPathSelectElement("Values/CultureUpgrade/ChangeModule")?.Remove();
      // text
      var itemGuid = item.XPathSelectElement("Values/Standard/GUID").Value;
      var textEN = item.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
      var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
      item.XPathSelectElement("Values/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Text").Remove();
      // info
      var infoGuid = item.XPathSelectElement("Values/Standard/InfoDescription").Value;
      textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={infoGuid}]/../Text/LocaText/English/Text").Value;
      textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={infoGuid}]/Text").Value;
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Long", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Long", textDE));
      item.XPathSelectElement("Values/Standard/InfoDescription").Remove();
      // image
      Helper.SetImage(item.XPathSelectElement("Values/Standard/IconFilename"));
    }
    internal static void TemplateHarborOfficeItem(XElement item) {
      item.XPathSelectElement("Values/Standard/Name").Remove();
      item.XPathSelectElement("Values/Item/MaxStackSize").Remove();
      item.XPathSelectElement("Values/Item/HasAction")?.Remove();
      item.XPathSelectElement("Values/Item/TradePrice").Remove();
      item.XPathSelectElement("Values/Locked").Remove();
      item.XPathSelectElement("Values/ExpeditionAttribute/FluffText")?.Remove();
      // text
      var itemGuid = item.XPathSelectElement("Values/Standard/GUID").Value;
      var textEN = item.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
      var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
      item.XPathSelectElement("Values/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Text").Remove();
      // info
      var infoGuid = item.XPathSelectElement("Values/Standard/InfoDescription").Value;
      textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={infoGuid}]/../Text/LocaText/English/Text").Value;
      textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={infoGuid}]/Text").Value;
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Long", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Long", textDE));
      item.XPathSelectElement("Values/Standard/InfoDescription").Remove();
      // EffectTargets
      foreach (var effectTarget in item.XPathSelectElements("Values/ItemEffect/EffectTargets/Item")) {
        var effectTargetGuid = effectTarget.Element("GUID").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={effectTargetGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={effectTargetGuid}]/Text").Value;
        effectTarget.Add(new XElement("Description"));
        effectTarget.XPathSelectElement("Description").Add(new XElement("EN"));
        effectTarget.XPathSelectElement("Description").Add(new XElement("DE"));
        effectTarget.XPathSelectElement("Description/EN").Add(new XElement("Short", textEN));
        effectTarget.XPathSelectElement("Description/DE").Add(new XElement("Short", textDE));
      }
      // image
      Helper.SetImage(item.XPathSelectElement("Values/Standard/IconFilename"));
    }
    internal static void TemplateVehicleItem(XElement item) {
      item.XPathSelectElement("Values/Standard/Name").Remove();
      item.XPathSelectElement("Values/Item/MaxStackSize").Remove();
      item.XPathSelectElement("Values/Item/HasAction")?.Remove();
      item.XPathSelectElement("Values/Item/TradePrice").Remove();
      item.XPathSelectElement("Values/Locked").Remove();
      item.XPathSelectElement("Values/ExpeditionAttribute/FluffText")?.Remove();
      item.XPathSelectElement("Values/Cost")?.Remove();
      // text
      var itemGuid = item.XPathSelectElement("Values/Standard/GUID").Value;
      var textEN = item.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
      var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
      item.XPathSelectElement("Values/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Text").Remove();
      // info
      var infoGuid = item.XPathSelectElement("Values/Standard/InfoDescription").Value;
      textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={infoGuid}]/../Text/LocaText/English/Text").Value;
      textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={infoGuid}]/Text").Value;
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Long", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Long", textDE));
      item.XPathSelectElement("Values/Standard/InfoDescription").Remove();
      // image
      Helper.SetImage(item.XPathSelectElement("Values/Standard/IconFilename"));
    }
    internal static void TemplateActiveItem(XElement item) {
      item.XPathSelectElement("Values/Standard/Name").Remove();
      item.XPathSelectElement("Values/Item/MaxStackSize").Remove();
      item.XPathSelectElement("Values/Item/HasAction")?.Remove();
      item.XPathSelectElement("Values/Item/TradePrice").Remove();
      item.XPathSelectElement("Values/Locked").Remove();
      item.XPathSelectElement("Values/ExpeditionAttribute/FluffText")?.Remove();
      item.XPathSelectElement("Values/Cost")?.Remove();
      // text
      var itemGuid = item.XPathSelectElement("Values/Standard/GUID").Value;
      var textEN = item.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
      var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={itemGuid}]/Text").Value;
      item.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
      item.XPathSelectElement("Values/Description").Add(new XElement("EN"));
      item.XPathSelectElement("Values/Description").Add(new XElement("DE"));
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
      item.XPathSelectElement("Values/Text").Remove();
      // info
      var infoGuid = item.XPathSelectElement("Values/Standard/InfoDescription").Value;
      textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={infoGuid}]/../Text/LocaText/English/Text").Value;
      textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={infoGuid}]/Text").Value;
      item.XPathSelectElement("Values/Description/EN").Add(new XElement("Long", textEN));
      item.XPathSelectElement("Values/Description/DE").Add(new XElement("Long", textDE));
      item.XPathSelectElement("Values/Standard/InfoDescription").Remove();
      // image
      Helper.SetImage(item.XPathSelectElement("Values/Standard/IconFilename"));
    }
    #endregion

  }

}