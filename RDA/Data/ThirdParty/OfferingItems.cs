using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class OfferingItems {

    #region Public Properties

    public Progression Progression { get; set; }
    public Description ProgressionDescription { get; set; }
    public string ItemPool { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public OfferingItems(XElement asset, GameTypes gameType) {
      Progression = (Progression)Enum.Parse(typeof(Progression), asset.Name.LocalName);
      ProgressionDescription = TempSource.GetDescriptionFromProgression(asset.Name.LocalName, gameType);
      var offeringItems = asset.XPathSelectElement("OfferingItems")?.Value ?? asset.XPathSelectElement("Pool")?.Value;
      if (offeringItems != null) {
        ItemPool = offeringItems;
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("OI");
      result.Add(new XAttribute("P", Progression));
      result.Add(ProgressionDescription.ToXml("PD"));
      if (ItemPool != null) {
        result.Add(new XAttribute("I", ItemPool));
      }
      return result;
    }

    #endregion Public Methods
  }
}