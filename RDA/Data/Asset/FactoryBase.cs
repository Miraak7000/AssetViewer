using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {

  public class FactoryBase {

    #region Public Properties

    public string CycleTime { get; set; }
    public List<Upgrade> FactoryInputs { get; private set; }
    public List<Upgrade> FactoryOutputs { get; private set; }

    #endregion Public Properties

    #region Public Constructors

    public FactoryBase(XElement element) {
      if (element.Element("CycleTime")?.Value is string time) {
        CycleTime = time;
      }
      if (element.Element("FactoryInputs") != null) {
        FactoryInputs = new List<Upgrade>();
        foreach (var item in element.Element("FactoryInputs").Elements("Item")) {
          FactoryInputs.Add(new Upgrade { Text = new Description(item.Element("Product").Value), Value = $"{item.Element("Amount")?.Value ?? "1"} / {item.Element("StorageAmount")?.Value ?? "1"}" });
        }
      }
      if (element.Element("FactoryOutputs") != null) {
        FactoryOutputs = new List<Upgrade>();
        foreach (var item in element.Element("FactoryOutputs").Elements("Item")) {
          FactoryOutputs.Add(new Upgrade { Text = new Description(item.Element("Product").Value), Value = $"{item.Element("Amount")?.Value ?? "1"} / {item.Element("StorageAmount").Value}" });
        }
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("FB");
      if (CycleTime != null) {
        result.Add(new XAttribute("CT", CycleTime));
      }
      if (FactoryInputs != null) {
        result.Add(new XElement("FI", FactoryInputs.Select(s => s.ToXml())));
      }
      if (FactoryOutputs != null) {
        result.Add(new XElement("FO", FactoryOutputs.Select(s => s.ToXml())));
      }
      return result;
    }

    #endregion Public Methods
  }
}