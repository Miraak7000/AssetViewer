using RDA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDA.Comparer {
  internal class BasicUpgradeComparer : IEqualityComparer<Upgrade> {
    public bool Equals(Upgrade x, Upgrade y) {
      return x.Text.Equals(y.Text) && x.Value.Equals(y.Value);

    }

    public int GetHashCode(Upgrade obj) {
      return 0;
    }
  }
}
