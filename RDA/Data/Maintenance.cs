using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {

  public class Maintenance {

    #region Properties

    public List<Upgrade> MaintenanceCosts { get; set; } = new List<Upgrade>();

    #endregion Properties

    #region Constructors

    public Maintenance(XElement element) {
      foreach (var item in element.Descendants("Item")) {
        MaintenanceCosts.Add(new Upgrade { Text = new Description(item.Element("Product").Value), Value = (item.Element("Amount")?.Value ?? "0") + (item.Element("InactiveAmount")?.Value == null ? "" : $" / {item.Element("InactiveAmount").Value}") });
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      if (MaintenanceCosts.Count > 0) {
        result.Add(new XElement("MaintenanceCosts", this.MaintenanceCosts.Select(s => s.ToXml())));
      }
      return result;
    }

    #endregion Methods
  }
}