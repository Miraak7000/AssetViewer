using RDA.Data;
using System.Collections.Generic;

namespace RDA.Comparer {
  internal class BasicUpgradeComparer : IEqualityComparer<Upgrade> {
    #region Methods

    public bool Equals(Upgrade x, Upgrade y) {
      return x.Text.Equals(y.Text) && x.Value.Equals(y.Value);
    }

    public int GetHashCode(Upgrade obj) {
      return 0;
    }

    #endregion Methods
  }
}