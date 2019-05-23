using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace AssetViewer.Veras {

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ExpeditionEventPath {

        #region Properties
        [XmlArrayItem("Item", IsNullable = false)]
        public List<ExpeditionEventPathRewardsItem> Rewards { get; set; }

        [XmlArrayItem("Option", IsNullable = false)]
        public List<ExpeditionEventPathOption> Options { get; set; }

        [XmlAttribute()]
        public string ID { get; set; }

        #endregion Properties
    }
}