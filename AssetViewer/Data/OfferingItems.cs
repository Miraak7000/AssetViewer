using System;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class OfferingItems {

    #region Properties

    public Progression Progression { get; set; }
    public int Items { get; set; }

    #endregion Properties

    #region Constructors

    public OfferingItems(XElement asset) {
      this.Progression = (Progression)Enum.Parse(typeof(Progression), asset.Attribute("Progression").Value);
      if (asset.Attribute("Items")?.Value is string str) {
        this.Items = int.Parse(str);
      }
    }

    #endregion Constructors
  }
}