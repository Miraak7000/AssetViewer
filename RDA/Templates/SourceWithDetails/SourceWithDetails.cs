using System.Collections.Generic;
using System.Xml.Linq;

namespace RDA.Templates {

  public struct SourceWithDetails {

    #region Properties

    public XElement Source { get; set; }
    public HashSet<XElement> Details { get; set; }

    #endregion Properties

    #region Constructors

    public SourceWithDetails(SourceWithDetails other) : this(other.Source, other.Details) {
    }

    public SourceWithDetails(XElement root, HashSet<XElement> details) : this() {
      Details = new HashSet<XElement>(details);
      Source = root;
    }

    #endregion Constructors

    #region Methods

    public SourceWithDetails Copy() {
      return new SourceWithDetails(this);
    }

    #endregion Methods
  }
}