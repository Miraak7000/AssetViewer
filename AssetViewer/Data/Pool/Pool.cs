using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class Pool {

    #region Properties

    [XmlArrayItem("Item", IsNullable = false)]
    public List<PoolItem> Items { get; set; }

    public IEnumerable<object> TrueItems => Items.Select(i => i.Item);

    [XmlAttribute]
    public int ID { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    #endregion Properties
  }
}