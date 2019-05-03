using System;
using System.Collections.Generic;
using AssetViewer.Library;

namespace AssetViewer.Data {

  public abstract class Asset {

    #region Properties
    public Int32 GUID { get; set; }
    public Description Description { get; set; }
    #endregion

    #region Public Methods
    public override String ToString() {
      return App.Language == Languages.English ? $"{this.GUID} - {this.Description.ShortEN}" : $"{this.GUID} - {this.Description.ShortDE}";
    }
    #endregion

  }

}