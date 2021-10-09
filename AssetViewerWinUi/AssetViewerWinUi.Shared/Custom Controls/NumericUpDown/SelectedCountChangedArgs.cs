using System.Collections.Generic;
using AssetViewer.Data;

namespace AssetViewer {

  public class SelectedCountChangedArgs {

    #region Public Properties

    public uint Count { get; set; }
    public IEnumerable<TemplateAsset> Assets { get; set; }

    #endregion Public Properties
  }
}