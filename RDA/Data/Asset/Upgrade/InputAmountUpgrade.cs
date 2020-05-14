using System.Xml.Linq;

namespace RDA.Data {

  public class InputAmountUpgrade {

    #region Public Properties

    public Description Text { get; set; }
    public string Value { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public InputAmountUpgrade(XElement element) {
      var id = element.Element("Product").Value;
      Text = new Description(id);
      var value = (int?)int.Parse(element.Element("Amount")?.Value ?? "-1");
      if (value == null) {
        Value = string.Empty;
      }
      else {
        Value = value > 0 ? $"+{value}" : $"{value}";
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("P");
      result.Add(Text.ToXml("T"));
      result.Add(new XAttribute("V", Value));
      return result;
    }

    #endregion Public Methods
  }
}