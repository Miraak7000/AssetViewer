using System;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class Description {

    #region Properties
    [XmlAttribute]
    public String ID { get; set; }
    public String EN { get; set; }
    public String DE { get; set; }
    #endregion

    #region Constructor
    public Description() {

    }
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