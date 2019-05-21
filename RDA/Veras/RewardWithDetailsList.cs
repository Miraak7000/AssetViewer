using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Veras {

    public class RewardWithDetailsList : List<RewardWithDetails> {

        #region Constructors

        public RewardWithDetailsList() : base() {
        }

        #endregion Constructors

        #region Methods

        public RewardWithDetailsList Copy() {
            return new RewardWithDetailsList(this.Select(l => l.Copy()));
        }

        //public RewardWithDetailsList(RewardWithDetailsList collection) : base(collection.Copy()) {
        //}
        public void AddSourceAsset(XElement element, HashSet<XElement> details = default) {
            var assetID = element.XPathSelectElement("Values/Standard/GUID").Value;
            var expeditionName = element.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value;
            var questGiver = element.XPathSelectElement("Values/Quest/QuestGiver")?.Value;
            //if (!source.Any(w => w.Root.XPathSelectElement("Values/Standard/GUID").Value == assetID)) {
            if (expeditionName != null) {
                var expedition = this.Find(w => w.Root.XPathSelectElement("Values/Expedition/ExpeditionName")?.Value == expeditionName);
                if (expedition.Root == null) {
                    this.Add(new RewardWithDetails(element, details));
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
                    this.Add(new RewardWithDetails(element, details));
                }
                else {
                    foreach (var item in details) {
                        quest.Details.Add(item);
                    }
                }
                return;
            }
            if (!this.Any(w => w.Root.XPathSelectElement("Values/Standard/GUID").Value == assetID)) {
                this.Add(new RewardWithDetails(element, details));
            }
            //}
            //else {
            //    Console.WriteLine("");
            //}
        }

        public void AddSourceAsset(RewardWithDetailsList input, Details details = null) {
            foreach (var item in input) {
                this.AddSourceAsset(item.Root, details?.Items ?? item.Details);
            }
        }

        #endregion Methods

        private RewardWithDetailsList(IEnumerable<RewardWithDetails> collection) : base(collection) {
        }
    }
}