using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Templates {

  public class SourceWithDetailsList : List<SourceWithDetails> {

    #region Constructors

    public SourceWithDetailsList() : base() {
    }

    #endregion Constructors

    #region Methods

    public SourceWithDetailsList Copy() {
      return new SourceWithDetailsList(this.Select(l => l.Copy()));
    }

    public void AddSourceAsset(XElement element, HashSet<XElement> details = default) {
      var assetID = element.XPathSelectElement("Values/Standard/GUID").Value;
      var expeditionName = element.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value;
      var questGiver = element.XPathSelectElement("Values/Quest/QuestGiver")?.Value;

      //Expedition
      if (expeditionName != null) {
        var expedition = this.Find(w => w.Source.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value == expeditionName);
        if (expedition.Source == null) {
          this.Add(new SourceWithDetails(element, details));
        }
        else {
          foreach (var item in details) {
            var itemID = item.XPathSelectElement("Values/Standard/GUID").Value;
            var itemTemplate = item.Element("Template").Value;

            if (expedition.Details.Any(i => i.XPathSelectElement("Values/Standard/GUID").Value == itemID && i.Element("Template").Value == itemTemplate)) {
              continue;
            }
            if (item.Element("Template").Value == "Expedition") {
              var subExpeditionName = item.XPathSelectElement("Values/Standard/Name")?.Value;
              if (expedition.Details.Any(i => i.XPathSelectElement("Values/Standard/Name")?.Value == subExpeditionName)) {
                continue;
              }
            }

            expedition.Details.Add(item);
          }
        }
        return;
      }

      //Quest
      else if (questGiver != null) {
        var quest = this.Find(w => w.Source.XPathSelectElement("Values/Quest/QuestGiver")?.Value == questGiver);
        if (quest.Source == null) {
          this.Add(new SourceWithDetails(element, details));
        }
        else {
          foreach (var item in details) {
            quest.Details.Add(item);
          }
        }
        return;
      }

      var old = this.Find(l => l.Source.XPathSelectElement("Values/Standard/GUID").Value == assetID && l.Source.Element("Template").Value == element.Element("Template").Value);

      //Tourism
      if (element.Element("Template")?.Value == "TourismFeature") {
        if (old.Source != null) {
          foreach (var pool in details) {
            var poolTemplate = pool.Element("Template").Value;
            if (old.Details.Any(e =>
            e.Element("Template").Value == poolTemplate &&
            e.Element("Item").Element("Pool").Value == pool.Element("Item").Element("Pool").Value)) {
              return;
            }
            else {
              old.Details.Add(pool);
            }
          }
        }
        else {
          this.Add(new SourceWithDetails(element, details));
        }
        return;
      }

      //Special Diving exception
      if (element.Element("Template")?.Value == "Dive" || element.Element("Template")?.Value == "Pickup" || element.Element("Template")?.Value == "Crafting") {
        old = this.Find(l => l.Source.Element("Template").Value == element.Element("Template").Value);
        if (old.Source != null) {
          return;
        }
      }

      // Normal adding to old code
      if (old.Source != null) {
        foreach (var item in details) {
          var asset = item.Element("Asset");
          if (asset != null) {
            var itemID = item.XPathSelectElement("Values/Standard/GUID").Value;
            var itemTemplate = item.Element("Template").Value;
            if (old.Details.Any(i => i.XPathSelectElement("Values/Standard/GUID")?.Value == itemID && i.Element("Template")?.Value == itemTemplate)) {
              continue;
            }
            if (old.Details.Any(i => i == item)) {
              continue;
            }
          }
          old.Details.Add(item);
        }
      }
      else {
        this.Add(new SourceWithDetails(element, details));
      }
    }

    public void AddSourceAsset(SourceWithDetailsList input/*, Details details = null*/) {
      foreach (var item in input) {
        this.AddSourceAsset(item.Source, /*details?.Items ??*/ item.Details);
      }
    }

    #endregion Methods

    private SourceWithDetailsList(IEnumerable<SourceWithDetails> collection) : base(collection) {
    }
  }
}