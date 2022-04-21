using System;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class OfferingItems {

    #region Public Properties

    public Progression Progression { get; set; }
    public Description ProgressionDescription { get; set; }
    public string Items { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public OfferingItems(XElement asset) {
      Progression = (Progression)Enum.Parse(typeof(Progression), asset.Attribute("P").Value);
      ProgressionDescription = new Description(asset.Element("PD"));
      if (asset.Attribute("I")?.Value is string str) {
        Items = str;
      }
    }

    #endregion Public Constructors
  }
}