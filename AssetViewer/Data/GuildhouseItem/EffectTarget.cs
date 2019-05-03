using System;
using System.Xml.Linq;
using AssetViewer.Library;

namespace AssetViewer.Data.GuildhouseItem {

  public class EffectTarget {

    #region Properties
    public Int32 GUID { get; set; }
    public Description Description { get; set; }
    #endregion

    #region Constructor
    public EffectTarget() {
      this.Description = new Description(String.Empty, String.Empty);
    }
    public EffectTarget(XElement element) {
      this.GUID = Int32.Parse(element.Element("GUID").Value);
      this.Description = new Description {
        ShortEN = element.Element("Description").Element("EN").Element("Short").Value,
        LongEN = element.Element("Description").Element("EN").Element("Long")?.Value,
        ShortDE = element.Element("Description").Element("DE")?.Element("Short")?.Value,
        LongDE = element.Element("Description").Element("DE")?.Element("Long")?.Value
      };
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      return App.Language == Languages.English ? this.Description.ShortEN : this.Description.ShortDE;
    }
    #endregion

  }

}