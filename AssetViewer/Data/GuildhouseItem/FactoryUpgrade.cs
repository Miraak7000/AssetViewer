using System;
using System.Text;
using System.Xml.Linq;

namespace AssetViewer.Data.GuildhouseItem {

  public class FactoryUpgrade {

    #region Properties
    public String Name {
      get { return App.GetTranslation("FactoryUpgrade"); }
    }
    public String Content {
      get { return this.GetContent(); }
    }
    #endregion

    #region Fields
    private readonly XElement Element;
    #endregion

    #region Constructor
    public FactoryUpgrade(XElement element) {
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
          case "AdditionalOutput":
            sb.AppendLine($"     {App.GetTranslation(item.Name.LocalName)}:");
            foreach (var product in item.Elements()) {
              sb.AppendLine($"          {App.GetTranslation(product.Element("Product").Element("Description"))} / {App.GetTranslation("Cycle")}: {product.Element("AdditionalOutputCycle").Value} / {App.GetTranslation("Amount")}: {product.Element("Amount").Value}");
            }
            break;
          case "InputAmountUpgrade":
            sb.AppendLine($"     {App.GetTranslation(item.Name.LocalName)}:");
            foreach (var product in item.Elements()) {
              sb.AppendLine($"          Product: {product.Element("Product").Value} / Amount: {product.Element("Amount")?.Value}");
            }
            break;
          case "NeededAreaPercentUpgrade":
            sb.AppendLine($"     {App.GetTranslation(item.Name.LocalName)}: {item.Element("Value").Value}%");
            break;
          case "OutputAmountFactorUpgrade":
          case "ProductivityUpgrade":
            var text = String.Empty;
            text = $"     {App.GetTranslation(item.Name.LocalName)}: {item.Element("Value").Value}";
            if (item.Element("Percental")?.Value == "1") text += "%";
            sb.AppendLine(text);
            break;
          case "AddedFertility":
            sb.AppendLine($"     {App.GetTranslation(item.Name.LocalName)}: {App.GetTranslation(item.Element("Description"))}");
            break;
          case "ReplaceInputs":
            sb.AppendLine($"     {App.GetTranslation(item.Name.LocalName)}:");
            sb.AppendLine($"          {App.GetTranslation("Old")}: {App.GetTranslation(item.Element("Item").Element("OldInput").Element("Description"))}");
            sb.AppendLine($"          {App.GetTranslation("New")}: {App.GetTranslation(item.Element("Item").Element("NewInput").Element("Description"))}");
            break;
          case "NeedsElectricity":
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