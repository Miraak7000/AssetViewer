using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  public class RewardsItem {

    #region Properties

    [XmlAttribute]
    public int ID { get; set; }

    [XmlAttribute]
    public int Amount { get; set; }

    #endregion Properties
  }
}