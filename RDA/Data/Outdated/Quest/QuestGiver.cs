using System;
using System.Xml.Linq;

namespace RDA.Data {
  public class QuestGiver {
    #region Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }

    #endregion Properties

    #region Constructors

    public QuestGiver(XElement asset) {
      foreach (var element in asset.Element("Values").Elements()) {
        switch (element.Name.LocalName) {
          case "Standard":
            this.ProcessElement_Standard(element);
            break;
        }
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement("QG");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("N", this.Name));
      result.Add(this.Text.ToXml("T"));
      return result;
    }

    private void ProcessElement_Standard(XElement element) {
      this.ID = element.Attribute("ID").Value;
      this.Name = element.Attribute("N").Value;
      this.Text = new Description(element.Element("ID").Value);
    }

    #endregion Methods
  }
}