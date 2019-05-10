using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Templates;

namespace RDA.Data {

  public class ThirdParty {

    #region Properties
    public String ID { get; set; }
    public String Name { get; set; }
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public List<Asset> OfferingItems { get; set; }
    #endregion

    #region Constructor
    public ThirdParty(XElement asset) {
      this.ID = asset.XPathSelectElement("Values/Standard/GUID").Value;
      this.Name = asset.XPathSelectElement("Values/Standard/Name").Value;
      this.Icon = new Icon(asset.XPathSelectElement("Values/Standard/IconFilename").Value);
      this.Text = new Description(this.ID);
      this.OfferingItems = new List<Asset>();
      var progression = asset.XPathSelectElement("Values/Trader/Progression").Elements();
      foreach (var game in progression) {
        this.FindOfferingItems(game.Element("OfferingItems").Value, String.Empty);
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("Name", this.Name));
      result.Add(this.Icon.ToXml());
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("OfferingItems", this.OfferingItems.Select(s => s.ToXml())));
      return result;
    }
    #endregion

    #region Private Methods
    private void FindOfferingItems(String id, String path) {
      path += $"/{id}";
      var asset = Program.Original.XPathSelectElement($"//Asset[Values/Standard/GUID={id}]");
      var template = asset.Element("Template").Value;
      switch (template) {
        case "RewardItemPool":
        case "RewardPool":
          var links = asset.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").ToArray();
          foreach (var link in links) {
            this.FindOfferingItems(link.Value, path);
          }
          break;
        case "ActiveItem":
        case "ItemSpecialAction":
        case "ItemSpecialActionVisualEffect":
        case "CultureItem":
          // TODO: needs to be implemented first
          break;
        case "GuildhouseItem":
        case "HarborOfficeItem":
        case "TownhallItem":
        case "VehicleItem":
        case "ShipSpecialist":
          var item = new Asset(asset, false);
          item.Path = path;
          if (this.OfferingItems.All(w => w.ID != item.ID)) this.OfferingItems.Add(item);
          break;
        default:
          throw new NotImplementedException(template);
      }
    }
    #endregion

  }

}