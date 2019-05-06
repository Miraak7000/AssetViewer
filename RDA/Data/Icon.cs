using System;
using System.IO;
using System.Xml.Linq;

namespace RDA.Data {

  public class Icon {

    #region Properties
    public String Filename { get; set; }
    #endregion

    #region Constructor
    public Icon(String filename) {
      var searchPath = Path.GetDirectoryName($@"{Program.PathRoot}\Resources\{filename}");
      var searchPattern = Path.GetFileNameWithoutExtension($@"{Program.PathRoot}\Resources\{filename}");
      var fileNames = Directory.GetFiles(searchPath, $"{searchPattern}??.png", SearchOption.TopDirectoryOnly);
      if (fileNames.Length != 1) throw new FileNotFoundException();
      this.Filename = filename;
      var file = File.ReadAllBytes(fileNames[0]);
      // publish icon
      var targetPath = Path.GetDirectoryName($@"{Program.PathViewer}\Resources\{filename}");
      var targetFile = Path.GetFullPath($@"{Program.PathViewer}\Resources\{filename}");
      if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
      File.WriteAllBytes(targetFile, file);
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