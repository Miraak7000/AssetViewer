using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class EventOptionRequirement {

    #region Properties

    public Description NeededAttribute { get; set; }

    [XmlAttribute]
    public string ID { get; set; }

    [XmlAttribute]
    public string Amount { get; set; }

    #endregion Properties
  }
}