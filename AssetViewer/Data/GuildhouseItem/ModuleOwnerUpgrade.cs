using System;
using System.Text;
using System.Xml.Linq;

namespace AssetViewer.Data.GuildhouseItem {

  public class ModuleOwnerUpgrade {

    #region Properties
    public String Name {
      get { return App.GetTranslation("ModuleOwnerUpgrade"); }
    }
    public String Content {
      get { return this.GetContent(); }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public ModuleOwnerUpgrade(XElement element) {
      this.Element = element;
      this.GetContent();
    }
    #endregion

    #region Private Methods
    private String GetContent() {
      if (!this.Element.HasElements) return null;
      var sb = new StringBuilder();
      foreach (var item in this.Element.Elements()) {
        switch (item.Name.LocalName) {
          case "ModuleLimitUpgrade":
            var text = String.Empty;
            text = $"     {App.GetTranslation(item.Name.LocalName)}: {item.Element("Value").Value}";
            if (item.Element("Percental")?.Value == "1") text += "%";
            sb.AppendLine(text);
            break;
          default:
            throw new NotImplementedException();
        }
      }
      return sb.ToString();
    }
    #endregion

  }

}