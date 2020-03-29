using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {
  public class ThirdParty {
    #region Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public List<OfferingItems> OfferingItems { get; set; }

    #endregion Properties

    #region Constructors

    public ThirdParty(XElement asset) {
      this.ID = asset.XPathSelectElement("Values/Standard/GUID").Value;
      this.Name = asset.XPathSelectElement("Values/Standard/Name").Value;
      this.Text = new Description(this.ID);
      this.OfferingItems = new List<OfferingItems>();
      var progressions = asset.XPathSelectElements("Values/Trader/Progression/*");
      //Hugo
      if (ID == "220") {
        progressions = asset.XPathSelectElements("Values/ConstructionAI/ItemTradeConfig/ItemPools")?.Elements();
      }
      foreach (var progression in progressions) {
        var item = new OfferingItems(progression);
        this.OfferingItems.Add(item);
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement("TP");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XAttribute("N", this.Name));
      result.Add(this.Text.ToXml("T"));
      result.Add(new XElement("OI", this.OfferingItems.Select(s => s.ToXml())));
      return result;
    }

    #endregion Methods
  }
}