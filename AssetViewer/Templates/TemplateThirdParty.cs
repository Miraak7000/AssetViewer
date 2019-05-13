using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Data;

namespace AssetViewer.Templates {

  public class TemplateThirdParty {

    #region Properties
    public String ID { get; set; }
    public String Name { get; set; }
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public List<OfferingItems> OfferingItems { get; set; }
    #endregion

    #region Constructor
    public TemplateThirdParty(XElement asset) {
      this.ID = asset.Attribute("ID").Value;
      this.Name = asset.Element("Name").Value;
      this.Icon = new Icon(asset.Element("Icon"));
      this.Text = new Description(asset.Element("Text"));
      this.OfferingItems = new List<OfferingItems>();
      foreach (var progression in asset.Element("OfferingItems").Elements()) {
        var item = new OfferingItems(progression);
        this.OfferingItems.Add(item);
      }
    }
    #endregion

  }

}