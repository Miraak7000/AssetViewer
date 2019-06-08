using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "Status", IsNullable = false)]
  public class TourismStatus {

    #region Properties

    public Description Requirement { get; set; }
    public Description Text { get; set; }

    [XmlAttribute]
    public string Pool { get; set; }

    #endregion Properties
  }
}