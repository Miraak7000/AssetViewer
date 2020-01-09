using AssetViewer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssetViewer.Templates {

  public class TemplateAsset {

    #region Properties

    public int ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public Description UpgradeText { get; set; }
    public Description AssociatedRegions { get; set; }
    public string RarityType { get; set; }

    public Description Rarity { get; set; }
    public String ItemType { get; set; }
    public String ReleaseVersion { get; set; }

    //
    public Allocation Allocation { get; set; }

    //
    public List<EffectTarget> EffectTargets { get; set; }

    public IEnumerable<Description> EffectBuildings => EffectTargets?.SelectMany(e => e.Buildings).Distinct();

    public string EffectTargetInfo => new Description(-1210).CurrentLang + string.Join(", ", EffectTargets.Select(s => s.Text.CurrentLang));
    public Boolean HasEffectTargetInfo { get; set; }

    //

    public List<Upgrade> PopulationUpgrades { get; set; }
    public List<Upgrade> ExpeditionAttributes { get; set; }
    public List<Upgrade> AttackableUpgrades { get; set; }
    public List<Upgrade> AttackerUpgrades { get; set; }
    public List<Upgrade> ItemActionUpgrades { get; set; }
    public List<Upgrade> DivingBellUpgrades { get; set; }
    public List<Upgrade> CraftableItemUpgrades { get; set; }
    public List<Upgrade> ItemSocketSet { get; set; }

    public IEnumerable<Upgrade> AllUpgrades => typeof(TemplateAsset)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(List<Upgrade>)
            && p.Name != nameof(Sources)
            && p.Name != nameof(CraftableItemUpgrades)
            && p.Name != nameof(UpgradeCosts)
            && p.Name != nameof(BuildCosts)
            && p.Name != nameof(ItemSets)
            && p.Name != nameof(ExpeditionAttributes)
            )
            .SelectMany(l => (List<Upgrade>)l.GetValue(this) ?? Enumerable.Empty<Upgrade>());

    //
    public List<Upgrade> ItemSets { get; set; }

    //
    public String TradePrice { get; set; }

    public String HiringFee { get; set; }

    //
    public Description Info { get; set; }

    //
    public List<Upgrade> Sources { get; set; }

    //
    public List<int> MonumentEvents { get; set; }

    public List<int> MonumentThresholds { get; set; }
    public List<int> MonumentRewards { get; set; }
    public List<Upgrade> ItemWithUI { get; }
    public List<Upgrade> ItemStartExpedition { get; }
    public List<Upgrade> Building { get; }
    public Modules Modules { get; set; }
    public string IsPausable { get; set; }
    public List<Upgrade> UpgradeCosts { get; }
    public List<Upgrade> BuildCosts { get; }
    public List<Upgrade> GenericUpgrades { get; } = new List<Upgrade>();
    public Maintenance Maintenance { get; } = new Maintenance();
    public List<Upgrade> Electric { get; }
    public FactoryBase FactoryBase { get; } = new FactoryBase();
    public List<string> SetParts { get; }
    public Description FestivalName { get; }

    #endregion Properties

    #region Constructors

    public TemplateAsset(XElement asset) {
      this.ID = int.Parse(asset.Attribute("ID").Value);
      this.Name = asset.Element("Name").Value;
      this.Text = new Description(asset.Element("Text"));
      this.RarityType = asset.Attribute("RarityType").Value;
      this.Rarity = new Description(asset.Element("Rarity"));
      this.ItemType = asset.Element("ItemType").Value;
      this.EffectTargets = asset.Element("EffectTargets")?.Elements().Select(s => new EffectTarget(s)).ToList() ?? new List<EffectTarget>();
      this.ReleaseVersion = asset.Attribute("Release")?.Value;
      this.IsPausable = asset.Attribute("IsPausable")?.Value;
      if (asset.Element("FestivalName") != null) {
        this.FestivalName = new Description(asset.Element("FestivalName"));
      }
      if (asset.Element("Allocation")?.HasElements == true) {
        this.Allocation = new Allocation(asset.Element("Allocation"));
      }
      if (asset.Element("AssociatedRegions") != null) {
        this.AssociatedRegions = new Description(asset.Element("AssociatedRegions"));
      }
      if (asset.Element("UpgradeText") != null) {
        this.UpgradeText = new Description(asset.Element("UpgradeText"));
      }
      this.HasEffectTargetInfo = this.EffectTargets.Count > 0;
      if (asset.Element("Modules")?.HasElements == true) {
        this.Modules = new Modules(asset.Element("Modules"));
      }
      if (asset.Element("Maintenance")?.HasElements == true) {
        this.Maintenance = new Maintenance(asset.Element("Maintenance"));
      }
      if (asset.Element("FactoryBase")?.HasElements == true) {
        this.FactoryBase = new FactoryBase(asset.Element("FactoryBase"));
      }
      if (asset.Element("UpgradeCosts")?.HasElements == true) {
        this.UpgradeCosts = asset.Element("UpgradeCosts").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("GenericUpgrades")?.HasElements == true) {
        this.GenericUpgrades = asset.Element("GenericUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("BuildCosts")?.HasElements == true) {
        this.BuildCosts = asset.Element("BuildCosts").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemSets")?.HasElements == true) {
        this.ItemSets = asset.Element("ItemSets").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("PopulationUpgrades")?.HasElements ?? false) {
        this.PopulationUpgrades = asset.Element("PopulationUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ExpeditionAttributes")?.HasElements ?? false) {
        this.ExpeditionAttributes = asset.Element("ExpeditionAttributes").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("AttackableUpgrades")?.HasElements ?? false) {
        this.AttackableUpgrades = asset.Element("AttackableUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("AttackerUpgrades")?.HasElements ?? false) {
        this.AttackerUpgrades = asset.Element("AttackerUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ItemActionUpgrades")?.HasElements ?? false) {
        this.ItemActionUpgrades = asset.Element("ItemActionUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      this.TradePrice = asset.Element("TradePrice")?.Value;
      this.HiringFee = asset.Element("HiringFee")?.Value;
      if (asset.Element("Info") != null) {
        this.Info = new Description(asset.Element("Info"));
      }
      if (asset.Element("Sources") != null) {
        this.Sources = asset.Element("Sources").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("MonumentEvents") != null) {
        this.MonumentEvents = asset.Element("MonumentEvents").Elements().Select(s => int.Parse(s.Value)).ToList();
      }
      if (asset.Element("MonumentThresholds") != null) {
        this.MonumentThresholds = asset.Element("MonumentThresholds").Elements().Select(s => int.Parse(s.Value)).ToList();
      }
      if (asset.Element("MonumentRewards") != null) {
        this.MonumentRewards = asset.Element("MonumentRewards").Elements().Select(s => int.Parse(s.Value)).ToList();
      }
      if (asset.Element("SetParts") != null) {
        this.SetParts = asset.Element("SetParts").Elements().Select(s => s.Value).ToList();
      }
      if (asset.Element("DivingBellUpgrades")?.HasElements ?? false) {
        this.DivingBellUpgrades = asset.Element("DivingBellUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("CraftableItemUpgrades")?.HasElements ?? false) {
        this.CraftableItemUpgrades = asset.Element("CraftableItemUpgrades").Elements().Select(s => new Upgrade(s)).ToList();
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
      if (asset.Element("ItemSocketSet")?.HasElements ?? false) {
        this.ItemSocketSet = asset.Element("ItemSocketSet").Elements().Select(s => new Upgrade(s)).ToList();
      }
    }

    #endregion Constructors
  }
}