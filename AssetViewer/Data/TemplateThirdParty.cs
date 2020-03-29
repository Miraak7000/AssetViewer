using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class TemplateThirdParty {

    #region Public Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public List<OfferingItems> OfferingItems { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public TemplateThirdParty(XElement asset) {
      this.ID = asset.Attribute("ID").Value;
      this.Name = asset.Attribute("N").Value;
      this.Text = new Description(asset.Element("T"));
      this.OfferingItems = new List<OfferingItems>();
      foreach (var progression in asset.Element("OI").Elements()) {
        var item = new OfferingItems(progression);
        this.OfferingItems.Add(item);
      }
    }

    #endregion Public Constructors
  }
}