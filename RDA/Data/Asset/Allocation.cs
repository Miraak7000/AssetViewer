using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class Allocation {

    #region Public Properties

    public String ID { get; set; }
    public Description Text { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Allocation(String template, String value) {
      if (value == "None" || value == null) {
        switch (template) {
          case "TownhallItem":
            this.ID = "TownHall";
            this.Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "ShipSpecialist":
          case "VehicleItem":
          case "ActiveItem":
            this.ID = "Ship";
            this.Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "GuildhouseItem":
            this.ID = "GuildHouse";
            this.Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "HarborOfficeItem":
            this.ID = "HarborOffice";
            this.Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "1010470":
          case "CultureItem":
            this.ID = "Zoo";
            this.Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "1010471":
            this.ID = "Museum";
            this.Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "110935":
            this.ID = "BotanicGarden";
            this.Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "QuestItem":
          case "StartExpeditionItem":
          case "FluffItem":
          case "ItemWithUI":
          case "ItemConstructionPlan":
            this.ID = "NoneAllocation";
            this.Text = new Description("-1230");
            break;

          default:
            throw new NotImplementedException();
        }
      }
      else {
        this.ID = value;
        this.Text = new Description(Assets.KeyToIdDict[value]);
        if (Assets.Icons.ContainsKey(value)) {
          this.Text.Icon = new Icon(Assets.Icons[value]);
        }
        else if (Assets.KeyToIdDict.ContainsKey(value)) {
          this.Text.Icon = new Description(Assets.KeyToIdDict[value]).Icon;
        }
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("A");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(this.Text.ToXml("T"));
      return result;
    }

    #endregion Public Methods
  }
}