using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class InputAmountUpgrade {

    #region Properties
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public String Value { get; set; }
    #endregion

    #region Constructor
    public InputAmountUpgrade(XElement element) {
      var id = element.Element("Product").Value;
      var item = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={id}]");
      this.Icon = new Icon(item.Element("IconFilename").Value);
      this.Text = new Description(id);
      var value = element.Element("Amount") == null ? null : (Int32?)Int32.Parse(element.Element("Amount").Value);
      if (value == null) {
        this.Value = String.Empty;
      } else {
        this.Value = value > 0 ? $"+{value}" : $"{value}";
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement("Product");
      result.Add(this.Icon.ToXml());
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("Value", this.Value));
      return result;
    }
    #endregion

  }

}