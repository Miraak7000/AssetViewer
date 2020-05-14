using System.Xml.Linq;

namespace RDA.Data {

  public class QuestGiver {

    #region Public Properties

    public string ID { get; set; }
    public string Name { get; set; }
    public Description Text { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public QuestGiver(XElement asset) {
      foreach (var element in asset.Element("Values").Elements()) {
        switch (element.Name.LocalName) {
          case "Standard":
            ProcessElement_Standard(element);
            break;
        }
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("QG");
      result.Add(new XAttribute("ID", ID));
      result.Add(new XElement("N", Name));
      result.Add(Text.ToXml("T"));
      return result;
    }

    #endregion Public Methods

    #region Private Methods

    private void ProcessElement_Standard(XElement element) {
      ID = element.Attribute("ID").Value;
      Name = element.Attribute("N").Value;
      Text = new Description(element.Element("ID").Value);
    }

    #endregion Private Methods
  }
}