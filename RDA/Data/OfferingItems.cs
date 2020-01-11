using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {
  public class OfferingItems {
    #region Properties

    public Progression Progression { get; set; }
    public string ItemPool { get; set; }

    #endregion Properties

    #region Constructors

    public OfferingItems(XElement asset) {
      this.Progression = (Progression)Enum.Parse(typeof(Progression), asset.Name.LocalName);
      var offeringItems = asset.XPathSelectElement("OfferingItems")?.Value ?? asset.XPathSelectElement("Pool")?.Value;
      if (offeringItems != null) {
        ItemPool = offeringItems;
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("Progression", this.Progression));
      if (ItemPool != null) {
        result.Add(new XAttribute("Items", ItemPool));
      }
      return result;
    }

    #endregion Methods
  }
}