using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Upgrade {

    #region Public Properties

    public Description Text { get; set; }
    public string Value { get; set; }
    public List<Upgrade> Additionals { get; set; }
    public string Weight { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Upgrade(XElement item) {
      var textItem = item.Name == "T" ? item : item.Element("T");
      if (textItem != null) {
        Text = new Description(textItem);
      }
      if (item.Attribute("W") != null) {
        Weight = item.Attribute("W").Value;
      }
      if (item.Attribute("V") != null) {
        Value = item.Attribute("V")?.Value;
      }
      if (item.Element("AO") != null) {
        Additionals = item.Element("AO").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("RI") != null) {
        Additionals = item.Element("RI").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("IAU") != null) {
        Additionals = item.Element("IAU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("RW") != null) {
        Additionals = item.Element("RW").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("A") != null) {
        Additionals = item.Element("A").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("DL") != null) {
        Additionals = item.Element("DL").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }

    #endregion Public Constructors
  }
}