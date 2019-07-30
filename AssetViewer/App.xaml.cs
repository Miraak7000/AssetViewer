using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace AssetViewer {

  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public partial class App : Application {

    #region Properties

    public static Dictionary<string, string> Descriptions { get; } = new Dictionary<string, string>();
   
    #endregion Properties

    #region Fields

    public static Library.Languages Language = Library.Languages.English;

    #endregion Fields

    #region Constructors

    public App() {
      switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName) {
        case "pt":
          Language = Library.Languages.Brazilian;
          break;

        case "zh":
          Language = Library.Languages.Chinese;
          break;

        case "en":
          Language = Library.Languages.English;
          break;

        case "fr":
          Language = Library.Languages.French;
          break;

        case "de":
          Language = Library.Languages.German;
          break;

        case "it":
          Language = Library.Languages.Italian;
          break;

        case "ja":
          Language = Library.Languages.Japanese;
          break;

        case "ko":
          Language = Library.Languages.Korean;
          break;

        case "pl":
          Language = Library.Languages.Polish;
          break;

        case "ru":
          Language = Library.Languages.Russian;
          break;

        case "es":
          Language = Library.Languages.Spanish;
          break;
        //case "pt": Language = Library.Languages.Portuguese; break;
        //case "zh	": Language = Library.Languages.Taiwanese; break;
        default:
          Language = Library.Languages.English;
          break;
      }

      LoadLanguageFile();
    }

    public static void LoadLanguageFile() {
      Descriptions.Clear();
      var resource = $"AssetViewer.Resources.Assets.Texts_{Language.ToString("G")}.xml";
      if (!Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(resource)) {
        Language = Library.Languages.English;
        resource = $"AssetViewer.Resources.Assets.Texts_{Language.ToString("G")}.xml";
      }

      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements()) {
          Descriptions.Add(item.Attribute("ID").Value, item.Value);
        }
      }
    }
  }

  #endregion Constructors
}