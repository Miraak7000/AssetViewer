using System;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace RDA.Data {

  public class Description {

    #region Properties
    public String ID { get; set; }
    public String EN { get; set; }
    public String DE { get; set; }
    public Icon Icon { get; set; }
    public DescriptionFontStyle FontStyle { get; set; }
    public Description AdditionalInformation { get; set; }
    #endregion

    #region Constructor
    public Description(String en, String de, Icon icon = null, Description AdditionalInformation = null, DescriptionFontStyle fontStyle = default) {
      this.ID = String.Empty;
      this.EN = en;
      this.DE = de;
      this.Icon = icon;
      this.AdditionalInformation = AdditionalInformation;
      this.FontStyle = fontStyle;
    }
    public Description(String id, DescriptionFontStyle fontStyle = default) {
      this.ID = id;
      this.EN = Assets.DescriptionEN[id];
      this.DE = Assets.DescriptionDE[id];
      if (Assets.Icons.ContainsKey(id)) {
        this.Icon = new Icon(Assets.Icons[id]);
      }
      this.FontStyle = fontStyle;
    }
    #endregion

    #region Public Methods
    public Description InsertBefore(String en, String de) {
      this.EN = $"{en} {this.EN}";
      this.DE = $"{de} {this.DE}";
      return this;
    }
    public static Description Find(String pattern) {
      var item = Assets.DescriptionEN.First(w => w.Value.StartsWith(pattern));
      return new Description(item.Key);
    }
    public XElement ToXml(String name) {
      var result = new XElement(name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("EN", this.EN));
      result.Add(new XElement("DE", this.DE));
      result.Add(this.Icon == null ? new XElement("Icon") : this.Icon.ToXml());
      if (FontStyle != default) {
        result.Add(new XAttribute("FontStyle", (int)FontStyle));
      }
      if (AdditionalInformation != null) {
        result.Add(AdditionalInformation.ToXml("AdditionalInformation"));
      }
      
      return result;
    }
    public override String ToString() {
      return this.EN;
    }
    #endregion

  }

}