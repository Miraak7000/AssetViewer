using System;
using System.Xml.Linq;

namespace AssetViewer.Data {

  [Serializable]
  public class Icon {

    #region Public Properties

    public string Filename { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Icon() {
    }

    public Icon(XElement item) {
      if (!string.IsNullOrEmpty(item.Attribute("F")?.Value)) {
        Filename = $"/AssetViewer;component/Resources/{item.Attribute("F").Value}";
      }
    }

    public Icon(string filename) {
      if (!string.IsNullOrEmpty(filename)) {
        Filename = $"/AssetViewer;component/Resources/{filename}";
      }
    }

    #endregion Public Constructors
  }
}