using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class ReplacingWorkforce {

    #region Properties
    public Description Text { get; set; }
    public String Value { get; set; }
    #endregion

    #region Constructor
    public ReplacingWorkforce(String id) {
      var en = "Instead of its usual workforce, the building employs";
      var de = "Statt der üblichen Arbeitskräfte beschäftigt das Gebäude";
      this.Text = new Description($"{en} {Assets.DescriptionEN[id]}", $"{de} {Assets.DescriptionDE[id]}");
      this.Value = String.Empty;
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement("Workforce");
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("Value", this.Value));
      return result;
    }
    #endregion

  }

}