using System;
using System.Xml.Linq;

namespace AssetViewer.Library {

  public class SelectableItem {

    #region Properties

    public String GUID { get; set; }
    public XElement Value { get; set; }
    public String IconFilename { get; set; }

    public String Icon {
      get {
        if (String.IsNullOrEmpty(this.IconFilename))
          return null;
        return $"/AssetViewer;component/Resources/{this.IconFilename}";
      }
    }

    #endregion Properties
  }
}