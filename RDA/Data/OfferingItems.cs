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
    public string ItemPool { get; set; }
    #endregion

    #region Constructor
    public OfferingItems(XElement asset) {
      this.Progression = (Progression)Enum.Parse(typeof(Progression), asset.Name.LocalName);
      var offeringItems = asset.XPathSelectElement("OfferingItems")?.Value;
      if (offeringItems != null) {
        ItemPool = offeringItems;
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("Progression", this.Progression));
      if (ItemPool != null) {
        result.Add(new XAttribute("Items", ItemPool));
      }
      return result;
    }
    #endregion
  }

}