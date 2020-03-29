using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class EventOption {

    #region Public Properties

    [XmlElement("T")]
    public Description Text { get; set; }

    [XmlAttribute]
    public string ID { get; set; }

    [XmlElement("OA")]
    public Description OptionAttribute { get; set; }

    [XmlArray("R")]
    [XmlArrayItem("I", IsNullable = false)]
    public List<EventOptionRequirement> Requirements { get; set; }

    #endregion Public Properties
  }
}