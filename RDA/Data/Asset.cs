using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RDA.Data {

  internal class Asset {

    #region Properties
    public String GUID { get; set; }
    public String Name { get; set; }
    public String IconFilename { get; set; }
    public String Description { get; set; }
    public List<Asset> Items { get; }
    #endregion

    #region Constructor
    public Asset() {
      this.Items = new List<Asset>();
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      return $"{this.GUID} - {this.Name}";
    }
    #endregion

  }

}