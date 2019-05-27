using System.Collections.Generic;
using System.Xml.Linq;

namespace RDA.Veras {
    public struct RootWithDetails {
        #region Constructors

        public RootWithDetails(RootWithDetails other) : this(other.Root, other.Details) {
        }

        public RootWithDetails(XElement root, HashSet<XElement> details) : this() {
            Details = new HashSet<XElement>(details);
            Root = root;
        }

        #endregion Constructors

        #region Properties

        public XElement Root { get; set; }
        public HashSet<XElement> Details { get; set; }

        #endregion Properties

        #region Methods

        internal RootWithDetails Copy() {
            return new RootWithDetails(this);
        }

        #endregion Methods
    }
}