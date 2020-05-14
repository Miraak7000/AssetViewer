using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot("EE", Namespace = "", IsNullable = false)]
  public class ExpeditionEvent {

    #region Public Properties

    [XmlElement("N")]
    public Description Name { get; set; }

    [XmlArray("PL")]
    [XmlArrayItem("P", IsNullable = false)]
    public List<EventPath> Paths { get; set; }

    [XmlAttribute]
    public string ID { get; set; }

    #endregion Public Properties
  }
}