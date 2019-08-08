using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;

namespace AssetViewer.Data {

  public class DataAddedFertility {

    #region Properties
    public String Icon {
      get { return null; }
    }
    public String Text {
      get {
        switch (App.Language) {
          case Languages.German:
            return "bereitgestellt";
          default:
            return "provided";
        }
      }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public DataAddedFertility(XElement element) {
      this.Element = element;
    }
    #endregion

  }

}