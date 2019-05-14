using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleNullReferenceException
namespace RDA.Data {

  public class RewardPool {

    #region Properties
    public String ID { get; set; }
    public Int32 Weight { get; set; }
    public List<String> Items { get; set; }
    #endregion

    #region Constructor
    public RewardPool(String id, Int32 weight) {
      this.ID = id;
      this.Weight = weight;
      this.Items = new List<String>();
      var rewardItem = Program.Original.XPathSelectElement($"//Asset[Values/Standard/GUID={id}]");
      switch (rewardItem.Element("Template").Value) {
        case "QuestItem":
          // ignore
          break;
        case "RewardPool":
          break;
        case "Product":
          this.Items.Add(id);
          break;
        default:
          throw new NotImplementedException(rewardItem.Element("Template").Value);
      }
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      return $"{this.ID} - {this.Weight}";
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement("Reward");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XAttribute("Weight", this.Weight));
      result.Add(new XAttribute("Items", this.Items.Select(s => s)));
      return result;
    }
    #endregion

  }

}