using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class EventOption {

    #region Properties

    public Description Text { get; set; }

    [XmlAttribute]
    public string ID { get; set; }

    public Description OptionAttribute { get; set; }

    [XmlArrayItem("Item", IsNullable = false)]
    public List<EventOptionRequirement> Requirements { get; set; }

    #endregion Properties
  }
}