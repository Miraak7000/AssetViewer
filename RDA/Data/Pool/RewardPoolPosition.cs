using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleNullReferenceException
namespace RDA.Data {

  public class RewardPoolPosition {

    #region Public Properties

    public string ID { get; set; }
    public List<RewardPool> RewardPool { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public RewardPoolPosition(XElement element, GameTypes gameType) {
      ID = element.Element("Reward").Value;
      RewardPool = new List<RewardPool>();
      var rewardItems = Assets.GUIDs[element.Element("Reward").Value, gameType].XPathSelectElements($"/Values/RewardPool/ItemsPool/Item");
      foreach (var rewardItem in rewardItems) {
        var weight = rewardItem.Element("Weight") == null ? 1 : int.Parse(rewardItem.Element("Weight").Value);
        if (weight == 0)
          continue;
        RewardPool.Add(new RewardPool(rewardItem.Element("ItemLink").Value, weight, gameType));
      }
      RewardPool = RewardPool.OrderByDescending(o => o.Weight).ToList();
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml(int position) {
      var result = new XElement("RPP");
      result.Add(new XAttribute("ID", ID));
      result.Add(new XAttribute("P", position));
      foreach (var rewardPool in RewardPool) {
        result.Add(rewardPool.ToXml());
      }
      return result;
    }

    #endregion Public Methods
  }
}