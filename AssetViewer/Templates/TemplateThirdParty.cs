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
    public List<Description> Progressions {
      get {
        var result = new List<Description>();
        result.Add(new Description("Early Game", "Frühes Spiel"));
        result.Add(new Description("Early-Mid Game", "Frühes-Mittleres Spiel"));
        result.Add(new Description("Mid Game", "Mittleres Spiel"));
        result.Add(new Description("Late-Mid Game", "Mittleres-Spätes Spiel"));
        result.Add(new Description("Late Game", "Spätes Spiel"));
        result.Add(new Description("End Game", "Endspiel"));
        return result;
      }
    }
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