using System;
using System.Xml.Linq;

namespace AssetViewer.Data {

  [Serializable]
  public class Icon {

    #region Properties

    public String Filename { get; set; }

    #endregion Properties

    #region Constructors

    public Icon() {
    }

    public Icon(XElement item) {
      if (!String.IsNullOrEmpty(item.Element("Filename")?.Value)) {
        this.Filename = $"/AssetViewer;component/Resources/{item.Element("Filename").Value}";
      }
    }

    #endregion Constructors
  }
}