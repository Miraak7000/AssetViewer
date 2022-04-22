using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {

  public class Maintenance {

    #region Public Properties

    public List<Upgrade> MaintenanceCosts { get; set; } = new List<Upgrade>();

    #endregion Public Properties

    #region Public Constructors

    public Maintenance(XElement element, GameTypes gameType) {
      foreach (var item in element.Descendants("Item")) {
        MaintenanceCosts.Add(new Upgrade { Text = new Description(item.Element("Product").Value, gameType), Value = (item.Element("Amount")?.Value ?? "0") + (item.Element("InactiveAmount")?.Value == null ? "" : $" / {item.Element("InactiveAmount").Value}") });
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("M");
      if (MaintenanceCosts.Count > 0) {
        result.Add(new XElement("MC", MaintenanceCosts.Select(s => s.ToXml())));
      }
      return result;
    }

    #endregion Public Methods
  }
}