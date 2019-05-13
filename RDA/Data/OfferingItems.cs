using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Templates;

namespace RDA.Data {

  public class OfferingItems {

    #region Properties
    public Progression Progression { get; set; }
    public List<Asset> Assets { get; set; }
    #endregion

    #region Constructor
    public OfferingItems(XElement asset) {
      this.Progression = (Progression)Enum.Parse(typeof(Progression), asset.Name.LocalName);
      this.Assets = new List<Asset>();
      var offeringItems = asset.XPathSelectElement("OfferingItems")?.Value;
      if (offeringItems != null) {
        this.FindOfferingItems(offeringItems, String.Empty);
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("Progression", this.Progression));
      result.Add(new XElement("Assets", this.Assets.Select(s => s.ToXml())));
      return result;
    }
    #endregion

    #region Private Methods
    private void FindOfferingItems(String id, String path) {
      path += $"/{id}";
      var asset = Program.Original.XPathSelectElement($"//Asset[Values/Standard/GUID={id}]");
      var template = asset.Element("Template").Value;
      switch (template) {
        case "RewardPool":
        case "RewardItemPool":
          var links = asset.XPathSelectElements("Values/RewardPool/ItemsPool/Item").ToArray();
          foreach (var link in links) {
            var weight = link.Element("Weight")?.Value;
            if (weight == null || Int32.Parse(weight) > 0) {
              this.FindOfferingItems(link.Element("ItemLink").Value, path);
            }
          }
          break;
        case "ActiveItem":
        case "ItemSpecialAction":
        case "ItemSpecialActionVisualEffect":
          // TODO: needs to be implemented first
          break;
        case "GuildhouseItem":
        case "HarborOfficeItem":
        case "TownhallItem":
        case "VehicleItem":
        case "ShipSpecialist":
        case "CultureItem":
          var item = new Asset(asset, false);
          item.Path = path;
          if (this.Assets.All(w => w.ID != item.ID)) this.Assets.Add(item);
          break;
        default:
          throw new NotImplementedException(template);
      }
    }
    #endregion
  }

}