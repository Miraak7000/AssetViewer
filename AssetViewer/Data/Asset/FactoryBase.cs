using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class FactoryBase {

    #region Public Properties

    public string CycleTime { get; set; }
    public List<Upgrade> FactoryInputs { get; }
    public List<Upgrade> FactoryOutputs { get; }

    #endregion Public Properties

    #region Public Constructors

    public FactoryBase() {
    }

    public FactoryBase(XElement element) {
      CycleTime = element.Attribute("CT")?.Value;
      if (element.Element("FI")?.HasElements ?? false) {
        FactoryInputs = element.Element("FI").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (element.Element("FO")?.HasElements ?? false) {
        FactoryOutputs = element.Element("FO").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }

    #endregion Public Constructors
  }
}