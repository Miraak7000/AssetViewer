using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {

  public class Modules {

    #region Public Properties

    public string Limit { get; set; }
    public List<Description> PossibleModules { get; set; } = new List<Description>();

    #endregion Public Properties

    #region Public Constructors

    public Modules(XElement element, GameTypes gameType) {
      Limit = element.Descendants("Limit").FirstOrDefault()?.Value;
      foreach (var item in element.Descendants("ModuleGUID").Select(e => new Description(e.Value, gameType))) {
        PossibleModules.Add(item);
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("Mo");
      if (Limit != null) {
        result.Add(new XAttribute("L", Limit));
      }
      if (PossibleModules.Count > 0) {
        result.Add(new XElement("PM", PossibleModules.Select(s => s.ToXml("M"))));
      }
      return result;
    }

    #endregion Public Methods
  }
}