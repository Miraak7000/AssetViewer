using System;
using System.Xml.Linq;

namespace RDA.Data {

  public class Quest {

    #region Properties
    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public String QuestGiver { get; set; }
    #endregion

    #region Constructor
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
            this.ProcessElement_Standard(element);
            break;
          case "Quest":
            this.ProcessElement_Quest(element);
            break;
          case "Reward":
            if (element.HasElements) throw new ArgumentOutOfRangeException();
            break;
          default:
            throw new NotImplementedException(element.Name.LocalName);
        }
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("Name", this.Name));
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("QuestGiver", this.QuestGiver));
      return result;
    }
    #endregion

    #region Private Methods
    private void ProcessElement_Standard(XElement element) {
      this.ID = element.Element("GUID").Value;
      this.Name = element.Element("Name").Value;
      this.Text = new Description(element.Element("GUID").Value);
    }

    private void ProcessElement_Quest(XElement element) {
      this.QuestGiver = element.Element("QuestGiver").Value;
    }
    #endregion

  }

}