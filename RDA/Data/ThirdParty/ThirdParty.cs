using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class ThirdParty {

    #region Public Properties

    public string ID { get; set; }
    public string Name { get; set; }
    public Description Text { get; set; }
    public List<OfferingItems> OfferingItems { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public ThirdParty(XElement asset) {
      ID = asset.XPathSelectElement("Values/Standard/AVGUID")?.Value ?? asset.XPathSelectElement("Values/Standard/GUID").Value;
      Name = asset.XPathSelectElement("Values/Standard/Name").Value;
      Text = new Description(asset);
      OfferingItems = new List<OfferingItems>();
      Enum.TryParse<GameTypes>(asset.Attribute("GameType").Value, out var gameType);
      var progressions = asset.XPathSelectElements("Values/Trader/Progression/*");
      //Hugo
      if (ID == "220") {
        progressions = asset.XPathSelectElements("Values/ConstructionAI/ItemTradeConfig/ItemPools")?.Elements();
      }
      foreach (var progression in progressions) {
        var item = new OfferingItems(progression, gameType);
        OfferingItems.Add(item);
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("TP");
      result.Add(new XAttribute("ID", ID));
      result.Add(new XAttribute("N", Name));
      result.Add(Text.ToXml("T"));
      result.Add(new XElement("OI", OfferingItems.Select(s => s.ToXml())));
      return result;
    }

    #endregion Public Methods
  }
}