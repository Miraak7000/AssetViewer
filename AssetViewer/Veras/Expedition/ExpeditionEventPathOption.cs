using AssetViewer.Data;
using AssetViewer.Veras.Expedition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Veras {

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ExpeditionEventPathOption {

        #region Properties

        public Description Text { get; set; }

        [XmlAttribute]
        public string ID { get; set; }

        [XmlAttribute]
        public string OptionAttribute { get; set; }

        [XmlArrayItem("Item", IsNullable = false)]
        public List<ExpeditionEventPathOptionsOptionRequirementsItem> Requirements { get; set; }

        #endregion Properties
    }
}