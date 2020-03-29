using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {
  public class FactoryBase {
    #region Properties

    public string CycleTime { get; set; }
    public List<Upgrade> FactoryInputs { get; private set; }
    public List<Upgrade> FactoryOutputs { get; private set; }

    #endregion Properties

    #region Constructors

    public FactoryBase(XElement element) {
      if (element.Element("CycleTime")?.Value is string time) {
        CycleTime = time;
      }
      if (element.Element("FactoryInputs") != null) {
        this.FactoryInputs = new List<Upgrade>();
        foreach (var item in element.Element("FactoryInputs").Elements("Item")) {
          this.FactoryInputs.Add(new Upgrade { Text = new Description(item.Element("Product").Value), Value = $"{item.Element("Amount").Value} / {item.Element("StorageAmount").Value}" });
        }
      }
      if (element.Element("FactoryOutputs") != null) {
        this.FactoryOutputs = new List<Upgrade>();
        foreach (var item in element.Element("FactoryOutputs").Elements("Item")) {
          this.FactoryOutputs.Add(new Upgrade { Text = new Description(item.Element("Product").Value), Value = $"{item.Element("Amount").Value} / {item.Element("StorageAmount").Value}" });
        }
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement("FB");
      if (this.CycleTime != null) {
        result.Add(new XAttribute("CT", this.CycleTime));
      }
      if (FactoryInputs != null) {
        result.Add(new XElement("FI", this.FactoryInputs.Select(s => s.ToXml())));
      }
      if (FactoryOutputs != null) {
        result.Add(new XElement("FO", this.FactoryOutputs.Select(s => s.ToXml())));
      }
      return result;
    }

    #endregion Methods
  }
}