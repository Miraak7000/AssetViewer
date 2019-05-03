using System;
using System.Collections.Generic;

namespace RDA.Data {

  internal class GuildhouseItem : Asset {

    #region Properties
    public String Rarity { get; set; }
    public List<String> EffectTargets { get; }
    #endregion

    #region Constructor
    public GuildhouseItem() {
      this.EffectTargets = new List<String>();
    }
    #endregion

  }

}