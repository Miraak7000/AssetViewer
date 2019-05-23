using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.ComponentModel;

namespace AssetViewer.Veras {


    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class Pool {

        [XmlArrayItem("Item", IsNullable = false)]
        public List<PoolItem> Items { get; set; }


        [XmlAttribute()]
        public string ID { get; set; }
    }


}