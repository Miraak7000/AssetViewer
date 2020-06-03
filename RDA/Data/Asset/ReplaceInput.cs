using System.Xml.Linq;

namespace RDA.Data {

  public class ReplaceInput {

    #region Public Properties

    public Description Text { get; set; }
    public string Value { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public ReplaceInput(XElement element) {
      var oldInput = new Description(element.Element("OldInput").Value);
      var newInput = new Description(element.Element("NewInput").Value);
      Text = newInput.InsertBefore("=>").InsertBefore(oldInput);
      Value = string.Empty;
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("RI");
      result.Add(Text.ToXml("T"));
      result.Add(new XAttribute("V", Value));
      return result;
    }

    #endregion Public Methods
  }
}