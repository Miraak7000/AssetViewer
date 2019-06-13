using System;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Allocation {

    #region Properties
    public String ID { get; set; }
    public Description Text { get; set; }
    #endregion

    #region Constructor
    public Allocation(XElement item) {
      this.ID = item.Attribute("ID").Value;
      this.Text = new Description(item.Element("Text"));
    }
    #endregion

  }

}