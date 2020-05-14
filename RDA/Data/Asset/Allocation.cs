using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class Allocation {

    #region Public Properties

    public string ID { get; set; }
    public Description Text { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Allocation(string template, string value) {
      if (value == "None" || value == null) {
        switch (template) {
          case "TownhallItem":
            ID = "TownHall";
            Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "ShipSpecialist":
          case "VehicleItem":
          case "ActiveItem":
            ID = "Ship";
            Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "GuildhouseItem":
            ID = "GuildHouse";
            Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "HarborOfficeItem":
            ID = "HarborOffice";
            Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "1010470":
          case "CultureItem":
            ID = "Zoo";
            Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "1010471":
            ID = "Museum";
            Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "110935":
            ID = "BotanicGarden";
            Text = new Description(Assets.KeyToIdDict[ID]);
            break;

          case "QuestItem":
          case "StartExpeditionItem":
          case "FluffItem":
          case "ItemWithUI":
          case "ItemConstructionPlan":
            ID = "NoneAllocation";
            Text = new Description("-1230");
            break;

          default:
            throw new NotImplementedException();
        }
      }
      else {
        ID = value;
        Text = new Description(Assets.KeyToIdDict[value]);
        if (Assets.Icons.ContainsKey(value)) {
          Text.Icon = new Icon(Assets.Icons[value]);
        }
        else if (Assets.KeyToIdDict.ContainsKey(value)) {
          Text.Icon = new Description(Assets.KeyToIdDict[value]).Icon;
        }
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("A");
      result.Add(new XAttribute("ID", ID));
      result.Add(Text.ToXml("T"));
      return result;
    }

    #endregion Public Methods
  }
}