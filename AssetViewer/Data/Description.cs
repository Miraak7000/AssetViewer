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

    public Icon Icon { get; set; }

    [XmlIgnore]
    public string CurrentLang => App.Descriptions[ID];

    [XmlAttribute]
    public DescriptionFontStyle FontStyle { get; set; }

    public Description AdditionalInformation { get; set; }

    #endregion Properties

    #region Constructors

    public Description() {
    }
    public Description(XElement item) {
      var h = item.Attribute("ID")?.Value;
      if (h == null) {
      }
      this.ID = item.Attribute("ID").Value;
      this.Icon = item.Element("Icon")?.Value == null ? null : new Icon(item.Element("Icon"));
      var att = item.Attribute("FontStyle");
      if (att != null) {
      }
      this.FontStyle = item.Attribute("FontStyle") == null ? default : (DescriptionFontStyle)Convert.ToInt32(item.Attribute("FontStyle").Value);
      this.AdditionalInformation = item.Element("AdditionalInformation")?.Value == null ? null : new Description(item.Element("AdditionalInformation"));
    }
    public Description(string id) {
      ID = id;
    }

    #endregion Constructors

    #region Methods

    public override string ToString() {
      return CurrentLang;
    }

    #endregion Methods
  }
}