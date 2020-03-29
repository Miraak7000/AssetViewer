using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class ReplaceInput {

    #region Public Properties

    public Description Text { get; set; }
    public String Value { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public ReplaceInput(XElement element) {
      var oldInput = new Description(element.Element("OldInput").Value);
      var newInput = new Description(element.Element("NewInput").Value);
      this.Text = newInput.InsertBefore("=>").InsertBefore(oldInput);
      this.Value = String.Empty;
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("RI");
      result.Add(this.Text.ToXml("T"));
      result.Add(new XAttribute("V", this.Value));
      return result;
    }

    #endregion Public Methods
  }
}