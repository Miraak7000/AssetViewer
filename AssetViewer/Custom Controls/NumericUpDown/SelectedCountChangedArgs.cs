using AssetViewer.Data;
using System.Collections.Generic;

namespace AssetViewer {

  public class SelectedCountChangedArgs {

    #region Public Properties

    public uint Count { get; set; }
    public IEnumerable<TemplateAsset> Assets { get; set; }

    #endregion Public Properties
  }
}