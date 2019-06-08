using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class ExpeditionEvent {

    #region Properties

    public Description Name { get; set; }

    [XmlArrayItem("Path", IsNullable = false)]
    public List<EventPath> Paths { get; set; }

    [XmlAttribute]
    public string ID { get; set; }

    #endregion Properties
  }
}