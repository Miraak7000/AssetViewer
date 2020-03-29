using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class EventPath {

    #region Public Properties

    [XmlArray("R")]
    [XmlArrayItem("I", IsNullable = false)]
    public List<RewardsItem> Rewards { get; set; }

    [XmlArray("OL")]
    [XmlArrayItem("O", IsNullable = false)]
    public List<EventOption> Options { get; set; }

    [XmlAttribute]
    public string ID { get; set; }

    #endregion Public Properties
  }
}