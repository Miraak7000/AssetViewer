using System;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class Description : IEquatable<Description> {

    #region Public Properties

    [XmlAttribute]
    public virtual int ID { get; set; }

    public Icon Icon { get; set; }

    [XmlIgnore]
    public virtual string CurrentLang => AssetProvider.Descriptions[ID];

    [XmlAttribute]
    public DescriptionFontStyle FontStyle { get; set; }

    public Description AdditionalInformation { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Description() {
    }

    public Description(XElement item) {
      ID = int.Parse(item.Attribute("ID").Value);
      if (item.Attribute("I")?.Value is string icon) {
        Icon = new Icon(icon);
      }
      FontStyle = item.Attribute("FS") == null ? default : (DescriptionFontStyle)Convert.ToInt32(item.Attribute("FS").Value);
      AdditionalInformation = item.Element("AI")?.Value == null ? null : new Description(item.Element("AI"));
    }

    public Description(int id) {
      ID = id;
    }

    #endregion Public Constructors

    #region Public Methods

    public override string ToString() {
      return CurrentLang;
    }

    public bool Equals(Description other) {
      return ID.Equals(other?.ID);
    }

    public override int GetHashCode() {
      return ID.GetHashCode();
    }

    #endregion Public Methods
  }
}