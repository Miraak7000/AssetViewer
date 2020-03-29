using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class AdditionalOutput {

    #region Public Properties

    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public String Value { get; set; }

    #endregion Public Properties

    #region Public Constructors

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

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("AO");
      result.Add(this.Text.ToXml("T"));
      result.Add(new XAttribute("V", this.Value));
      return result;
    }

    #endregion Public Methods
  }
}