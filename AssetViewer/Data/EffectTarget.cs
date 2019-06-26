using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AssetViewer.Data {

  public class EffectTarget {

    #region Properties

    public Description Text { get; set; }
    public List<Description> Buildings { get; set; } = new List<Description>();

    #endregion Properties

    #region Constructors

    public EffectTarget(XElement element) {
      Text = new Description(element.Element("Text"));
      Buildings = element.Element("Buildings").Elements().Select(b => new Description(b)).ToList();
    }

    #endregion Constructors

    #region Methods


    #endregion Methods
  }
}