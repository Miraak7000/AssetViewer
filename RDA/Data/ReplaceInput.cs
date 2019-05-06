using System.Xml.Linq;

namespace RDA.Data {

  public class ReplaceInput {

    #region Properties
    public Description Old { get; set; }
    public Description New { get; set; }
    #endregion

    #region Constructor
    public ReplaceInput(XElement element) {
      this.Old = new Description(element.Element("OldInput").Value);
      this.New = new Description(element.Element("NewInput").Value);
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement("Item");
      result.Add(this.Old.ToXml("Old"));
      result.Add(this.New.ToXml("New"));
      return result;
    }
    #endregion

  }

}