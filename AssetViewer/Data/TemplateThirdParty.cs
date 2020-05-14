using System.Collections.Generic;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class TemplateThirdParty {

    #region Public Properties

    public string ID { get; set; }
    public string Name { get; set; }
    public Description Text { get; set; }
    public List<OfferingItems> OfferingItems { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public TemplateThirdParty(XElement asset) {
      ID = asset.Attribute("ID").Value;
      Name = asset.Attribute("N").Value;
      Text = new Description(asset.Element("T"));
      OfferingItems = new List<OfferingItems>();
      foreach (var progression in asset.Element("OI").Elements()) {
        var item = new OfferingItems(progression);
        OfferingItems.Add(item);
      }
    }

    #endregion Public Constructors
  }
}