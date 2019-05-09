using System;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace RDA.Data {

  public class Description {

    #region Properties
    public String ID { get; set; }
    public String EN { get; set; }
    public String DE { get; set; }
    #endregion

    #region Constructor
    public Description(String en, String de) {
      this.ID = String.Empty;
      this.EN = en;
      this.DE = de;
    }
    public Description(String id) {
      this.ID = id;
      this.EN = Program.DescriptionEN[id];
      this.DE = Program.DescriptionDE[id];
    }
    #endregion

    #region Public Methods
    public Description InsertBefore(String en, String de) {
      this.EN = $"{en} {this.EN}";
      this.DE = $"{de} {this.DE}";
      return this;
    }
    public static Description Find(String pattern) {
      var item = Program.DescriptionEN.First(w => w.Value.StartsWith(pattern));
      return new Description(item.Key);
    }
    public XElement ToXml(String name) {
      var result = new XElement(name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("EN", this.EN));
      result.Add(new XElement("DE", this.DE));
      return result;
    }
    public override String ToString() {
      return this.EN;
    }
    #endregion

  }

}