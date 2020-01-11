using System;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class Description : IEquatable<Description> {

    #region Properties

    [XmlAttribute]
    public virtual int ID { get; set; }

    public Icon Icon { get; set; }

    [XmlIgnore]
    public virtual string CurrentLang => App.Descriptions[ID];

    [XmlAttribute]
    public DescriptionFontStyle FontStyle { get; set; }

    public Description AdditionalInformation { get; set; }

    #endregion Properties

    #region Constructors

    public Description() {
    }

    public Description(XElement item) {
      this.ID = int.Parse(item.Attribute("ID").Value);
      this.Icon = item.Element("Icon")?.Value == null ? null : new Icon(item.Element("Icon"));
      this.FontStyle = item.Attribute("FontStyle") == null ? default : (DescriptionFontStyle)Convert.ToInt32(item.Attribute("FontStyle").Value);
      this.AdditionalInformation = item.Element("AdditionalInformation")?.Value == null ? null : new Description(item.Element("AdditionalInformation"));
    }

    public Description(int id) {
      ID = id;
    }

    #endregion Constructors

    #region Methods

    public override string ToString() {
      return CurrentLang;
    }

    public bool Equals(Description other) {
      return ID.Equals(other?.ID);
    }

    public override int GetHashCode() {
      return ID.GetHashCode();
    }

    #endregion Methods
  }
}