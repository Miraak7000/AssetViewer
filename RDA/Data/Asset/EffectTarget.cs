using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class EffectTarget {

    #region Public Properties

    public Description Text { get; set; }
    public List<Description> Buildings { get; set; } = new List<Description>();

    #endregion Public Properties

    #region Public Constructors

    public EffectTarget(XElement element, GameTypes gameType) {
      Text = new Description(element.Value, gameType);
      var asset = Assets.GUIDs[element.Value, gameType];
      //Building

      //BuidlingPool
      var buildings = asset
         .XPathSelectElement("Values/ItemEffectTargetPool/EffectTargetGUIDs")?
         .Descendants("GUID");
      if (buildings != null) {
        Buildings = buildings
        .Where(a => Assets.Descriptions.ContainsKey(a.Value))
        .Select(a => new Description(a.Value, gameType))
        .Where(a=> a.Languages.Any())
        .ToList();
      }
      else {
        Buildings.Add(new Description(asset));
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("ET");
      result.Add(Text.ToXml("T"));
      result.Add(new XElement("B", Buildings.Select(b => b.ToXml("T"))));
      return result;
    }

    #endregion Public Methods
  }
}