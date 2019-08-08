using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;

namespace AssetViewer.Data {

  public class DataExpeditionAttribute {

    #region Properties
    public String Text {
      get {
        switch (App.Language) {
          case Languages.German:
            return "Expeditions-Bonus";
          default:
            return "Expeditions bonus";
        }
      }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public DataExpeditionAttribute(XElement element) {
      this.Element = element;
    }
    #endregion

  }

}