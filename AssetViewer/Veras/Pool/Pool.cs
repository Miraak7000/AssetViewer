using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Veras {

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class Pool {

        #region Properties

        [XmlArrayItem("Item", IsNullable = false)]
        public List<PoolItem> Items { get; set; }

        [XmlAttribute()]
        public string ID { get; set; }

        #endregion Properties
    }
}