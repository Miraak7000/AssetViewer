using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class ReplacingWorkforce {

    #region Public Properties

    public Description Text { get; set; }
    public String Value { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public ReplacingWorkforce(String id) {
      this.Text = new Description("20116").Replace("[AssetData([ItemAssetData([RefGuid]) ReplacingWorkforce]) Text]", new Description(id));
      this.Value = String.Empty;
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("RW");
      result.Add(this.Text.ToXml("T"));
      result.Add(new XAttribute("V", this.Value));
      return result;
    }

    #endregion Public Methods
  }
}