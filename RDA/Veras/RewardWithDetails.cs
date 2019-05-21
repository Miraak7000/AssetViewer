using System.Collections.Generic;
using System.Xml.Linq;

namespace RDA.Veras {

    public struct RewardWithDetails {

        #region Constructors

        public RewardWithDetails(RewardWithDetails other) : this(other.Root, other.Details) {
        }

        public RewardWithDetails(XElement root, HashSet<XElement> details) : this() {
            Details = new HashSet<XElement>(details);
            Root = root;
        }

        #endregion Constructors

        #region Properties

        public XElement Root { get; set; }
        public HashSet<XElement> Details { get; set; }

        #endregion Properties

        #region Methods

        internal RewardWithDetails Copy() {
            return new RewardWithDetails(this);
        }

        #endregion Methods
    }
}