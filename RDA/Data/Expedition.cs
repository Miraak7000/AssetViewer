using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleNullReferenceException
namespace RDA.Data {

  public class Expedition {

    #region Properties
    public String ID { get; set; }
    public String Name { get; set; }
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public String ExpeditionRegion { get; set; }
    public String FillEventPool { get; set; }
    public List<RewardPosition> Rewards { get; set; }
    #endregion

    #region Constructor
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
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("Name", this.Name));
      result.Add(this.Icon == null ? new XElement("Icon") : this.Icon.ToXml());
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("ExpeditionRegion", this.ExpeditionRegion));
      result.Add(new XElement("FillEventPool", this.FillEventPool));
      result.Add(new XElement("Rewards"));
      if (this.Rewards != null) {
        for (int x = 0; x < this.Rewards.Count; x++) {
          result.Element("Rewards").Add(this.Rewards[x].ToXml(x));
        }
      }
      return result;
    }
    #endregion

    #region Private Methods
    private void ProcessElement_Standard(XElement element) {
      this.ID = element.Element("GUID").Value;
      this.Name = element.Element("Name").Value;
      if (element.Element("IconFilename") != null) this.Icon = new Icon(element.Element("IconFilename").Value);
    }
    private void ProcessElement_Expedition(XElement element) {
      this.Text = new Description(element.Element("ExpeditionName").Value);
      this.ExpeditionRegion = element.Element("ExpeditionRegion")?.Value;
      this.FillEventPool = element.Element("FillEventPool")?.Value;
      if (element.Element("Reward") != null) {
        var rewardAssets = Program.Original.XPathSelectElements($"//Asset[Values/Standard/GUID={element.Element("Reward").Value}]/Values/Reward/RewardAssets/Item");
        this.Rewards = new List<RewardPosition>();
        foreach (var rewardAsset in rewardAssets) {
          var position = new RewardPosition(rewardAsset);
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
    #endregion

  }

}