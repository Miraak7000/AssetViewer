using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class Upgrade {

    #region Properties
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public String Value { get; set; }
    public List<Upgrade> Additionals { get; set; }
    #endregion

    #region Constructor
    public Upgrade(XElement item) {
      this.Icon = item.Element("Icon") == null ? null : new Icon(item.Element("Icon"));
      this.Text = new Description(item.Element("Text"));
      this.Value = item.Element("Value").Value;
      if (item.Element("AdditionalOutputs") != null) {
        this.Additionals = item.Element("AdditionalOutputs").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("ReplaceInputs") != null) {
        this.Additionals = item.Element("ReplaceInputs").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("InputAmountUpgrades") != null) {
        this.Additionals = item.Element("InputAmountUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (item.Element("ReplacingWorkforce") != null) {
        this.Additionals = item.Element("ReplacingWorkforce").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }
    #endregion

  }

}