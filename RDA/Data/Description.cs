using System;
using System.IO;
using System.Xml;

namespace RDA.Data {

  public class Description {

    #region Properties
    public String ID { get; set; }
    public String EN { get; set; }
    public String DE { get; set; }
    #endregion

    #region Constructor
    public Description(String id) {
      this.ID = id;
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      using (var stringWriter = new StringWriter()) {
        using (var xmlWriter = XmlWriter.Create(stringWriter)) {
          xmlWriter.WriteStartElement("Description");
          xmlWriter.WriteAttributeString("ID", this.ID);
          xmlWriter.WriteElementString("EN", this.EN);
          xmlWriter.WriteElementString("DE", this.DE);
          xmlWriter.WriteEndElement();
        }
        return stringWriter.ToString();
      }
    }
    #endregion

  }

}