using System.Xml.Linq;

namespace RDA.Data {

  public class ReplacingWorkforce {

    #region Public Properties

    public Description Text { get; set; }
    public string Value { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public ReplacingWorkforce(string id, GameTypes gameType) {
      Text = new Description("20116", gameType).Replace("[AssetData([ItemAssetData([RefGuid]) ReplacingWorkforce]) Text]", new Description(id, gameType));
      Value = string.Empty;
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("RW");
      result.Add(Text.ToXml("T"));
      result.Add(new XAttribute("V", Value));
      return result;
    }

    #endregion Public Methods
  }
}