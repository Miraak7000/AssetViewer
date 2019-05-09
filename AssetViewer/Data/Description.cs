using System;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Description {

    #region Properties
    public String ID { get; set; }
    public String EN { get; set; }
    public String DE { get; set; }
    #endregion

    #region Constructor
    public Description(XElement item) {
      this.ID = item.Attribute("ID")?.Value;
      this.EN = item.Element("EN")?.Value;
      this.DE = item.Element("DE")?.Value;
    }
    public Description(String en, String de) {
      this.ID = String.Empty;
      this.EN = en;
      this.DE = de;
    }
    #endregion

  }

}