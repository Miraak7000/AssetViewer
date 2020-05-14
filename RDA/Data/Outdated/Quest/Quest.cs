using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class Quest {

    #region Public Properties

    public string ID { get; set; }
    public string Name { get; set; }
    public Description Text { get; set; }
    public string QuestGiver { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Quest(XElement asset) {
      foreach (var element in asset.Element("Values").Elements()) {
        switch (element.Name.LocalName) {
          case "PreConditionList":
          case "Text":
          case "Objectives":
          case "QuestOptional":
            // ignore this nodes
            break;

          case "Standard":
            ProcessElement_Standard(element);
            break;

          case "Quest":
            ProcessElement_Quest(element);
            break;

          case "Reward":
            if (element.HasElements)
              throw new ArgumentOutOfRangeException();
            break;

          default:
            throw new NotImplementedException(element.Name.LocalName);
        }
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("Q");
      result.Add(new XAttribute("ID", ID));
      result.Add(new XAttribute("N", Name));
      result.Add(Text.ToXml("T"));
      result.Add(new XAttribute("QG", QuestGiver));
      return result;
    }

    #endregion Public Methods

    #region Private Methods

    private void ProcessElement_Standard(XElement element) {
      ID = element.Element("GUID").Value;
      Name = element.Element("Name").Value;
      Text = new Description(element.Element("GUID").Value);
    }

    private void ProcessElement_Quest(XElement element) {
      QuestGiver = element.Element("QuestGiver").Value;
    }

    #endregion Private Methods
  }
}