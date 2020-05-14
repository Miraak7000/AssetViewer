using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace AssetViewer.Data {

  public class TemplateAsset : INotifyPropertyChanged {

    #region Public Properties

    public int ID { get; set; }
    public string Name { get; set; }
    public Description Text { get; set; }
    public Description UpgradeText { get; set; }
    public Description AssociatedRegions { get; set; }
    public string RarityType { get; set; }

    public uint Count {
      get => count;
      set {
        if (count != value) {
          count = value;
          RaisePropertyChanged(nameof(Count));
          AssetProvider.RaiseAssetCountChanged(this);
        }
      }
    }

    public Description Rarity { get; set; }
    public string ItemType { get; set; }
    public string ReleaseVersion { get; set; }
    public Allocation Allocation { get; set; }
    public List<EffectTarget> EffectTargets { get; set; }
    public IEnumerable<Description> EffectBuildings => EffectTargets?.SelectMany(e => e.Buildings).Distinct();
    public string EffectTargetInfo => new Description(-1210).CurrentLang + string.Join(", ", EffectTargets.Select(s => s.Text.CurrentLang));
    public bool HasEffectTargetInfo { get; set; }
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

    public List<Upgrade> ItemSets { get; set; }
    public string TradePrice { get; set; }
    public string HiringFee { get; set; }
    public Description Info { get; set; }
    public List<Upgrade> Sources { get; set; }
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

    #endregion Public Properties

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Constructors

    public TemplateAsset(XElement asset) {
      ID = int.Parse(asset.Attribute("ID").Value);
      Name = asset.Element("N").Value;
      Text = new Description(asset.Element("T"));
      RarityType = asset.Attribute("RT").Value;
      Rarity = new Description(asset.Element("R"));
      ItemType = asset.Element("IT").Value;
      EffectTargets = asset.Element("ET")?.Elements().Select(s => new EffectTarget(s)).ToList() ?? new List<EffectTarget>();
      ReleaseVersion = asset.Attribute("RV")?.Value;
      IsPausable = asset.Attribute("IP")?.Value;
      if (asset.Element("FN") != null) {
        FestivalName = new Description(asset.Element("FN"));
      }
      if (asset.Element("A")?.HasElements == true) {
        Allocation = new Allocation(asset.Element("A"));
      }
      if (asset.Element("AR") != null) {
        AssociatedRegions = new Description(asset.Element("AR"));
      }
      if (asset.Element("UT") != null) {
        UpgradeText = new Description(asset.Element("UT"));
      }
      if (asset.Element("Mo")?.HasElements == true) {
        Modules = new Modules(asset.Element("Mo"));
      }
      if (asset.Element("M")?.HasElements == true) {
        Maintenance = new Maintenance(asset.Element("M"));
      }
      if (asset.Element("FB")?.HasElements == true) {
        FactoryBase = new FactoryBase(asset.Element("FB"));
      }
      if (asset.Element("UC")?.HasElements == true) {
        UpgradeCosts = asset.Element("UC").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("GU")?.HasElements == true) {
        GenericUpgrades = asset.Element("GU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("BC")?.HasElements == true) {
        BuildCosts = asset.Element("BC").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("IS")?.HasElements == true) {
        ItemSets = asset.Element("IS").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("PU")?.HasElements ?? false) {
        PopulationUpgrades = asset.Element("PU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("EA")?.HasElements ?? false) {
        ExpeditionAttributes = asset.Element("EA").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("DU")?.HasElements ?? false) {
        AttackableUpgrades = asset.Element("DU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("AU")?.HasElements ?? false) {
        AttackerUpgrades = asset.Element("AU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("IAU")?.HasElements ?? false) {
        ItemActionUpgrades = asset.Element("IAU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      TradePrice = asset.Element("TP")?.Value;
      HiringFee = asset.Element("HF")?.Value;
      if (asset.Element("I") != null) {
        Info = new Description(asset.Element("I"));
      }
      if (asset.Element("S") != null) {
        Sources = asset.Element("S").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ME") != null) {
        MonumentEvents = asset.Element("ME").Elements().Select(s => int.Parse(s.Value)).ToList();
      }
      if (asset.Element("MT") != null) {
        MonumentThresholds = asset.Element("MT").Elements().Select(s => int.Parse(s.Value)).ToList();
      }
      if (asset.Element("MR") != null) {
        MonumentRewards = asset.Element("MR").Elements().Select(s => int.Parse(s.Value)).ToList();
      }
      if (asset.Element("SP") != null) {
        SetParts = asset.Element("SP").Elements().Select(s => s.Value).ToList();
      }
      if (asset.Element("DBU")?.HasElements ?? false) {
        DivingBellUpgrades = asset.Element("DBU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("CIU")?.HasElements ?? false) {
        CraftableItemUpgrades = asset.Element("CIU").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("IWUI")?.HasElements ?? false) {
        ItemWithUI = asset.Element("IWUI").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ISE")?.HasElements ?? false) {
        ItemStartExpedition = asset.Element("ISE").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("B")?.HasElements ?? false) {
        Building = asset.Element("B").Elements().Select(s => new Upgrade(s)).ToList();
      }
      if (asset.Element("ISS")?.HasElements ?? false) {
        ItemSocketSet = asset.Element("ISS").Elements().Select(s => new Upgrade(s)).ToList();
      }
      HasEffectTargetInfo = EffectTargets.Count > 0;
    }

    #endregion Public Constructors

    #region Public Methods

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion Public Methods

    #region Private Fields

    private uint count;

    #endregion Private Fields
  }
}