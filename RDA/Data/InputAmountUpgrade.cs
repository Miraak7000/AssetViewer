using System;
using System.Xml.Linq;

namespace RDA.Data {
  public class InputAmountUpgrade {
    #region Properties

    public Description Text { get; set; }
    public String Value { get; set; }

    #endregion Properties

    #region Constructors

    public InputAmountUpgrade(XElement element) {
      var id = element.Element("Product").Value;
      this.Text = new Description(id);
      var value = (Int32?)Int32.Parse(element.Element("Amount")?.Value ?? "-1");
      if (value == null) {
        this.Value = String.Empty;
      }
      else {
        this.Value = value > 0 ? $"+{value}" : $"{value}";
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement("Product");
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("Value", this.Value));
      return result;
    }

    #endregion Methods
  }
}