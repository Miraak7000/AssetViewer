using System;

namespace RDA.Data {

  public class CultureUpgrade {

    #region Properties
    public AttractivenessUpgrade Attractiveness { get; set; }
    #endregion

    public class AttractivenessUpgrade {

      #region Properties
      public String Value { get; set; }
      #endregion

    }
  }

}