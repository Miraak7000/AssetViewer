using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class InputAmountUpgrade {

    #region Public Properties

    public Description Text { get; set; }
    public String Value { get; set; }

    #endregion Public Properties

    #region Public Constructors

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

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("P");
      result.Add(this.Text.ToXml("T"));
      result.Add(new XAttribute("V", this.Value));
      return result;
    }

    #endregion Public Methods
  }
}