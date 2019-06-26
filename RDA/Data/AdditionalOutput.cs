using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class AdditionalOutput {

    #region Properties
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public String Value { get; set; }
    #endregion

    #region Constructor
    public AdditionalOutput(XElement element) {
      var id = element.Element("Product").Value;
      var item = Assets.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={id}]");
      var cycle = element.Element("AdditionalOutputCycle")?.Value;
      var amount = element.Element("Amount")?.Value;
      this.Text = new Description(id);
      if (cycle != null && amount != null) {
        this.Value = $"{cycle} / {amount}";
      } else if (cycle != null) {
        this.Value = $"{cycle}";
      } else if (amount != null) {
        this.Value = $"{amount}";
      } else {
        this.Value = String.Empty;
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement("Product");
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("Value", this.Value));
      return result;
    }
    #endregion

  }

}