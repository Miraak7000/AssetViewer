﻿using System.Xml.Linq;

namespace RDA.Data {

  public class AdditionalOutput {

    #region Public Properties

    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public string Value { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public AdditionalOutput(XElement element) {
      var id = element.Element("Product").Value;
      var cycle = element.Element("AdditionalOutputCycle")?.Value;
      var amount = element.Element("Amount")?.Value;
      Text = new Description(id);
      if (cycle != null && amount != null) {
        Value = $"{cycle} / {amount}";
      }
      else if (cycle != null) {
        Value = $"{cycle}";
      }
      else if (amount != null) {
        Value = $"{amount}";
      }
      else {
        Value = string.Empty;
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("AO");
      result.Add(Text.ToXml("T"));
      result.Add(new XAttribute("V", Value));
      return result;
    }

    #endregion Public Methods
  }
}