using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Veras {

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ExpeditionEventPathRewardsItem {

        #region Properties

        [XmlAttribute()]
        public string ID { get; set; }

        [XmlAttribute()]
        public int Amount { get; set; }

        #endregion Properties
    }
}