using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Veras {
    public class RewardWithDetailsList : List<RootWithDetails> {
        #region Constructors

        public RewardWithDetailsList() : base() {
        }

        #endregion Constructors

        #region Methods

        public RewardWithDetailsList Copy() {
            return new RewardWithDetailsList(this.Select(l => l.Copy()));
        }

        public void AddSourceAsset(XElement element, HashSet<XElement> details = default) {
            var assetID = element.XPathSelectElement("Values/Standard/GUID").Value;
            var expeditionName = element.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value;
            var questGiver = element.XPathSelectElement("Values/Quest/QuestGiver")?.Value;
            if (expeditionName != null) {
                var expedition = this.Find(w => w.Root.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value == expeditionName);
                if (expedition.Root == null) {
                    this.Add(new RootWithDetails(element, details));
                }
                else {
                    foreach (var item in details) {
                        expedition.Details.Add(item);
                    }
                }
                return;
            }
            else if (questGiver != null) {
                var quest = this.Find(w => w.Root.XPathSelectElement("Values/Quest/QuestGiver")?.Value == questGiver);
                if (quest.Root == null) {
                    this.Add(new RootWithDetails(element, details));
                }
                else {
                    foreach (var item in details) {
                        quest.Details.Add(item);
                    }
                }
                return;
            }
            var old = this.Find(l => l.Root.XPathSelectElement("Values/Standard/GUID").Value == assetID);
            if (old.Root != null) {
                foreach (var item in details) {
                    old.Details.Add(item);
                }
            }
            else {
                this.Add(new RootWithDetails(element, details));
            }
        }

        public void AddSourceAsset(RewardWithDetailsList input, Details details = null) {
            foreach (var item in input) {
                this.AddSourceAsset(item.Root, details?.Items ?? item.Details);
            }
        }

        #endregion Methods

        private RewardWithDetailsList(IEnumerable<RootWithDetails> collection) : base(collection) {
        }
    }
}