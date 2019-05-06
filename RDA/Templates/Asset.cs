using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;

namespace RDA.Templates {

  public class Asset {

    #region Properties
    public String ID { get; set; }
    public String Name { get; set; }
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public Description Rarity { get; set; }
    public String ItemType { get; set; }
    //
    public Allocation Allocation { get; set; }
    //
    public List<Description> EffectTargets { get; set; }
    //
    public List<Upgrade> FactoryUpgrades { get; set; }
    public List<Upgrade> ExpeditionAttributes { get; set; }
    //
    public String TradePrice { get; set; }
    //
    public Description Info { get; set; }
    #endregion

    #region Constructor
    public Asset(XElement asset) {
      this.ID = asset.XPathSelectElement("Values/Standard/GUID").Value;
      this.Name = asset.XPathSelectElement("Values/Standard/Name").Value;
      this.Icon = new Icon(asset.XPathSelectElement("Values/Standard/IconFilename").Value);
      this.Text = new Description(asset.XPathSelectElement("Values/Standard/GUID").Value);
      if (asset.XPathSelectElement("Values/Item/Rarity") == null) {
        this.Rarity = new Description("118002");
      } else {
        this.Rarity = new Description(Helper.GetDescriptionID(asset.XPathSelectElement("Values/Item/Rarity").Value));
      }
      this.ItemType = asset.XPathSelectElement("Values/Item/ItemType")?.Value ?? "Common";
      if (this.ItemType != "Specialist" && this.ItemType != "Common") throw new NotImplementedException();
      //
      this.Allocation = new Allocation(asset.XPathSelectElement("Values/Item/Allocation")?.Value);
      //
      if (asset.XPathSelectElement("Values/ItemEffect").HasElements && asset.XPathSelectElement("Values/ItemEffect/EffectTargets") == null) throw new NotImplementedException();
      if (asset.XPathSelectElement("Values/ItemEffect").HasElements && asset.XPathSelectElement("Values/ItemEffect/EffectTargets").HasElements) {
        this.EffectTargets = new List<Description>();
        foreach (var item in asset.XPathSelectElements("Values/ItemEffect/EffectTargets/Item")) {
          this.EffectTargets.Add(new Description(Program.DescriptionEN[item.Value], Program.DescriptionDE[item.Value]));
        }
      }
      //
      if (asset.XPathSelectElement("Values/FactoryUpgrade").HasElements) {
        this.FactoryUpgrades = new List<Upgrade>();
        foreach (var item in asset.XPathSelectElements("Values/FactoryUpgrade").Elements()) {
          this.FactoryUpgrades.Add(new Upgrade(item));
        }
      }
      if (asset.XPathSelectElement("Values/ExpeditionAttribute").HasElements) {
        var attributes = asset.XPathSelectElements("Values/ExpeditionAttribute/ExpeditionAttributes/Item").Where(w => w.HasElements).ToArray();
        if (attributes.Length > 0) {
          this.ExpeditionAttributes = new List<Upgrade>();
          foreach (var attribute in attributes) {
            if (attribute.Element("Attribute") == null) continue;
            this.ExpeditionAttributes.Add(new Upgrade(attribute.Element("Attribute").Value, attribute.Element("Amount")?.Value));
          }
        }
      }
      //
      this.TradePrice = asset.XPathSelectElement("Values/Item/TradePrice")?.Value;
      //
      this.Info = asset.XPathSelectElement("Values/Standard/InfoDescription") == null ? null : new Description(asset.XPathSelectElement("Values/Standard/InfoDescription").Value);
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XElement("Name", this.Name));
      result.Add(this.Icon.ToXml());
      result.Add(this.Text.ToXml("Text"));
      result.Add(this.Rarity.ToXml("Rarity"));
      result.Add(new XElement("ItemType", this.ItemType));
      //
      result.Add(this.Allocation.ToXml());
      //
      result.Add(new XElement("EffectTargets", this.EffectTargets == null ? null : this.EffectTargets.Select(s => s.ToXml("Target"))));
      //
      result.Add(new XElement("FactoryUpgrades", this.FactoryUpgrades == null ? null : this.FactoryUpgrades.Select(s => s.ToXml())));
      result.Add(new XElement("ExpeditionAttributes", this.ExpeditionAttributes == null ? null : this.ExpeditionAttributes.Select(s => s.ToXml())));
      //
      result.Add(new XElement("TradePrice", this.TradePrice));
      //
      if (this.Info != null) result.Add(this.Info.ToXml("Info"));
      return result.ToString(SaveOptions.None);
    }
    #endregion

  }

}