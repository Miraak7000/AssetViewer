using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;

namespace AssetViewer.Data {

  public class DataIncidentExplosionIncreaseUpgrade {

    #region Properties
    public String Text {
      get {
        switch (App.Language) {
          case Languages.German:
            return "Explosionswahrscheinlichkeit";
          default:
            return "Explosion probability";
        }
      }
    }
    public String Value {
      get {
        var upgrade = this.Element.XPathSelectElement("Values/IncidentInfectableUpgrade/IncidentExplosionIncreaseUpgrade");
        var value = Int32.Parse(upgrade.Element("Value").Value) * 10;
        return value > 0 ? $"+{value}%" : $"{value}%";
      }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public DataIncidentExplosionIncreaseUpgrade(XElement element) {
      this.Element = element;
    }
    #endregion

  }

}