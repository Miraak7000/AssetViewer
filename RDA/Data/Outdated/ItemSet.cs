using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public class ItemSet {

    #region Public Properties

    public string ID { get; set; }
    public string Name { get; set; }
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public Description Info { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public ItemSet(XElement asset) {
      ID = asset.XPathSelectElement("Values/Standard/GUID").Value;
      Name = asset.XPathSelectElement("Values/Standard/Name").Value;
      Text = new Description(asset);
      Info = new Description(asset.XPathSelectElement("Values/Standard/InfoDescription").Value);
    }

    #endregion Public Constructors

    #region Public Methods

    public override string ToString() {
      using (var stringWriter = new StringWriter()) {
        using (var xmlWriter = XmlWriter.Create(stringWriter)) {
          xmlWriter.WriteStartElement("Asset");
          xmlWriter.WriteAttributeString("ID", ID);
          xmlWriter.WriteElementString("Template", GetType().Name);
          xmlWriter.WriteElementString("Name", Name);
          xmlWriter.WriteElementString("Icon", Icon.Filename);
          xmlWriter.WriteElementString("Text", Text.ToString());
          xmlWriter.WriteElementString("Info", Info.ToString());
          xmlWriter.WriteEndElement();
        }
        return stringWriter.ToString();
      }
    }

    #endregion Public Methods
  }
}