using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleNullReferenceException
namespace RDA.Data {

  public class Expedition {

    #region Public Properties

    public string ID { get; set; }
    public string Name { get; set; }
    public Description Text { get; set; }
    public string ExpeditionRegion { get; set; }
    public string FillEventPool { get; set; }
    public List<RewardPoolPosition> Rewards { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Expedition(XElement asset) {
      foreach (var element in asset.Element("Values").Elements()) {
        switch (element.Name.LocalName) {
          case "Locked":
          case "WorldMapSound":
            // ignore this nodes
            break;

          case "Standard":
            ProcessElement_Standard(element);
            break;

          case "Expedition":
            ProcessElement_Expedition(element);
            break;

          default:
            throw new NotImplementedException(element.Name.LocalName);
        }
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("E");
      result.Add(new XAttribute("ID", ID));
      result.Add(new XElement("N", Name));
      result.Add(Text.ToXml("T"));
      result.Add(new XElement("ER", ExpeditionRegion));
      result.Add(new XElement("FEP", FillEventPool));
      result.Add(new XElement("R"));
      if (Rewards != null) {
        for (var x = 0; x < Rewards.Count; x++) {
          result.Element("R").Add(Rewards[x].ToXml(x));
        }
      }
      return result;
    }

    #endregion Public Methods

    #region Private Methods

    private void ProcessElement_Standard(XElement element) {
      ID = element.Element("GUID").Value;
      Name = element.Element("Name").Value;
    }

    private void ProcessElement_Expedition(XElement element) {
      Text = new Description(element.Element("ExpeditionName").Value);
      ExpeditionRegion = element.Element("ExpeditionRegion")?.Value;
      FillEventPool = element.Element("FillEventPool")?.Value;
      if (element.Element("Reward") != null) {
        var rewardAssets = Assets.Original.XPathSelectElements($"//Asset[Values/Standard/GUID={element.Element("Reward").Value}]/Values/Reward/RewardAssets/Item");
        Rewards = new List<RewardPoolPosition>();
        foreach (var rewardAsset in rewardAssets) {
          var position = new RewardPoolPosition(rewardAsset);
          var amount = rewardAsset.Element("Amount")?.Value ?? "1";
          switch (amount) {
            case "3":
              Rewards.Add(position);
              Rewards.Add(position);
              Rewards.Add(position);
              break;

            case "2":
              Rewards.Add(position);
              Rewards.Add(position);
              break;

            case "1":
              Rewards.Add(position);
              break;
          }
        }
      }
    }

    #endregion Private Methods
  }
}