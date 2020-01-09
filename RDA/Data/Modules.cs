using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {
  public class Modules {
    #region Properties

    public string Limit { get; set; }
    public List<Description> PossibleModules { get; set; } = new List<Description>();

    #endregion Properties

    #region Constructors

    public Modules(XElement element) {
      this.Limit = element.Descendants("Limit").FirstOrDefault()?.Value;
      foreach (var item in element.Descendants("ModuleGUID").Select(e => new Description(e.Value))) {
        PossibleModules.Add(item);
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      if (this.Limit != null) {
        result.Add(new XAttribute("Limit", this.Limit));
      }
      if (PossibleModules.Count > 0) {
        result.Add(new XElement("PossibleModules", this.PossibleModules.Select(s => s.ToXml("Module"))));
      }
      return result;
    }

    #endregion Methods
  }
}