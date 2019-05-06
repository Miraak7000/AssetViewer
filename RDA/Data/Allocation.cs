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
    public Allocation(String value) {
      switch (value) {
        case "HarborOffice":
          this.ID = value;
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_harbour_kontor.png");
          this.Text = new Description(Helper.GetDescriptionID(value));
          break;
        case "TownHall":
          this.ID = value;
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_townhall.png");
          this.Text = new Description(Helper.GetDescriptionID(value));
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

  }

}