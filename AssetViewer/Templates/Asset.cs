using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AssetViewer.Data;

namespace AssetViewer.Templates {

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
    public Description EffectTargetInfo { get; set; }
    //
    public List<Upgrade> FactoryUpgrades { get; set; }
    public List<Upgrade> BuildingUpgrades { get; set; }
    public List<Upgrade> CultureUpgrades { get; set; }
    public List<Upgrade> ModuleOwnerUpgrades { get; set; }
    public List<Upgrade> ResidenceUpgrades { get; set; }
    public List<Upgrade> PopulationUpgrades { get; set; }
    public List<Upgrade> ElectricUpgrades { get; set; }
    public List<Upgrade> ExpeditionAttributes { get; set; }
    public List<Upgrade> AttackableUpgrades { get; set; }
    //
    public String TradePrice { get; set; }
    //
    public Description Info { get; set; }
    #endregion

    #region Constructor
    public Asset(XElement asset) {
      this.ID = asset.Attribute("ID").Value;
      this.Name = asset.Element("Name").Value;
      this.Icon = new Icon(asset.Element("Icon"));
      this.Text = new Description(asset.Element("Text"));
      this.Rarity = new Description(asset.Element("Rarity"));
      this.ItemType = asset.Element("ItemType").Value;
      this.Allocation = new Allocation(asset.Element("Allocation"));
      this.EffectTargets = asset.Element("EffectTargets").Elements().Select(s => new Description(s)).ToList();
      this.EffectTargetInfo = new Description("Affects ", "Beeinflusst ");
      for (int i = 0; i < this.EffectTargets.Count; i++) {
        if (i > 0) {
          this.EffectTargetInfo.EN += ", ";
          this.EffectTargetInfo.DE += ", ";
        }
        this.EffectTargetInfo.EN += this.EffectTargets[i].EN;
        this.EffectTargetInfo.DE += this.EffectTargets[i].DE;
      }
      if (asset.Element("FactoryUpgrades").HasElements) {
        this.FactoryUpgrades = asset.Element("FactoryUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("BuildingUpgrades").HasElements) {
        this.BuildingUpgrades = asset.Element("BuildingUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("CultureUpgrades").HasElements) {
        this.CultureUpgrades = asset.Element("CultureUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ModuleOwnerUpgrades").HasElements) {
        this.ModuleOwnerUpgrades = asset.Element("ModuleOwnerUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ResidenceUpgrades").HasElements) {
        this.ResidenceUpgrades = asset.Element("ResidenceUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("PopulationUpgrades").HasElements) {
        this.PopulationUpgrades = asset.Element("PopulationUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ElectricUpgrades").HasElements) {
        this.ElectricUpgrades = asset.Element("ElectricUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ExpeditionAttributes").HasElements) {
        this.ExpeditionAttributes = asset.Element("ExpeditionAttributes").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("AttackableUpgrades").HasElements) {
        this.AttackableUpgrades = asset.Element("AttackableUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      this.TradePrice = asset.Element("TradePrice")?.Value;
      if (asset.Element("Info") != null) {
        this.Info = new Description(asset.Element("Info"));
      }
    }
    #endregion

  }

}