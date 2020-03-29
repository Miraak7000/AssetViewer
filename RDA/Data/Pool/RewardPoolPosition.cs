using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleNullReferenceException
namespace RDA.Data {
  public class RewardPoolPosition {
    #region Properties

    public String ID { get; set; }
    public List<RewardPool> RewardPool { get; set; }

    #endregion Properties

    #region Constructors

    public RewardPoolPosition(XElement element) {
      this.ID = element.Element("Reward").Value;
      this.RewardPool = new List<RewardPool>();
      var rewardItems = Assets.Original.XPathSelectElements($"//Asset[Values/Standard/GUID={element.Element("Reward").Value}]/Values/RewardPool/ItemsPool/Item");
      foreach (var rewardItem in rewardItems) {
        var weight = rewardItem.Element("Weight") == null ? 1 : Int32.Parse(rewardItem.Element("Weight").Value);
        if (weight == 0)
          continue;
        this.RewardPool.Add(new RewardPool(rewardItem.Element("ItemLink").Value, weight));
      }
      this.RewardPool = this.RewardPool.OrderByDescending(o => o.Weight).ToList();
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml(Int32 position) {
      var result = new XElement("RPP");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XAttribute("P", position));
      foreach (var rewardPool in this.RewardPool) {
        result.Add(rewardPool.ToXml());
      }
      return result;
    }

    #endregion Methods
  }
}