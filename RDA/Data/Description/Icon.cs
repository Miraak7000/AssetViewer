using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {

  public class Icon {

    #region Public Properties

    public static string[] IgnoredDirectorys { get; set; } = new string[] {
      $@"{Program.PathRoot}\Resources\data\level_editor\random_slots_icons",
      //$@"{Program.PathRoot}\Resources\data\ui\2kimages\main\3dicons\Temporary_Ornament",
      //$@"{Program.PathRoot}\Resources\data\ui\2kimages\main\3dicons\ornaments\preorder_ornament_s03"
    };

    public string Filename { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Icon(string filename) {
      var searchPath = Path.GetDirectoryName($@"{Program.PathRoot}\Resources\{filename}");
      var searchPattern = Path.GetFileNameWithoutExtension($@"{Program.PathRoot}\Resources\{filename}");
      if (IgnoredDirectorys.Contains(searchPath)) {
        return;
      }
      var fileNames = Directory.GetFiles(searchPath, $"{searchPattern}??.png", SearchOption.TopDirectoryOnly);
      if (fileNames.Length == 0) {
        Debug.WriteLine($"Picture Missing: {searchPath} {searchPattern}");
        return;
      }
      Filename = filename;
      var file = File.ReadAllBytes(fileNames[0]);
      // publish icon
      var targetPath = Path.GetDirectoryName($@"{Program.PathViewer}\Resources\{filename}");
      var targetFile = Path.GetFullPath($@"{Program.PathViewer}\Resources\{filename}");
      if (!Directory.Exists(targetPath))
        Directory.CreateDirectory(targetPath);
      if (!File.Exists(targetFile)) {
        try {
          File.WriteAllBytes(targetFile, file);
        }
        catch (Exception) { }
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("I");
      result.Add(new XAttribute("F", Filename));
      return result;
    }

    #endregion Public Methods
  }
}