using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;

namespace RDA.Templates {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public class ItemSet {

    #region Properties
    public String ID { get; set; }
    public String Name { get; set; }
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public Description Info { get; set; }
    #endregion

    #region Constructor
    public ItemSet(XElement asset) {
      this.ID = asset.XPathSelectElement("Values/Standard/GUID").Value;
      this.Name = asset.XPathSelectElement("Values/Standard/Name").Value;
      this.Text = new Description(asset.XPathSelectElement("Values/Standard/GUID").Value);
      this.Info = new Description(asset.XPathSelectElement("Values/Standard/InfoDescription").Value);
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      using (var stringWriter = new StringWriter()) {
        using (var xmlWriter = XmlWriter.Create(stringWriter)) {
          xmlWriter.WriteStartElement("Asset");
          xmlWriter.WriteAttributeString("ID", this.ID);
          xmlWriter.WriteElementString("Template", this.GetType().Name);
          xmlWriter.WriteElementString("Name", this.Name);
          xmlWriter.WriteElementString("Icon", this.Icon.Filename);
          xmlWriter.WriteElementString("Text", this.Text.ToString());
          xmlWriter.WriteElementString("Info", this.Info.ToString());
          xmlWriter.WriteEndElement();
        }
        return stringWriter.ToString();
      }
    }
    #endregion

  }

}