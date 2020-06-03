using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Allocation {

    #region Public Properties

    public string ID { get; set; }
    public Description Text { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Allocation(XElement item) {
      ID = item.Attribute("ID")?.Value;
      Text = new Description(item.Element("T"));
    }

    #endregion Public Constructors
  }
}