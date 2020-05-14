using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot("P", Namespace = "", IsNullable = false)]
  public class Pool {

    #region Public Properties

    [XmlArray("IL")]
    [XmlArrayItem("I", IsNullable = false)]
    public List<PoolItem> Items { get; set; }

    public IEnumerable<object> TrueItems => Items.Select(i => i.Item);

    [XmlAttribute]
    public int ID { get; set; }

    [XmlAttribute("N")]
    public string Name { get; set; }

    #endregion Public Properties
  }
}