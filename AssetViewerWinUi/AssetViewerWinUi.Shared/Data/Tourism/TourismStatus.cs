using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "S", IsNullable = false)]
  public class TourismStatus {

    #region Public Properties

    [XmlElement("R")]
    public Description Requirement { get; set; }

    [XmlElement("T")]
    public Description Text { get; set; }

    [XmlAttribute("P")]
    public string Pool { get; set; }

    #endregion Public Properties
  }
}