using System;
using System.IO;
using System.Xml;

namespace RDA.Data {

  public class Icon {

    #region Properties
    public String Filename { get; set; }
    public Byte[] Value { get; set; }
    #endregion

    #region Constructor
    public Icon(String filename) {
      this.Filename = filename;
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      using (var stringWriter = new StringWriter()) {
        using (var xmlWriter = XmlWriter.Create(stringWriter)) {
          xmlWriter.WriteElementString("Filename", this.Filename);
        }
        return stringWriter.ToString();
      }
    }
    #endregion

  }

}