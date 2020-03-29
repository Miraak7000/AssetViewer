using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Upgrade {

    #region Properties

    public Description Text { get; set; }
    public String Value { get; set; }
    public List<Upgrade> Additionals { get; set; }
    public string Weight { get; set; }

    #endregion Properties

    #region Constructors

    public Upgrade(XElement item) {
      var textItem = item.Name == "T" ? item : item.Element("T");
      if (textItem != null) {
        this.Text = new Description(textItem);
      }
      if (item.Attribute("W") != null) {
        this.Weight = item.Attribute("W").Value;
      }
      if (item.Attribute("V") != null) {
        this.Value = item.Attribute("V")?.Value;
      }
      if (item.Element("AO") != null) {
        this.Additionals = item.Element("AO").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("RI") != null) {
        this.Additionals = item.Element("RI").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("IAU") != null) {
        this.Additionals = item.Element("IAU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("RW") != null) {
        this.Additionals = item.Element("RW").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("A") != null) {
        this.Additionals = item.Element("A").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("DL") != null) {
        this.Additionals = item.Element("DL").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }

    #endregion Constructors
  }
}