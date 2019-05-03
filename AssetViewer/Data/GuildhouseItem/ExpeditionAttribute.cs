using System;
using System.Text;
using System.Xml.Linq;

namespace AssetViewer.Data.GuildhouseItem {

  public class ExpeditionAttribute {

    #region Properties
    public String Name {
      get { return App.GetTranslation("ExpeditionAttribute"); }
    }
    public String Content {
      get { return this.GetContent(); }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public ExpeditionAttribute(XElement element) {
      this.Element = element;
      this.GetContent();
    }
    #endregion

    #region Private Methods
    private String GetContent() {
      if (!this.Element.HasElements) return null;
      var sb = new StringBuilder();
      sb.AppendLine($"     {App.GetTranslation("ItemDifficulties")}: {this.Element.Element("ItemDifficulties")?.Value}");
      sb.AppendLine($"     {App.GetTranslation("BaseMorale")}: {this.Element.Element("BaseMorale")?.Value}");
      if (this.Element.Element("ExpeditionAttributes") != null) {
        foreach (var item in this.Element.Element("ExpeditionAttributes").Elements()) {
          if (!item.HasElements || item.Element("Attribute") == null) continue;
          sb.AppendLine($"     {App.GetTranslation(item.Element("Attribute").Value)}: {item.Element("Amount")?.Value}");
        }
      }
      return sb.ToString();
    }
    #endregion

  }

}