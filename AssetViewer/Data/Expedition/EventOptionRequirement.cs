using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public partial class EventOptionRequirement {

    #region Properties

    [XmlAttribute]
    public string NeededAttribute { get; set; }

    #endregion Properties
  }
}