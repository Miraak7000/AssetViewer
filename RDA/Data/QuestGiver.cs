using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class QuestGiver {

    #region Properties
    public String ID { get; set; }
    public String Name { get; set; }
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    #endregion

    #region Constructor
    public QuestGiver(XElement asset) {
      foreach (var element in asset.Element("Values").Elements()) {
        switch (element.Name.LocalName) {
          case "Standard":
            this.ProcessElement_Standard(element);
            break;
        }
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("Name", this.Name));
      result.Add(this.Icon.ToXml());
      result.Add(this.Text.ToXml("Text"));
      return result;
    }
    #endregion

    #region Private Methods
    private void ProcessElement_Standard(XElement element) {
      this.ID = element.Element("GUID").Value;
      this.Name = element.Element("Name").Value;
      this.Icon = new Icon(element.Element("IconFilename").Value);
      this.Text = new Description(element.Element("GUID").Value);
    }
    #endregion

  }

}