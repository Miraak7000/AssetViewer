using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class ReplaceInput {

    #region Properties
    public Description Text { get; set; }
    public String Value { get; set; }
    #endregion

    #region Constructor
    public ReplaceInput(XElement element) {
      var oldInput = new Description(element.Element("OldInput").Value);
      var newInput = new Description(element.Element("NewInput").Value);
      this.Text = new Description($"{oldInput.EN} => {newInput.EN}", $"{oldInput.DE} => {newInput.DE}");
      this.Value = String.Empty;
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement("Item");
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("Value", this.Value));
      return result;
    }
    #endregion

  }

}