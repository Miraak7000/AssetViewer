using AssetViewer.Data;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AssetViewer.Templates {

  public class TemplateThirdParty {

    #region Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public List<OfferingItems> OfferingItems { get; set; }

    #endregion Properties

    #region Constructors

    public TemplateThirdParty(XElement asset) {
      this.ID = asset.Attribute("ID").Value;
      this.Name = asset.Element("Name").Value;
      this.Text = new Description(asset.Element("Text"));
      this.OfferingItems = new List<OfferingItems>();
      foreach (var progression in asset.Element("OfferingItems").Elements()) {
        var item = new OfferingItems(progression);
        this.OfferingItems.Add(item);
      }
    }

    #endregion Constructors
  }
}