using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class SourceWithDetailsList : List<SourceWithDetails> {

    #region Public Properties

    public HashSet<XElement> FollowingEvents { get; set; } = new HashSet<XElement>();

    public float FollowingWeight { get; set; } = 1;
    public Dictionary<string, double> ElementWeights { get; set; } = new Dictionary<string, double>();

    #endregion Public Properties

    #region Public Constructors

    public SourceWithDetailsList() : base() {
    }

    #endregion Public Constructors

    #region Public Methods

    public SourceWithDetailsList Copy() {
      return new SourceWithDetailsList(this.Select(l => l.Copy())) { FollowingEvents = new HashSet<XElement>(this.FollowingEvents), ElementWeights = this.ElementWeights.ToDictionary(i => i.Key, i => i.Value) };
    }

    public void AddSourceAsset(XElement element, HashSet<AssetWithWeight> details = default) {
      var assetID = element.XPathSelectElement("Values/Standard/GUID").Value;
      var expeditionName = element.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value;
      var questGiver = element.XPathSelectElement("Values/Quest/QuestGiver")?.Value;

      //Expedition
      if (expeditionName != null) {
        var expedition = this.FirstOrDefault(w => w.Source.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value == expeditionName);
        if (expedition?.Source == null) {
          this.Add(new SourceWithDetails(element, details));
        }
        else {
          foreach (var item in details) {
            var itemID = item.Asset.XPathSelectElement("Values/Standard/GUID").Value;
            var itemTemplate = item.Asset.Element("Template").Value;

            if (expedition.Details.Any(i => i.Asset.XPathSelectElement("Values/Standard/GUID").Value == itemID && i.Asset.Element("Template").Value == itemTemplate)) {
              continue;
            }
            if (item.Asset.Element("Template").Value == "Expedition") {
              var subExpeditionName = item.Asset.XPathSelectElement("Values/Standard/Name")?.Value;
              if (expedition.Details.Any(i => i.Asset.XPathSelectElement("Values/Standard/Name")?.Value == subExpeditionName)) {
                continue;
              }
            }

            expedition.Details.Add(item);
          }
        }
        return;
      }

      var old = this.FirstOrDefault(l => l.Source.XPathSelectElement("Values/Standard/GUID").Value == assetID && l.Source.Element("Template").Value == element.Element("Template").Value);
      if (questGiver != null) {
        old = this.FirstOrDefault(w => w.Source.XPathSelectElement("Values/Quest/QuestGiver")?.Value == questGiver);
      }
      else if (element.Element("Template")?.Value == "Dive") {
        old = this.FirstOrDefault(w => w.Source.Element("Template")?.Value == "Dive");
      }

      //Tourism
      if (element.Element("Template")?.Value == "TourismFeature") {
        if (old?.Source != null) {
          foreach (var pool in details) {
            var poolTemplate = pool.Asset.Element("Template").Value;

            if (pool.Asset.Element("Item")?.Element("Pool")?.Value is string pooValue &&
              old.Details.FirstOrDefault(i => i.Asset.Element("Template").Value == poolTemplate && (i.Asset.Element("Item").Element("Pool").Value == pooValue)) is AssetWithWeight child) {
              child.Weight += pool.Weight;
            }
            else if (pool.Asset.XPathSelectElement("Values/Standard/GUID")?.Value is string guid &&
              old.Details.FirstOrDefault(i => i.Asset.XPathSelectElement("Values/Standard/GUID")?.Value == guid) is AssetWithWeight child2) {
              child2.Weight += pool.Weight;
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

      // Normal adding to old code
      if (old?.Source != null) {
        foreach (var item in details) {
          var itemID = item.Asset.XPathSelectElement("Values/Standard/GUID").Value;
          var itemTemplate = item.Asset.Element("Template").Value;
          if (old.Details.FirstOrDefault(i => i.Asset.XPathSelectElement("Values/Standard/GUID")?.Value == itemID && i.Asset.Element("Template")?.Value == itemTemplate) is AssetWithWeight child) {
            child.Weight += item.Weight;
          }
          else {
            old.Details.Add(item);
          }
        }
      }
      else {
        this.Add(new SourceWithDetails(element, details));
      }
    }

    public void AddSourceAsset(SourceWithDetailsList input) {
      foreach (var item in input) {
        this.AddSourceAsset(item.Source, item.Details);
      }
      foreach (var item in input.FollowingEvents) {
        this.FollowingEvents.Add(item);
      }
    }

    #endregion Public Methods

    #region Private Constructors

    private SourceWithDetailsList(IEnumerable<SourceWithDetails> collection) : base(collection) {
    }

    #endregion Private Constructors
  }
}