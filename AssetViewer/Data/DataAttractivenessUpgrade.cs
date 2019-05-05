using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;

namespace AssetViewer.Data {

  public class DataAttractivenessUpgrade {

    #region Properties
    public String Text {
      get {
        switch (App.Language) {
          case Languages.German:
            return "Attraktivität";
          default:
            return "Attractiveness";
        }
      }
    }
    public String Value {
      get {
        var upgrade = this.Element.XPathSelectElement("Values/CultureUpgrade/AttractivenessUpgrade");
        var value = Int32.Parse(upgrade.Element("Value").Value);
        return value > 0 ? $"+{value}" : $"{value}";
      }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public DataAttractivenessUpgrade(XElement element) {
      this.Element = element;
    }
    #endregion

  }

}