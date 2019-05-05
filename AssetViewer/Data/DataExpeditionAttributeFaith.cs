using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;

namespace AssetViewer.Data {

  public class DataExpeditionAttributeFaith {

    #region Properties
    public String Text {
      get {
        switch (App.Language) {
          case Languages.German:
            return "Glaube";
          default:
            return "Faith";
        }
      }
    }
    public String Value {
      get {
        var upgrade = this.Element.XPathSelectElement("Values/ExpeditionAttribute/ExpeditionAttributes/Item[Attribute='Faith']");
        var amount = upgrade?.Element("Amount")?.Value;
        if (amount == null) return null;
        var value = Int32.Parse(amount);
        return value > 0 ? $"+{value}" : $"{value}";
      }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public DataExpeditionAttributeFaith(XElement element) {
      this.Element = element;
    }
    #endregion

  }

}