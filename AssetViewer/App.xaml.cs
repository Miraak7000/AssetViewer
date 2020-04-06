using AssetViewer.Comparer;
using AssetViewer.Data;
using System;
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

    public static Dictionary<int, string> Descriptions { get; } = new Dictionary<int, string>();
    public static bool CountMode { get; set; }
    #endregion Properties

    #region Fields

    public static Data.Languages Language { get; set; } = Data.Languages.English;
    public static List<Languages> PossibleLanguages { get; } = new List<Languages>();

    #endregion Fields

    #region Constructors

    public App() {
      var comp = RarityComparer.Default;

      foreach (var language in Enum.GetValues(typeof(Languages))) {
        var lang = (Languages)language;
        var resource = $"AssetViewer.Resources.Assets.Texts_{lang.ToString("G")}.xml";
        if (Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(resource)) {
          PossibleLanguages.Add(lang);
        }
      }
      switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName) {
        case "pt":
          Language = Data.Languages.Brazilian;
          break;

        case "zh":
          Language = Data.Languages.Chinese;
          break;

        case "en":
          Language = Data.Languages.English;
          break;

        case "fr":
          Language = Data.Languages.French;
          break;

        case "de":
          Language = Data.Languages.German;
          break;

        case "it":
          Language = Data.Languages.Italian;
          break;

        case "ja":
          Language = Data.Languages.Japanese;
          break;

        case "ko":
          Language = Data.Languages.Korean;
          break;

        case "pl":
          Language = Data.Languages.Polish;
          break;

        case "ru":
          Language = Data.Languages.Russian;
          break;

        case "es":
          Language = Data.Languages.Spanish;
          break;
        //case "pt": Language = Library.Languages.Portuguese; break;
        //case "zh	": Language = Library.Languages.Taiwanese; break;
        default:
          Language = Data.Languages.English;
          break;
      }

      LoadLanguageFile();
    }

    public static void LoadLanguageFile() {
      Descriptions.Clear();
      var resource = $"AssetViewer.Resources.Assets.Texts_{Language.ToString("G")}.xml";
      if (!Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(resource)) {
        Language = Data.Languages.English;
        resource = $"AssetViewer.Resources.Assets.Texts_{Language.ToString("G")}.xml";
      }

      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
      using (var reader = new StreamReader(stream)) {
        var document = XDocument.Parse(reader.ReadToEnd()).Root;
        foreach (var item in document.Elements()) {
          Descriptions.Add(int.Parse(item.Attribute("ID").Value), item.Value);
        }
      }
    }
  }

  #endregion Constructors
}