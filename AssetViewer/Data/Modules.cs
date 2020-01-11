using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Modules {

    #region Properties

    public string Limit { get; set; }
    public List<Description> PossibleModules { get; set; } = new List<Description>();

    #endregion Properties

    #region Constructors

    public Modules(XElement element) {
      Limit = element.Attribute("Limit")?.Value;
      if (element.Element("PossibleModules")?.HasElements ?? false) {
        this.PossibleModules = element.Element("PossibleModules").Elements().Select(s => new Description(s)).ToList();
      }
    }

    #endregion Constructors
  }
}