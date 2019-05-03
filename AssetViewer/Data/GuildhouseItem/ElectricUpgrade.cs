using System;
using System.Text;
using System.Xml.Linq;

namespace AssetViewer.Data.GuildhouseItem {

  public class ElectricUpgrade {

    #region Properties
    public String Name {
      get { return App.GetTranslation("ElectricUpgrade"); }
    }
    public String Content {
      get { return this.GetContent(); }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public ElectricUpgrade(XElement element) {
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
          case "ProvideElectricity":
            if (item.Value == "1") sb.AppendLine($"     {App.GetTranslation(item.Name.LocalName)}");
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