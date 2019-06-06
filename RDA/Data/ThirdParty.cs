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
    public List<OfferingItems> OfferingItems { get; set; }
    #endregion

    #region Constructor
    public ThirdParty(XElement asset) {
      this.ID = asset.XPathSelectElement("Values/Standard/GUID").Value;
      this.Name = asset.XPathSelectElement("Values/Standard/Name").Value;
      this.Icon = new Icon(asset.XPathSelectElement("Values/Standard/IconFilename").Value);
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

  }

}