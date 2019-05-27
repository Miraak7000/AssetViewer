using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Veras.Expedition {

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ExpeditionEventPathOptionsOptionRequirementsItem {

        #region Properties

        [XmlAttribute]
        public string NeededAttribute { get; set; }

        #endregion Properties
    }
}