using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class EffectTarget {

    #region Properties

    public Description Text { get; set; }
    public List<Description> Buildings { get; set; }

    #endregion Properties

    #region Constructors

    public EffectTarget(XElement element) {
      Text = new Description(element.Element("T"));
      Buildings = element.Element("B").Elements().Select(b => new Description(b)).ToList();
    }

    #endregion Constructors
  }
}