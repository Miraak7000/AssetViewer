using System;
using System.IO;
using System.Xml.Linq;

namespace RDA.Data {

  public class Icon {

    #region Properties
    public String Filename { get; set; }
    public Byte[] Value { get; set; }
    #endregion

    #region Constructor
    public Icon(String filename) {
      var searchPath = Path.GetDirectoryName($@"{Program.PathRoot}\Resources\{filename}");
      var searchPattern = Path.GetFileNameWithoutExtension($@"{Program.PathRoot}\Resources\{filename}");
      var fileNames = Directory.GetFiles(searchPath, $"{searchPattern}??.png", SearchOption.TopDirectoryOnly);
      if (fileNames.Length != 1) throw new FileNotFoundException();
      this.Filename = filename;
      this.Value = File.ReadAllBytes(fileNames[0]);
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XElement("Filename", this.Filename));
      return result;
    }
    #endregion

  }

}