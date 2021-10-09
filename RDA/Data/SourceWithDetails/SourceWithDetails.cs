using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {

  public class SourceWithDetails {

    #region Public Properties

    public XElement Source { get; set; }
    public HashSet<AssetWithWeight> Details { get; set; }
    public bool IsRollable { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public SourceWithDetails(SourceWithDetails other) : this(other.Source, other.Details, other.IsRollable) {
    }

    public SourceWithDetails(XElement root, HashSet<AssetWithWeight> details, bool isRollable = false) : this() {
      Details = new HashSet<AssetWithWeight>(details.Select(d => d.Copy()));
      Source = root;
      IsRollable = isRollable;
    }

    public SourceWithDetails() {
    }

    #endregion Public Constructors

    #region Public Methods

    public SourceWithDetails Copy() {
      return new SourceWithDetails(this);
    }

    #endregion Public Methods
  }
}