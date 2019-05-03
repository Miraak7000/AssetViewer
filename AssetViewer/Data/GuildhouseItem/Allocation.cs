using System;
using System.Xml.Linq;
using AssetViewer.Library;

namespace AssetViewer.Data.GuildhouseItem {

  public class Allocation {

    #region Properties
    public Int32 GUID { get; set; }
    public Description Description { get; set; }
    #endregion

    #region Constructor
    public Allocation() {
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      return App.Language == Languages.English ? this.Description.ShortEN : this.Description.ShortDE;
    }
    #endregion

  }

}