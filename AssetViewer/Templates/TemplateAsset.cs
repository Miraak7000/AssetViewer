using AssetViewer.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace AssetViewer.Templates {

  public class TemplateAsset {

    #region Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }

    public Description Rarity { get; set; }
    public String ItemType { get; set; }
    public String ReleaseVersion { get; set; }

    //
    public Allocation Allocation { get; set; }

    //
    public List<EffectTarget> EffectTargets { get; set; }
    public IEnumerable<Description> EffectBuildings => EffectTargets?.SelectMany(e => e.Buildings).Distinct();

    public Description EffectTargetInfo { get; set; }
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
    public List<Upgrade> KontorUpgrades { get; }
    public List<Upgrade> ShipyardUpgrades { get; }
    public List<Upgrade> ItemGeneratorUpgrades { get; }

    public IEnumerable<Upgrade> AllUpgrades => typeof(TemplateAsset)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(List<Upgrade>) && p.Name != nameof(Sources))
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

    #endregion Properties

    #region Constructors

    public TemplateAsset(XElement asset) {
      this.ID = asset.Attribute("ID").Value;
      this.Name = asset.Element("Name").Value;
      this.Text = new Description(asset.Element("Text"));
      this.Rarity = new Description(asset.Element("Rarity"));
      this.ItemType = asset.Element("ItemType").Value;
      this.Allocation = asset.Element("Allocation").HasElements ? new Allocation(asset.Element("Allocation")) : null;
      this.EffectTargets = asset.Element("EffectTargets").Elements().Select(s => new EffectTarget(s)).ToList();
      this.EffectTargetInfo = new Description("Affects ", "Beeinflusst ");
      this.ReleaseVersion = asset.Attribute("Release")?.Value;
      for (var i = 0; i < this.EffectTargets.Count; i++) {
        if (i > 0) {
          this.EffectTargetInfo.EN += ", ";
          this.EffectTargetInfo.DE += ", ";
        }
        this.EffectTargetInfo.EN += this.EffectTargets[i].Text.EN;
        this.EffectTargetInfo.DE += this.EffectTargets[i].Text.DE;
      }
      this.HasEffectTargetInfo = this.EffectTargets.Count > 0;
      if (asset.Element("ItemSets") != null && asset.Element("ItemSets").HasElements) {
        this.ItemSets = asset.Element("ItemSets").Elements().Select(s => new Upgrade(s)).ToList();
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
      if (asset.Element("TradeShipUpgrades").HasElements) {
        this.TradeShipUpgrades = asset.Element("TradeShipUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("VehicleUpgrades").HasElements) {
        this.VehicleUpgrades = asset.Element("VehicleUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("AttackerUpgrades").HasElements) {
        this.AttackerUpgrades = asset.Element("AttackerUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ShipyardUpgrades").HasElements) {
        this.ShipyardUpgrades = asset.Element("ShipyardUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("VisitorHarborUpgrades").HasElements) {
        this.VisitorHarborUpgrades = asset.Element("VisitorHarborUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("RepairCraneUpgrades").HasElements) {
        this.RepairCraneUpgrades = asset.Element("RepairCraneUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("KontorUpgrades").HasElements) {
        this.KontorUpgrades = asset.Element("KontorUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("IncidentInfectableUpgrades").HasElements) {
        this.IncidentInfectableUpgrades = asset.Element("IncidentInfectableUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("IncidentInfluencerUpgrades").HasElements) {
        this.IncidentInfluencerUpgrades = asset.Element("IncidentInfluencerUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemGeneratorUpgrades").HasElements) {
        this.ItemGeneratorUpgrades = asset.Element("ItemGeneratorUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemActionUpgrades")?.HasElements ?? false) {
        this.ItemActionUpgrades = asset.Element("ItemActionUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
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
    }

    #endregion Constructors
  }
}