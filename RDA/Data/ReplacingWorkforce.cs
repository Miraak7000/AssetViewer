using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class ReplacingWorkforce {

    #region Properties

    public Description Text { get; set; }
    public String Value { get; set; }

    #endregion Properties

    #region Constructors

    public ReplacingWorkforce(String id) {
      this.Text = new Description("20116").Replace("[AssetData([ItemAssetData([RefGuid]) ReplacingWorkforce]) Text]", new Description(id));
      this.Value = String.Empty;
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement("Workforce");
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("Value", this.Value));
      return result;
    }

    #endregion Methods
  }
}