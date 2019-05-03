using System;
using System.Text;
using System.Xml.Linq;

namespace AssetViewer.Data.GuildhouseItem {

  public class ResidenceUpgrade {

    #region Properties
    public String Name {
      get { return App.GetTranslation("ResidenceUpgrade"); }
    }
    public String Content {
      get { return this.GetContent(); }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public ResidenceUpgrade(XElement element) {
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
          case "AdditionalHappiness":
            sb.AppendLine($"     {App.GetTranslation(item.Name.LocalName)}: {item.Value}");
            break;
          case "NeedProvideNeedUpgrade":
            // ignore
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