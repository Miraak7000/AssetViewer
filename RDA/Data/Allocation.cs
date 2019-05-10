using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class Allocation {

    #region Properties
    public String ID { get; set; }
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    #endregion

    #region Constructor
    public Allocation(String template, String value) {
      switch (template) {
        case "GuildhouseItem":
          this.Template_GuildhouseItem(value);
          break;
        case "TownhallItem":
          this.Template_TownhallItem(value);
          break;
        case "HarborOfficeItem":
          this.Template_HarborOfficeItem(value);
          break;
        case "VehicleItem":
          this.Template_VehicleItem(value);
          break;
        case "ShipSpecialist":
          this.Template_ShipSpecialist(value);
          break;
        case "CultureItem":
          this.Template_CultureItem(value);
          break;
        default:
          throw new NotImplementedException(template);
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(this.Icon.ToXml());
      result.Add(this.Text.ToXml("Text"));
      return result;
    }
    #endregion

    #region Private Methods
    private void Template_GuildhouseItem(String value) {
      switch (value) {
        case "HarborOffice":
          this.ID = value;
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_harbour_kontor.png");
          this.Text = new Description(Helper.GetDescriptionID("HarborOffice"));
          break;
        case "TownHall":
          this.ID = value;
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_townhall.png");
          this.Text = new Description(Helper.GetDescriptionID("TownHall"));
          break;
        case "RadiusBuilding":
          // seems not being implemented, so simply use the TradeUnion
          this.ID = "TradeUnion";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_guildhouse.png");
          this.Text = new Description("2346");
          break;
        default:
          if (value != null) throw new NotImplementedException();
          this.ID = "TradeUnion";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_guildhouse.png");
          this.Text = new Description("2346");
          break;
      }
    }
    private void Template_TownhallItem(String value) {
      switch (value) {
        default:
          if (value != null && value != "None") throw new NotImplementedException();
          this.ID = "TownHall";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_townhall.png");
          this.Text = new Description("2347");
          break;
      }
    }
    private void Template_HarborOfficeItem(String value) {
      switch (value) {
        default:
          if (value != null && value != "None") throw new NotImplementedException();
          this.ID = "HarborOffice";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_harbour_kontor.png");
          this.Text = new Description(Helper.GetDescriptionID("HarborOffice"));
          break;
      }
    }
    private void Template_VehicleItem(String value) {
      switch (value) {
        case "SailShip":
          this.ID = "SailShip";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_ship.png");
          this.Text = new Description("191455");
          break;
        case "SteamShip":
          this.ID = "SteamShip";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/ships/icon_ship_collier.png");
          this.Text = new Description(Helper.GetDescriptionID("SteamShip"));
          break;
        case "Warship":
          this.ID = "Warship";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/ships/icon_ship_battlecruiser.png");
          this.Text = new Description(Helper.GetDescriptionID("Warship"));
          break;
        default:
          if (value != null) throw new NotImplementedException();
          this.ID = "Ships";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_ship.png");
          this.Text = new Description("191454");
          break;
      }
    }
    private void Template_ShipSpecialist(String value) {
      switch (value) {
        case "Warship":
          this.ID = "Warship";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/ships/icon_ship_battlecruiser.png");
          this.Text = new Description(Helper.GetDescriptionID("Warship"));
          break;
        default:
          if (value != null) throw new NotImplementedException();
          this.ID = "Ships";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_ship.png");
          this.Text = new Description("191454");
          break;
      }
    }
    private void Template_CultureItem(String value) {
      switch (value) {
        case "Museum":
          this.ID = "Museum";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_museum.png");
          this.Text = new Description("2351");
          break;
        default:
          if (value != null) throw new NotImplementedException();
          this.ID = "Zoo";
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_zoo.png");
          this.Text = new Description("2349");
          break;
      }
    }
    #endregion

  }

}