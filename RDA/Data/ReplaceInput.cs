using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class ReplaceInput {

    #region Properties

    public Description Text { get; set; }
    public String Value { get; set; }

    #endregion Properties

    #region Constructors

    public ReplaceInput(XElement element) {
      var oldInput = new Description(element.Element("OldInput").Value);
      var newInput = new Description(element.Element("NewInput").Value);
      this.Text = newInput.InsertBefore("=>").InsertBefore(oldInput);
      this.Value = String.Empty;
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement("Item");
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("Value", this.Value));
      return result;
    }

    #endregion Methods
  }
}