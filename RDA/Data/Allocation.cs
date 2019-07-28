using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class Allocation {

    #region Properties

    public String ID { get; set; }
    public Description Text { get; set; }

    #endregion Properties

    #region Constructors

    public Allocation(String template, String value) {
      if (value == "None" || value == null) {
        switch (template) {
          case "TownhallItem":
            this.ID = "TownHall";
            this.Text = new Description(Assets.Descriptions[ID]);
            break;

          case "ShipSpecialist":
          case "VehicleItem":
          case "ActiveItem":
            this.ID = "Ship";
            this.Text = new Description(Assets.Descriptions[ID]);
            break;

          case "GuildhouseItem":
            this.ID = "GuildHouse";
            this.Text = new Description(Assets.Descriptions[ID]);
            break;

          case "HarborOfficeItem":
            this.ID = "HarborOffice";
            this.Text = new Description(Assets.Descriptions[ID]);
            break;

          case "CultureItem":
            this.ID = "Zoo";
            this.Text = new Description(Assets.Descriptions[ID]);
            break;

          default:
            throw new NotImplementedException();
        }
      }
      else {
        this.ID = value;
        this.Text = new Description(Assets.Descriptions[value]);
        if (Assets.Icons.ContainsKey(value)) {
          this.Text.Icon = new Icon(Assets.Icons[value]);
        }
        else if (Assets.Descriptions.ContainsKey(value)) {
          this.Text.Icon = new Description(Assets.Descriptions[value]).Icon;
        }
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(this.Text.ToXml("Text"));
      return result;
    }

    #endregion Methods
  }
}