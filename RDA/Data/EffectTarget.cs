using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {
  public class EffectTarget {
    #region Properties

    public Description Text { get; set; }
    public List<Description> Buildings { get; set; } = new List<Description>();

    #endregion Properties

    #region Constructors

    public EffectTarget(XElement element) {
      Text = new Description(element.Value);
      var asset = Assets.Original.Descendants("Asset").FirstOrDefault(a => a
         .XPathSelectElement("Values/Standard/GUID")?
         .Value == element.Value);
      //Building

      //BuidlingPool
      var buildings = asset
         .XPathSelectElement("Values/ItemEffectTargetPool/EffectTargetGUIDs")?
         .Descendants("GUID");
      if (buildings != null) {
        Buildings = buildings
        .Where(a => Assets.Descriptions.ContainsKey(a.Value))
        .Select(a => new Description(a.Value))
        .ToList();
      }
      else {
        Buildings.Add(new Description(asset.XPathSelectElement("Values/Standard/GUID").Value));
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement("Target");
      result.Add(Text.ToXml("Text"));
      result.Add(new XElement("Buildings", Buildings.Select(b => b.ToXml("Text"))));
      return result;
    }

    #endregion Methods
  }
}