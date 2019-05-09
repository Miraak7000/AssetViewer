using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Library {

  internal static class Extensions {

    #region Private Methods
    public static void AddSourceAsset(this List<XElement> source, XElement element) {
      var assetID = element.XPathSelectElement("Values/Standard/GUID").Value;
      var expeditionName = element.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value;
      var questGiver = element.XPathSelectElement("Values/Quest/QuestGiver")?.Value;
      if (!source.Where(w => w.XPathSelectElement("Values/Standard/GUID").Value == assetID).Any()) {
        if (expeditionName != null) {
          if (!source.Where(w => w.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value == expeditionName).Any()) {
            source.Add(element);
          }
        } else if (questGiver != null) {
          if (!source.Where(w => w.XPathSelectElement("Values/Quest/QuestGiver")?.Value == questGiver).Any()) {
            source.Add(element);
          }
        } else {
          source.Add(element);
        }
      }
    }
    #endregion

  }

}