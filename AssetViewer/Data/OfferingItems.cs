using System;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class OfferingItems {

    #region Properties

    public Progression Progression { get; set; }
    public string Items { get; set; }

    #endregion Properties

    #region Constructors

    public OfferingItems(XElement asset) {
      this.Progression = (Progression)Enum.Parse(typeof(Progression), asset.Attribute("Progression").Value);
      this.Items = asset.Attribute("Items")?.Value;
    }

    #endregion Constructors
  }
}