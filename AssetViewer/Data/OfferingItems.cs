using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Templates;

namespace AssetViewer.Data {

  public class OfferingItems {

    #region Properties
    public Progression Progression { get; set; }
    public List<Asset> Assets { get; set; }
    #endregion

    #region Constructor
    public OfferingItems(XElement asset) {
      this.Progression = (Progression)Enum.Parse(typeof(Progression), asset.Attribute("Progression").Value);
      this.Assets = asset.XPathSelectElements("Assets/*").Select(s => new Asset(s)).ToList();
    }
    #endregion

  }

}