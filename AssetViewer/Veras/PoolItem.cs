using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Veras {

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class PoolItem {


        [XmlAttribute()]
        public string ID { get; set; }

        [XmlAttribute()]
        public int Weight { get; set; }
    }


}