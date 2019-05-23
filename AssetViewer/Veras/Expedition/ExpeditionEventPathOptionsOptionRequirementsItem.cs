using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;

namespace AssetViewer.Veras.Expedition {

    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ExpeditionEventPathOptionsOptionRequirementsItem {

        [XmlAttribute]
        public string NeededAttribute { get; set; }
    }


}
