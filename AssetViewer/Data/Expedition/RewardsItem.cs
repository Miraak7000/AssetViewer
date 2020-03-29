using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class RewardsItem {

    #region Public Properties

    [XmlAttribute]
    public int ID { get; set; }

    [XmlAttribute("A")]
    public int Amount { get; set; }

    #endregion Public Properties
  }
}