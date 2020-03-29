using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleNullReferenceException
namespace RDA.Data {

  public class Expedition {

    #region Public Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public String ExpeditionRegion { get; set; }
    public String FillEventPool { get; set; }
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
            this.ProcessElement_Standard(element);
            break;

          case "Expedition":
            this.ProcessElement_Expedition(element);
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
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("N", this.Name));
      result.Add(this.Text.ToXml("T"));
      result.Add(new XElement("ER", this.ExpeditionRegion));
      result.Add(new XElement("FEP", this.FillEventPool));
      result.Add(new XElement("R"));
      if (this.Rewards != null) {
        for (int x = 0; x < this.Rewards.Count; x++) {
          result.Element("R").Add(this.Rewards[x].ToXml(x));
        }
      }
      return result;
    }

    #endregion Public Methods

    #region Private Methods

    private void ProcessElement_Standard(XElement element) {
      this.ID = element.Element("GUID").Value;
      this.Name = element.Element("Name").Value;
    }

    private void ProcessElement_Expedition(XElement element) {
      this.Text = new Description(element.Element("ExpeditionName").Value);
      this.ExpeditionRegion = element.Element("ExpeditionRegion")?.Value;
      this.FillEventPool = element.Element("FillEventPool")?.Value;
      if (element.Element("Reward") != null) {
        var rewardAssets = Assets.Original.XPathSelectElements($"//Asset[Values/Standard/GUID={element.Element("Reward").Value}]/Values/Reward/RewardAssets/Item");
        this.Rewards = new List<RewardPoolPosition>();
        foreach (var rewardAsset in rewardAssets) {
          var position = new RewardPoolPosition(rewardAsset);
          var amount = rewardAsset.Element("Amount")?.Value ?? "1";
          switch (amount) {
            case "3":
              this.Rewards.Add(position);
              this.Rewards.Add(position);
              this.Rewards.Add(position);
              break;

            case "2":
              this.Rewards.Add(position);
              this.Rewards.Add(position);
              break;

            case "1":
              this.Rewards.Add(position);
              break;
          }
        }
      }
    }

    #endregion Private Methods
  }
}