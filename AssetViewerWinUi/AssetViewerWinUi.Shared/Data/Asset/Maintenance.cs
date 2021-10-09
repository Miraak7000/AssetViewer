using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Maintenance {

    #region Public Properties

    public List<Upgrade> MaintenanceCosts { get; set; } = new List<Upgrade>();

    #endregion Public Properties

    #region Public Constructors

    public Maintenance() {
    }

    public Maintenance(XElement element) {
      if (element.Element("MC")?.HasElements ?? false) {
        MaintenanceCosts = element.Element("MC").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }

    #endregion Public Constructors
  }
}