using System;

namespace RDA.Data {

  public class CultureUpgrade {

    #region Properties

    public AttractivenessUpgrade Attractiveness { get; set; }

    #endregion Properties

    #region Classes

    public class AttractivenessUpgrade {

      #region Properties

      public String Value { get; set; }

      #endregion Properties
    }

    #endregion Classes
  }
}