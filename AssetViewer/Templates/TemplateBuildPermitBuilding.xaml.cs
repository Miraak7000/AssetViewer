using System;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Data;

namespace AssetViewer.Templates {

  public partial class TemplateBuildPermitBuilding : UserControl {

    #region Properties
    public String Icon {
      get {
        return $"/AssetViewer;component/Resources/{this.Asset.XPathSelectElement("Values/Standard/IconFilename").Value}";
      }
    }
    public XElement Description {
      get { return this.Asset.XPathSelectElement("Values/Description"); }
    }
    #endregion

    private readonly XElement Asset;

    #region Constructor
    public TemplateBuildPermitBuilding() {
      this.InitializeComponent();
    }
    public TemplateBuildPermitBuilding(XElement asset) {
      this.InitializeComponent();
      this.Asset = asset;
      this.DataContext = this;
    }
    #endregion

  }

}