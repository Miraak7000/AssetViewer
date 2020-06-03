using System.Xml.Linq;

namespace RDA.Data {

  public class AssetWithWeight {

    #region Public Properties

    public XElement Asset { get; set; }

    public double Weight { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public AssetWithWeight(XElement asset, double weight = 1.0F) {
      Asset = asset;
      Weight = weight;
    }

    #endregion Public Constructors

    #region Public Methods

    public AssetWithWeight Copy() {
      return new AssetWithWeight(Asset, Weight);
    }

    #endregion Public Methods
  }
}