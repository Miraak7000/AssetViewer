using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Modules {

    #region Public Properties

    public string Limit { get; set; }
    public List<Description> PossibleModules { get; set; } = new List<Description>();

    #endregion Public Properties

    #region Public Constructors

    public Modules(XElement element) {
      Limit = element.Attribute("L")?.Value;
      if (element.Element("PM")?.HasElements ?? false) {
        PossibleModules = element.Element("PM").Elements().Select(s => new Description(s)).ToList();
      }
    }

    #endregion Public Constructors
  }
}