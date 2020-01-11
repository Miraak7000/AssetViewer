using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Maintenance {

    #region Properties

    public List<Upgrade> MaintenanceCosts { get; set; } = new List<Upgrade>();

    #endregion Properties

    #region Constructors

    public Maintenance() {
    }

    public Maintenance(XElement element) {
      if (element.Element("MaintenanceCosts")?.HasElements ?? false) {
        this.MaintenanceCosts = element.Element("MaintenanceCosts").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }

    #endregion Constructors
  }
}