using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class AdditionalOutput {

    #region Properties

    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public String Value { get; set; }

    #endregion Properties

    #region Constructors

    public AdditionalOutput(XElement element) {
      var id = element.Element("Product").Value;
      var cycle = element.Element("AdditionalOutputCycle")?.Value;
      var amount = element.Element("Amount")?.Value;
      this.Text = new Description(id);
      if (cycle != null && amount != null) {
        this.Value = $"{cycle} / {amount}";
      }
      else if (cycle != null) {
        this.Value = $"{cycle}";
      }
      else if (amount != null) {
        this.Value = $"{amount}";
      }
      else {
        this.Value = String.Empty;
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