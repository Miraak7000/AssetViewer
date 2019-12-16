using AssetViewer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Templates {

  public class TemplateAsset {

    #region Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public string RarityType { get; set; }

    public Description Rarity { get; set; }
    public String ItemType { get; set; }
    public String ReleaseVersion { get; set; }

    //
    public Allocation Allocation { get; set; }

    //
    public List<EffectTarget> EffectTargets { get; set; }

    public IEnumerable<Description> EffectBuildings => EffectTargets?.SelectMany(e => e.Buildings).Distinct();

    public string EffectTargetInfo => new Description("-1210").CurrentLang + string.Join(", ", EffectTargets.Select(s => s.Text.CurrentLang));
    public Boolean HasEffectTargetInfo { get; set; }

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
    public List<Upgrade> TradeShipUpgrades { get; set; }
    public List<Upgrade> VehicleUpgrades { get; set; }
    public List<Upgrade> AttackerUpgrades { get; set; }
    public List<Upgrade> VisitorHarborUpgrades { get; set; }
    public List<Upgrade> RepairCraneUpgrades { get; set; }
    public List<Upgrade> IncidentInfectableUpgrades { get; set; }
    public List<Upgrade> IncidentInfluencerUpgrades { get; set; }
    public List<Upgrade> ItemActionUpgrades { get; set; }
    public List<Upgrade> KontorUpgrades { get; set; }
    public List<Upgrade> ShipyardUpgrades { get; set; }
    public List<Upgrade> ItemGeneratorUpgrades { get; set; }
    public List<Upgrade> PassiveTradeGoodGenUpgrades { get; set; }
    public List<Upgrade> DivingBellUpgrades { get; set; }
    public List<Upgrade> CraftableItemUpgrades { get; set; }
    public List<Upgrade> PierUpgrade { get; set; }
    public List<Upgrade> ItemSocketSet { get; set; }
    public List<Upgrade> HeaterUpgrade { get; set; }

    public IEnumerable<Upgrade> AllUpgrades => typeof(TemplateAsset)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(List<Upgrade>) && p.Name != nameof(Sources) && p.Name != nameof(CraftableItemUpgrades))
            .SelectMany(l => (List<Upgrade>)l.GetValue(this) ?? Enumerable.Empty<Upgrade>());

    //
    public List<Upgrade> ItemSets { get; set; }

    //
    public String TradePrice { get; set; }

    //
    public Description Info { get; set; }

    //
    public List<Upgrade> Sources { get; set; }

    //
    public List<String> MonumentEvents { get; set; }

    public List<String> MonumentThresholds { get; set; }
    public List<String> MonumentRewards { get; set; }
    public List<Upgrade> ItemWithUI { get; }
    public List<Upgrade> ItemStartExpedition { get; }
    public List<Upgrade> Building { get; }

    #endregion Properties

    #region Constructors

    public TemplateAsset(XElement asset) {
      this.ID = asset.Attribute("ID").Value;
      this.Name = asset.Element("Name").Value;
      this.Text = new Description(asset.Element("Text"));
      this.RarityType = asset.Attribute("RarityType").Value;
      this.Rarity = new Description(asset.Element("Rarity"));
      this.ItemType = asset.Element("ItemType").Value;
      this.Allocation = asset.Element("Allocation").HasElements ? new Allocation(asset.Element("Allocation")) : null;
      this.EffectTargets = asset.Element("EffectTargets")?.Elements().Select(s => new EffectTarget(s)).ToList() ?? new List<EffectTarget>();
      this.ReleaseVersion = asset.Attribute("Release")?.Value;
      this.HasEffectTargetInfo = this.EffectTargets.Count > 0;
      if (asset.Element("ItemSets")?.HasElements == true) {
        this.ItemSets = asset.Element("ItemSets").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("FactoryUpgrades")?.HasElements ?? false) {
        this.FactoryUpgrades = asset.Element("FactoryUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("BuildingUpgrades")?.HasElements ?? false) {
        this.BuildingUpgrades = asset.Element("BuildingUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("CultureUpgrades")?.HasElements ?? false) {
        this.CultureUpgrades = asset.Element("CultureUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ModuleOwnerUpgrades")?.HasElements ?? false) {
        this.ModuleOwnerUpgrades = asset.Element("ModuleOwnerUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ResidenceUpgrades")?.HasElements ?? false) {
        this.ResidenceUpgrades = asset.Element("ResidenceUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("PopulationUpgrades")?.HasElements ?? false) {
        this.PopulationUpgrades = asset.Element("PopulationUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ElectricUpgrades")?.HasElements ?? false) {
        this.ElectricUpgrades = asset.Element("ElectricUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ExpeditionAttributes")?.HasElements ?? false) {
        this.ExpeditionAttributes = asset.Element("ExpeditionAttributes").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("AttackableUpgrades")?.HasElements ?? false) {
        this.AttackableUpgrades = asset.Element("AttackableUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("TradeShipUpgrades")?.HasElements ?? false) {
        this.TradeShipUpgrades = asset.Element("TradeShipUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("VehicleUpgrades")?.HasElements ?? false) {
        this.VehicleUpgrades = asset.Element("VehicleUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("AttackerUpgrades")?.HasElements ?? false) {
        this.AttackerUpgrades = asset.Element("AttackerUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ShipyardUpgrades")?.HasElements ?? false) {
        this.ShipyardUpgrades = asset.Element("ShipyardUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("VisitorHarborUpgrades")?.HasElements ?? false) {
        this.VisitorHarborUpgrades = asset.Element("VisitorHarborUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("RepairCraneUpgrades")?.HasElements ?? false) {
        this.RepairCraneUpgrades = asset.Element("RepairCraneUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("KontorUpgrades")?.HasElements ?? false) {
        this.KontorUpgrades = asset.Element("KontorUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("IncidentInfectableUpgrades")?.HasElements ?? false) {
        this.IncidentInfectableUpgrades = asset.Element("IncidentInfectableUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("IncidentInfluencerUpgrades")?.HasElements ?? false) {
        this.IncidentInfluencerUpgrades = asset.Element("IncidentInfluencerUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemGeneratorUpgrades")?.HasElements ?? false) {
        this.ItemGeneratorUpgrades = asset.Element("ItemGeneratorUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemActionUpgrades")?.HasElements ?? false) {
        this.ItemActionUpgrades = asset.Element("ItemActionUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("PassiveTradeGoodGenUpgrades")?.HasElements ?? false) {
        this.PassiveTradeGoodGenUpgrades = asset.Element("PassiveTradeGoodGenUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      this.TradePrice = asset.Element("TradePrice")?.Value;
      if (asset.Element("Info") != null) {
        this.Info = new Description(asset.Element("Info"));
      }
      if (asset.Element("Sources") != null) {
        this.Sources = asset.Element("Sources").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("MonumentEvents") != null) {
        this.MonumentEvents = asset.Element("MonumentEvents").Elements().Select(s => s.Value).ToList();
      }
      if (asset.Element("MonumentThresholds") != null) {
        this.MonumentThresholds = asset.Element("MonumentThresholds").Elements().Select(s => s.Value).ToList();
      }
      if (asset.Element("MonumentRewards") != null) {
        this.MonumentRewards = asset.Element("MonumentRewards").Elements().Select(s => s.Value).ToList();
      }
      if (asset.Element("DivingBellUpgrades")?.HasElements ?? false) {
        this.DivingBellUpgrades = asset.Element("DivingBellUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("CraftableItemUpgrades")?.HasElements ?? false) {
        this.CraftableItemUpgrades = asset.Element("CraftableItemUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("PierUpgrade")?.HasElements ?? false) {
        this.PierUpgrade = asset.Element("PierUpgrade").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemWithUI")?.HasElements ?? false) {
        this.ItemWithUI = asset.Element("ItemWithUI").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemStartExpedition")?.HasElements ?? false) {
        this.ItemStartExpedition = asset.Element("ItemStartExpedition").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("Building")?.HasElements ?? false) {
        this.Building = asset.Element("Building").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("PierUpgrade")?.HasElements ?? false) {
        this.PierUpgrade = asset.Element("PierUpgrade").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemSocketSet")?.HasElements ?? false) {
        this.ItemSocketSet = asset.Element("ItemSocketSet").Elements().Select(s => new Upgrade(s)).ToList();
      }  
      if (asset.Element("HeaterUpgrade")?.HasElements ?? false) {
        this.HeaterUpgrade = asset.Element("HeaterUpgrade").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }

    #endregion Constructors
  }
}