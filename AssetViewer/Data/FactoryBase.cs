using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class FactoryBase {

    #region Properties

    public string CycleTime { get; set; }
    public List<Upgrade> FactoryInputs { get; private set; }
    public List<Upgrade> FactoryOutputs { get; private set; }

    #endregion Properties

    #region Constructors

    public FactoryBase() {
    }

    public FactoryBase(XElement element) {
      CycleTime = element.Attribute("CycleTime")?.Value;
      if (element.Element("FactoryInputs")?.HasElements ?? false) {
        this.FactoryInputs = element.Element("FactoryInputs").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (element.Element("FactoryOutputs")?.HasElements ?? false) {
        this.FactoryOutputs = element.Element("FactoryOutputs").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }

    #endregion Constructors
  }
}