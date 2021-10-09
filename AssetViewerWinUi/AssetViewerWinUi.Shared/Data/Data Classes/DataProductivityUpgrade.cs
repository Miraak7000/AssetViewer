using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;

namespace AssetViewer.Data {

  public class DataProductivityUpgrade {

    #region Properties
    public String Text {
      get {
        switch (App.Language) {
          case Languages.German:
            return "Produktivität";
          default:
            return "Productivity";
        }
      }
    }
    public String Value {
      get {
        var upgrade = this.Element.XPathSelectElement("Values/FactoryUpgrade/ProductivityUpgrade");
        var value = Int32.Parse(upgrade.Element("Value").Value);
        if (upgrade.Element("Percental")?.Value == "1") {
          return value > 0 ? $"+{value}%" : $"{value}%";
        } else {
          return value > 0 ? $"+{value}" : $"{value}";
        }
      }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public DataProductivityUpgrade(XElement element) {
      this.Element = element;
    }
    #endregion

  }

}