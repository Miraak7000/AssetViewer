using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class Asset {

    #region Public Properties

    public static ConcurrentDictionary<string, ConcurrentBag<SourceWithDetailsList>> SavedSources { get; set; } = new ConcurrentDictionary<string, ConcurrentBag<SourceWithDetailsList>>();
    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public Description UpgradeText { get; set; }
    public Description AssociatedRegions { get; set; }
    public string RarityType { get; private set; } = "Common";
    public Description Rarity { get; set; }
    public string ItemType { get; set; }
    public String ReleaseVersion { get; set; } = "Release";
    public String TradePrice { get; set; }
    public string HiringFee { get; private set; }
    public Description Info { get; set; }
    public Modules Modules { get; set; }
    public bool IsPausable { get; set; }

    public Allocation Allocation { get; set; }

    public Maintenance Maintenance { get; private set; }
    public FactoryBase FactoryBase { get; private set; }

    public List<EffectTarget> EffectTargets { get; set; }

    public IEnumerable<Upgrade> AllUpgrades => typeof(Asset)
      .GetProperties()
      .Where(p => p.PropertyType == typeof(List<Upgrade>) && p.Name != nameof(Sources))
      .OrderBy(p => p.Name)
      .SelectMany(l => (List<Upgrade>)l.GetValue(this) ?? Enumerable.Empty<Upgrade>());

    public List<Upgrade> ItemActionUpgrades { get; set; }

    public List<Upgrade> ItemSets { get; set; }
    public List<TempSource> Sources { get; set; }

    public List<String> MonumentEvents { get; set; }
    public List<String> MonumentThresholds { get; set; }
    public List<String> MonumentRewards { get; set; }
    public List<Upgrade> PopulationUpgrades { get; set; }
    public List<Upgrade> ExpeditionAttributes { get; set; }
    public List<Upgrade> DefenceUpgrades { get; set; }
    public List<Upgrade> AttackerUpgrades { get; set; }
    public List<Upgrade> DivingBellUpgrades { get; private set; }
    public List<Upgrade> CraftableItemUpgrades { get; private set; }
    public List<Upgrade> ItemWithUI { get; private set; }
    public List<Upgrade> ItemStartExpedition { get; private set; }
    public List<Upgrade> ItemSocketSet { get; private set; }
    public List<Upgrade> UpgradeCosts { get; private set; }
    public List<Upgrade> BuildCosts { get; private set; }
    public List<Upgrade> GenericUpgrades { get; private set; } = new List<Upgrade>();
    public IEnumerable<string> SetParts { get; internal set; }
    public Description FestivalName { get; internal set; }

    #endregion Public Properties

    #region Public Constructors

    public Asset(XElement asset, Boolean findSources) {
      //Set Item Typ
      this.ItemType = asset.Element("Item")?.Element("ItemType")?.Value;
      if (this.ItemType == "Normal" || this.ItemType == "None" || string.IsNullOrWhiteSpace(this.ItemType)) {
        switch (asset.Element("Template").Value) {
          case string s when s.EndsWith("Buff"):
          case "MonumentEventCategory":
          case "MonumentEvent":
          case "MonumentEventReward":
          case "StartExpeditionItem":
            //Ignore
            break;

          case "GuildhouseItem":
          case "ItemSpecialAction":
          case "HarborOfficeItem":
          case "VehicleItem":
          case "TownhallItem":
          case "ItemSpecialActionVisualEffect":
          case "ActiveItem":
          case "ShipSpecialist":
          case "ItemConstructionPlan":
            this.ItemType = "Item";
            break;

          case "BuildPermitBuilding":
          case "FarmBuilding":
          case "ResidenceBuilding7":
          case "FreeAreaBuilding":
          case "FactoryBuilding7":
          case "SlotFactoryBuilding7":
          case "HeavyFactoryBuilding":
          case "HeavyFreeAreaBuilding":
          case "Slot":
          case "OilPumpBuilding":
          case "PublicServiceBuilding":
          case "CityInstitutionBuilding":
          case "Market":
          case "Warehouse":
          case "CultureBuilding":
          case "Monument":
          case "PowerplantBuilding":
          case "Street":
          case "StreetBuilding":
          case "Guildhouse":
          case "HarborOffice":
          case "HarborWarehouse7":
          case "HarborDepot":
          case "Shipyard":
          case "HarborBuildingAttacker":
          case "RepairCrane":
          case "HarborLandingStage7":
          case "VisitorPier":
          case "HarborWarehouseStrategic":
          case "WorkforceConnector":
          case "OrnamentalBuilding":
          case "HarborPropObject":
          case "BridgeBuilding":
          case "QuestLighthouse":
          case "WorkAreaSlot":
          case "SimpleBuilding":
          case "HarborWarehouseSlot7":
          case "CampaignQuestObject":
          case "CampaignUncleMansion":
          case "ItemCrafterBuilding":
          case "VisualBuilding_NoLogic":
          case "ResidenceBuilding7_Arctic":
          case "FreeAreaBuilding_Arctic":
          case "FarmBuilding_Arctic":
          case "HeavyFreeAreaBuilding_Arctic":
          case "FactoryBuilding7_Arctic":
          case "SlotFactoryBuilding7_Arctic":
          case "Monument_with_Shipyard":
          case "Heater_Arctic":
          case "Headquarter":
          case "PalaceMinistry":
          case "Palace":
            this.ItemType = "Building";
            break;

          case "BuildPermitModules":
          case "CultureModule":
          case "OrnamentalModule":
          case "Farmfield":
          case "PalaceModule":
            this.ItemType = "Module";
            break;

          case "CultureItem":
            this.ItemType = "Animal";
            break;

          case "Product":
            this.ItemType = "Product";
            break;

          case "FluffItem":
            this.ItemType = "Character Item";
            break;

          case "ItemWithUI":
            this.ItemType = "Document";
            break;

          case "QuestItem":
          case "QuestItemMagistrate":
            this.ItemType = "Quest Item";
            break;

          case "ItemSet":
            this.ItemType = "Item Set";
            break;

          default:
            Debug.WriteLine(asset.Element("Template").Value);
            break;
        }
      }
      //All Other Elements
      foreach (var element in asset.Element("Values").Elements()) {
        switch (element.Name.LocalName) {
          case "Text":
          case "Locked":
          case "Buff":
          case "Blocking":
          case "Selection":
          case "Object":
          case "Constructable":
          case "Mesh":
          case "SoundEmitter":
          case "FeedbackController":
          case "AmbientMoodProvider":
          case "BuildPermit":
          case "VisualEffectWhenActive":
          case "QuestObject":
          case "MinimapToken":
          case "RandomMapObject":
          case "IslandUnique":
          case "DelayedConstruction":
          case "RandomDummySpawner":
          case "CommandQueue":
          case "WorkforceConnector":
          // ignore this nodes

          case "UpgradeList":
          case "VisitorUpgrade":
          case "ItemCrafterBuilding":
          //Maybe next patch. all empty

          case "ItemGeneratorUpgrade":
          case "SpecialAction":
          case "NewspaperUpgrade":
          case "DistributionUpgrade":
          case "DistributionCenterMarker":
          case "ItemConstructionPlan":
          // Todo: needs to implemented

          case "Product":
          //Todo: maybe add some properties

          case "Infolayer":
          case "ItemContainer":
          case "BuildingModule":
          case "Attackable":
          case "LogisticNode":
          case "StreetActivation":
          case "IncidentInfectable":
          case "Factory7": //NeededFertility
          case "Slot": //Gold, iron etc slot to build
          case "IncidentInfluencer":
          case "IncidentResolver":
          case "StorageBase":
          case "Street":
          case "DistributionCenter":
          case "Distribution":
          case "VisitorHarbor":
          case "Bridge":
          case "BezierPath":
          case "CampaignBehaviour":
          case "HeatProvider":
          case "BuildingUnique":
          //Maybe usefull Building Informations
          case "InfluenceSource":
          //Building influence gain
          case "EffectForward":
          case "MonumentUpgrade":
          case "InfluenceSourceUpgrade":
          case "PalaceMinistry":
          case "Palace":
            //Ministary stuff

            break;

          case "Standard":
            this.ProcessElement_Standard(element);
            break;

          case "Item":
            this.ProcessElement_Item(element);
            break;

          case "ItemEffect":
            this.ProcessElement_ItemEffect(element);
            break;

          case "Building": //Building category, Terrain
            this.ProcessElement_Building(element);
            break;

          case "FactoryUpgrade":
          case "BuildingUpgrade":
          case "CultureUpgrade":
          case "ResidenceUpgrade":
          case "ElectricUpgrade":
          case "VisitorHarborUpgrade":
          case "ShipyardUpgrade":
          case "VehicleUpgrade":
          case "RepairCraneUpgrade":
          case "KontorUpgrade":
          case "TradeShipUpgrade":
          case "IncidentInfectableUpgrade":
          case "IncidentInfluencerUpgrade":
          case "PierUpgrade":
          case "HeaterUpgrade":
          case "ModuleOwnerUpgrade":
          case "WarehouseUpgrade":
          case "ItemContainerUpgrade":
          //Buildings
          case "Culture":
          case "LoadingPier":
          case "Attacker":
          case "Shipyard":
          case "RepairCrane":
          case "TrainStation":
          case "PowerplantUpgrade":
            this.ProcessElement_GenericUpgradeChilds(element, element.Name.LocalName);
            break;

          case "PassiveTradeGoodGenUpgrade":
          //Buildings
          case "Residence7":
            this.ProcessElement_GenericUpgradeElement(element, element.Name.LocalName);
            break;

          //Non Generics
          case "PopulationUpgrade":
            this.ProcessElement_PopulationUpgrade(element);
            break;

          case "ExpeditionAttribute":
            this.ProcessElement_ExpeditionAttribute(element);
            break;

          case "AttackerUpgrade":
            this.ProcessElement_AttackerUpgrade(element);
            break;

          case "AttackableUpgrade":
            this.ProcessElement_AttackableUpgrade(element);
            break;

          case "ProjectileUpgrade":
            this.ProcessElement_ProjectileUpgrade(element);
            break;

          case "ItemAction":
            this.ProcessElement_ItemActions(element);
            break;

          case "MonumentEventCategory":
            this.ProcessElement_MonumentEventCategory(element);
            break;

          case "MonumentEvent":
            this.ProcessElement_MonumentEvent(element);
            break;

          case "Ornament":
            this.Info = element.Element("OrnamentDescritpion") == null ? null : new Description(element.Element("OrnamentDescritpion").Value);
            break;

          case "Reward":
            this.ProcessElement_MonumentEventReward(element);
            break;

          case "DivingBellUpgrade":
            this.ProcessElement_DivingBellUpgrade(element);
            break;

          case "CraftableItem":
            this.ProcessElement_CraftableItem(element);
            break;

          case "ItemWithUI":
            this.ProcessElement_ItemWithUI(element);
            break;

          case "ItemStartExpedition":
            this.ProcessElement_ItemStartExpedition(element);
            break;

          case "ItemSocketSet":
            this.ProcessElement_ItemSocketSet(element);
            break;

          //Buildings
          case "Upgradable":
            this.ProcessElement_Upgradable(element);
            break;

          case "Cost":
            this.ProcessElement_Cost(element);
            break;

          case "Pausable":
            this.ProcessElement_Pausable(element);
            break;

          case "FactoryBase":
            this.FactoryBase = new FactoryBase(element);
            break;

          case "ModuleOwner":
            this.Modules = new Modules(element);
            break;

          case "Maintenance":
            this.Maintenance = new Maintenance(element);
            break;

          case "Electric":
            this.ProcessElement_Electric(element);
            break;

          case "PublicService":
            this.ProcessElement_PublicService(element);
            break;

          case "Powerplant":
            this.ProcessElement_Powerplant(element);
            break;

          case "Market":
            this.ProcessElement_Market(element);
            break;

          case "Heated":
            this.ProcessElement_Heated(element);
            break;

          case "Warehouse":
            this.ProcessElement_Warehouse(element);
            break;

          case "FreeAreaProductivity":
            this.ProcessElement_FreeAreaProductivity(element);
            break;

          case "Monument":
            this.ProcessElement_Monument(element);
            break;

          default:
            Debug.WriteLine(element.Name.LocalName);
            //throw new NotImplementedException(element.Name.LocalName);
            break;
        }
      }
      if (findSources) {
        var results = this.FindSources(this.ID);
        var sources = results.MergeResults(ID.ToString());
        this.Sources = sources.Select(s => new TempSource(s)).ToList();
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("Asset");
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XAttribute("RV", this.ReleaseVersion));
      result.Add(new XElement("N", this.Name));
      result.Add(this.Text.ToXml("T"));
      result.Add(this.Rarity == null ? new Description(Assets.KeyToIdDict["Common"]).ToXml("R") : this.Rarity.ToXml("R"));
      result.Add(new XAttribute("RT", RarityType));
      result.Add(new XElement("IT", this.ItemType));

      if (this.FestivalName != null) {
        result.Add(this.FestivalName.ToXml("FN"));
      }
      if (this.Allocation != null) {
        result.Add(this.Allocation.ToXml());
      }
      if (this.AssociatedRegions != null) {
        result.Add(this.AssociatedRegions.ToXml("AR"));
      }

      var type = typeof(Asset);
      foreach (var item in type.GetProperties().Where(p => p.PropertyType == typeof(List<Upgrade>))) {
        if (item.GetValue(this) != null) {
          var builder = new StringBuilder();
          foreach (var character in item.Name) {
            if (Char.IsUpper(character)) {
              builder.Append(character);
            }
          }
          result.Add(new XElement(builder.ToString(), (item.GetValue(this) as List<Upgrade>)?.Select(s => s.ToXml())));
        }
      }

      result.Add(new XElement("TP", this.TradePrice));
      result.Add(new XElement("HF", this.HiringFee));

      if (this.Info != null)
        result.Add(this.Info.ToXml("I"));
      if (this.FactoryBase != null)
        result.Add(this.FactoryBase.ToXml());
      if (this.Maintenance != null)
        result.Add(this.Maintenance.ToXml());
      if (this.UpgradeText != null)
        result.Add(this.UpgradeText.ToXml("UT"));
      if (this.Modules != null)
        result.Add(this.Modules.ToXml());
      if (this.IsPausable)
        result.Add(new XAttribute("IP", true));

      if (this.EffectTargets != null)
        result.Add(new XElement("ET", this.EffectTargets.Select(s => s.ToXml())));
      if (this.Sources != null)
        result.Add(new XElement("S", this.Sources?.Select(s => s.ToXml())));

      if (this.MonumentEvents != null)
        result.Add(new XElement("ME", this.MonumentEvents?.Select(s => new XElement("E", s))));
      if (this.MonumentThresholds != null)
        result.Add(new XElement("MT", this.MonumentThresholds?.Select(s => new XElement("T", s))));
      if (this.MonumentRewards != null)
        result.Add(new XElement("MR", this.MonumentRewards?.Select(s => new XElement("R", s))));
      if (this.SetParts != null)
        result.Add(new XElement("SP", this.SetParts?.Select(s => new XElement("P", s))));
      return result;
    }

    public override String ToString() {
      return $"{this.ID} - {this.Name}";
    }

    #endregion Public Methods

    #region Private Methods

    private void ProcessElement_Building(XElement element) {
      if (element.Element("AssociatedRegions")?.Value is string str) {
        var regions = str.Split(';').Select(s => new Description(Assets.KeyToIdDict[s]));
        AssociatedRegions = Description.Join(regions, ", ");
      }
    }

    private void ProcessElement_Monument(XElement element) {
      if ((element.Element("UpgradeTarget")?.Value ?? "0") != "0") {
        this.UpgradeText = new Description("10580").Remove("&lt;br/&gt;[AssetData([Conditions QuestCondition Context]) Text] [Conditions QuestCondition CurrentAmount]/[Conditions QuestCondition Amount]");
        this.UpgradeText.AppendWithSpace(new Description(element.Element("UpgradeTarget").Value));
      }
    }

    private void ProcessElement_Heated(XElement element) {
      if (element.Element("RequiresHeat")?.Value == "1") {
        this.GenericUpgrades.Add(new Upgrade { Text = new Description("116353") });
      }
    }

    private void ProcessElement_FreeAreaProductivity(XElement element) {
      var radius = "";
      if (element.Element("InfluenceRadius")?.Value is string value) {
        radius = value;
        if (element.Element("NeededAreaPercent")?.Value is string needed) {
          radius += $" / {needed}%";
        }
        this.GenericUpgrades.Add(new Upgrade { Text = new Description("12504"), Value = radius });
      }
    }

    private void ProcessElement_Warehouse(XElement element) {
      if (element.Element("WarehouseStorage")?.Element("StorageMax")?.Value is string value && value != "0") {
        this.GenericUpgrades.Add(new Upgrade { Text = new Description("22426"), Value = "+" + value });
      }
    }

    private void ProcessElement_ItemSocketSet(XElement element) {
      this.ItemSocketSet = new List<Upgrade>();
      if (element.Element("SetBuff")?.Value is string buff) {
        this.ItemSocketSet.AddRange(Assets.Buffs[buff].AllUpgrades.ToList());
        this.EffectTargets = Assets.Buffs[buff].EffectTargets;
      }
      else if (element.Element("RegionSetBuff") is XElement regionBuffs) {
        this.EffectTargets = new List<EffectTarget>();
        foreach (var region in regionBuffs.Elements()) {
          switch (region.Name.LocalName) {
            case "Moderate":
              this.ItemSocketSet.Add(new Upgrade { Text = new Description("113322"), Additionals = Assets.Buffs[region.Element("SetBuff").Value].AllUpgrades.ToList() });
              break;

            case "Colony01":
              this.ItemSocketSet.Add(new Upgrade { Text = new Description("113395"), Additionals = Assets.Buffs[region.Element("SetBuff").Value].AllUpgrades.ToList() });
              break;

            default:
              throw new NotImplementedException();
          }
          this.EffectTargets.AddRange(Assets.Buffs[region.Element("SetBuff").Value].EffectTargets);
        }
      }
      var allocation = Assets.Original.Descendants("Asset")
        .FirstOrDefault(a => a.Descendants("Set").Any(s => s.Value == this.ID));
      if (allocation != null) {
        this.Allocation = new Allocation(allocation.XPathSelectElement("Values/Standard/GUID").Value, null);
      }
    }

    private void ProcessElement_Standard(XElement element) {
      this.ID = element.Element("GUID").Value;
      this.Name = element.Element("Name").Value;
      this.Text = new Description(element.Element("GUID").Value);
      this.Info = element.Element("InfoDescription") == null ? null : new Description(element.Element("InfoDescription").Value);
    }

    private void ProcessElement_Item(XElement element) {
      this.RarityType = element.Element("Rarity")?.Value ?? "Common";
      this.Rarity = element.Element("Rarity") == null ? new Description("118002") : new Description(Assets.GetDescriptionID(element.Element("Rarity").Value));
      this.Allocation = new Allocation(element.Parent.Parent.Element("Template").Value, element.Element("Allocation")?.Value);
      if (element.Element("Allocation") == null) {
        element.Add(new XElement("Allocation", this.Allocation.ID));
      }
      this.TradePrice = element.Element("TradePrice") == null ? null : (Int32.Parse(element.Element("TradePrice").Value) / 4).ToString();
      this.HiringFee = element.Element("TradePrice") == null ? null : (Int32.Parse(element.Element("TradePrice").Value)).ToString();
      if (element.Element("ItemSet") != null) {
        this.ItemSets = new List<Upgrade> {
          new Upgrade(element.Element("ItemSet"))
        };
      }
    }

    private void ProcessElement_ItemEffect(XElement element) {
      if (element.HasElements) {
        if (element.Element("EffectTargets") == null)
          throw new NotImplementedException();
        if (element.Element("EffectTargets").HasElements) {
          this.EffectTargets = new List<EffectTarget>();
          foreach (var item in element.Element("EffectTargets").Elements()) {
            EffectTargets.Add(new EffectTarget(item));
          }
        }
      }
    }

    private void ProcessElement_GenericUpgradeElement(XElement element, string category) {
      if (element.Value != "")
        switch (element.Name.LocalName) {
          case "PublicServiceNoSatisfactionDistance":
          case "ChangeModule":
          case "ForcedFeedbackVariation":
          case "AdditionalModuleSoundLoop":
          case "HideBuff":
          case "ReplaceAssemblyOptions":
          case "HealBuildingsPerMinuteUpgrade":
          case "OverrideIncidentAttractiveness":
          case "RiotInfluenceUpgrade":
          case "FireInfluenceUpgrade":
          case "IllnessInfluenceUpgrade":
          case "DistanceUpgrade":
          //case "WorkerUnit":
          //case "MaxWorkerAmount":
          //case "WorkerPause":
          //case "PlantTreePercent":
          //case "CutTree":
          case "WayTime":
          case "NeededAreaPercent":
          //case "FreeAreaType":
          case "CultureType":  //maybe Implement
          case "CultureSpawnGroup":
          case "Sets":
          case "SetPages":
          case "UndiscoveredSet":
          case "ModuleListHeader":
          case "OpenSetPages":
          case "CultureMoodImage":
          case "MinLoadingTime":
          case "ProjectileAsset":
          case "ProjectileCount":
          case "AccuracyBase":
          case "AccuracyIncreaseOverDistance":
          case "AccuracySpeedDecay":
          case "TargetAngleVariation":
          case "ShootingTracking":
          case "TurretExplosionEffectAsset":
          case "MuzzleEffectAssets":
          case "Turrets":
          case "AttackRangeApproachPercentage":
          case "EmitProjectileTimeAfterStartAnimation":
          case "AccuracyByDistance":
          case "HealBuildingsPerMinute":
          case "InitializeForestMinTreeAreaPercent":

          //Ministary buffs (maybe todo)
          case "ElectricityRangeUpgrade":
            break;

          default:
            this.GenericUpgrades.Add(new Upgrade(element) { Category = category });
            break;
        }
    }

    private void ProcessElement_GenericUpgradeChilds(XElement element, string category) {
      if (element.HasElements) {
        foreach (var item in element.Elements()) {
          ProcessElement_GenericUpgradeElement(item, category);
        }
      }
    }

    private void ProcessElement_Upgradable(XElement element) {
      if (element.HasElements && (element.Element("NextGUID")?.Value ?? "0") != "0") {
        this.UpgradeText = new Description("10580").Remove("&lt;br/&gt;[AssetData([Conditions QuestCondition Context]) Text] [Conditions QuestCondition CurrentAmount]/[Conditions QuestCondition Amount]");
        this.UpgradeText.AppendWithSpace(new Description(element.Element("NextGUID").Value));

        this.UpgradeCosts = new List<Upgrade>();
        foreach (var item in element.Descendants("Item").Where(i => i.Element("Amount")?.Value != null)) {
          this.UpgradeCosts.Add(new Upgrade() { Text = new Description(item.Element("Ingredient").Value), Value = item.Element("Amount").Value });
        }
      }
    }

    private void ProcessElement_Cost(XElement element) {
      if (element.HasElements) {
        this.BuildCosts = new List<Upgrade>();
        foreach (var item in element.Descendants("Item").Where(i => i.Element("Amount")?.Value != null)) {
          if (item.Element("Ingredient")?.Value is string value) {
            this.BuildCosts.Add(new Upgrade() { Text = new Description(value), Value = item.Element("Amount").Value });
          }
          else {
            var index = element.Element("Costs").Elements().ToList().IndexOf(item);
            var ingedient = Assets.DefaultValues["Cost"].Element("Costs").Elements().ElementAt(index).Element("Ingredient").Value;
            this.BuildCosts.Add(new Upgrade() { Text = new Description(ingedient), Value = item.Element("Amount").Value });
          }
        }
        if (element.Element("InfluenceCostPoints")?.Value is string influencecost) {
          this.BuildCosts.Add(new Upgrade() { Text = new Description("1010190"), Value = influencecost });
        }
      }
    }

    private void ProcessElement_Pausable(XElement element) {
      if (element.Element("CanPauseManually")?.Value == "1") {
        this.IsPausable = true;
      }
    }

    private void ProcessElement_PopulationUpgrade(XElement element) {
      if (element.HasElements) {
        this.PopulationUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          switch (item.Name.LocalName) {
            case "InputBenefitModifier":
              var buffs = item.Elements("Item").SelectMany(e => e.Elements().Where(ele => ele.Name.LocalName != "Product"));
              foreach (var buffname in buffs.Select(b => b.Name.LocalName).Distinct()) {
                var firstBuff = new Upgrade(buffs.FirstOrDefault(b => b.Name.LocalName == buffname));
                firstBuff.Additionals = new List<Upgrade>();
                firstBuff.Value = null;
                foreach (var buff in buffs.Where(b => b.Name.LocalName == buffname)) {
                  var secBuff = new Upgrade(buff);
                  secBuff.Text = new Description(buff.Parent.Element("Product").Value);
                  firstBuff.Additionals.Add(secBuff);
                }
                PopulationUpgrades.Add(firstBuff);
              }
              break;

            default:
              this.PopulationUpgrades.Add(new Upgrade(item));
              break;
          }
        }
      }
    }

    private void ProcessElement_ExpeditionAttribute(XElement element) {
      if (element.HasElements) {
        var attributes = element.XPathSelectElements("ExpeditionAttributes/Item").Where(w => w.HasElements).ToArray();
        if (attributes.Length > 0) {
          this.ExpeditionAttributes = new List<Upgrade>();
          foreach (var attribute in attributes) {
            if (attribute.Element("Attribute") == null)
              continue;
            if (attribute.Element("Attribute").Value == "PerkEntertainer")
              continue;
            this.ExpeditionAttributes.Add(new Upgrade(attribute.Element("Attribute").Value, attribute.Element("Amount")?.Value));
          }
        }
      }
    }

    private void ProcessElement_AttackerUpgrade(XElement element) {
      if (element.HasElements) {
        this.AttackerUpgrades = new List<Upgrade>();
        var projektile = element.Element("UseProjectile");
        if (projektile != null) {
          this.AttackerUpgrades.Add(new Upgrade(projektile));
          var Projectile = Assets
            .Original
            .Descendants("Asset")
            .FirstOrDefault(a => a.XPathSelectElement($"Values/Standard/GUID")?.Value == projektile.Value);
          if (Projectile.XPathSelectElement("Values/Exploder/InnerDamage")?.Value is string damage && damage != "0") {
            this.AttackerUpgrades.Add(new Upgrade() { Text = new Description("20621"), Value = damage });
          }
        }
        foreach (var item in element.Elements().Except(new[] { projektile })) {
          // TODO: this needs to be implemented
          if (item.Name.LocalName == "ReloadTimeUpgrade")
            continue;
          if (item.Name.LocalName == "AddStatusEffects")
            continue;
          if (item.Name.LocalName == "DamageFactor") {
            foreach (var factor in item.Elements()) {
              this.AttackerUpgrades.Add(new Upgrade(factor));
            }
            continue;
          }
          var upgrade = new Upgrade(item);
          if (upgrade.Text == null) {
            continue;
          }
          this.AttackerUpgrades.Add(upgrade);
        }
      }
    }

    private void ProcessElement_AttackableUpgrade(XElement element) {
      if (element.HasElements) {
        this.DefenceUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          switch (item.Name.LocalName) {
            case "DamageReceiveFactor":
              foreach (var subItem in item.Elements()) {
                this.DefenceUpgrades.Add(new Upgrade(subItem));
              }
              break;

            default:
              this.DefenceUpgrades.Add(new Upgrade(item));
              break;
          }
        }
      }
    }

    private void ProcessElement_ProjectileUpgrade(XElement element) {
      if (element.HasElements) {
        throw new NotImplementedException();
      }
    }

    private void ProcessElement_ItemActions(XElement element) {
      if (element.HasElements) {
        var itemAction = element.Element("ItemAction")?.Value ?? "NOACTION";
        Description ActionText = null;

        this.ItemActionUpgrades = new List<Upgrade>();

        if (element.Element("ActionDescription")?.Value is string desc) {
          ActionText = new Description(desc, DescriptionFontStyle.Light);
        }

        if (ActionText != null) {
          this.ItemActionUpgrades.Add(new Upgrade() { Text = ActionText, Value = element.Element("Charges")?.Value ?? "" });
        }

        if (itemAction == "KAMIKAZE") {
          this.ItemActionUpgrades.Add(new Upgrade() { Text = new Description("21347") { AdditionalInformation = new Description("21348", DescriptionFontStyle.Light) } });
          this.ItemActionUpgrades.Add(new Upgrade() { Text = new Description("21353") });
          return;
        }

        if (element.Element("ActiveBuff")?.Value is string buff) {
          this.ItemActionUpgrades.AddRange(Assets.Buffs[buff].AllUpgrades.ToList());
        }

        if (ActionText != null) {
          if (element.Element("ActionDuration")?.Value != null) {
            this.ItemActionUpgrades.Add(new Upgrade(element.Element("ActionDuration")));
          }
          if (element.Element("ActionCooldown")?.Value != null) {
            this.ItemActionUpgrades.Add(new Upgrade(element.Element("ActionCooldown")));
          }
          if (element.Element("IsDestroyedAfterCooldown")?.Value != null) {
            this.ItemActionUpgrades.Add(new Upgrade(element.Element("IsDestroyedAfterCooldown")));
          }
        }
      }
    }

    private void ProcessElement_MonumentEventCategory(XElement element) {
      if (element.HasElements) {
        this.MonumentEvents = element.XPathSelectElements("Events/Item/Event").Select(s => s.Value).ToList();
      }
    }

    private void ProcessElement_MonumentEvent(XElement element) {
      if (element.HasElements) {
        this.MonumentThresholds = element.XPathSelectElements("RewardThresholds/Item/Reward").Select(s => s.Value).ToList();
      }
    }

    private void ProcessElement_MonumentEventReward(XElement element) {
      if (element.HasElements) {
        this.MonumentRewards = element.XPathSelectElements("RewardAssets/Item/Reward").Select(s => s.Value).ToList();
      }
    }

    private void ProcessElement_CraftableItem(XElement element) {
      if (element.HasElements) {
        this.CraftableItemUpgrades = new List<Upgrade>();
        foreach (var item in element.Element("CraftingCosts").Elements()) {
          this.CraftableItemUpgrades.Add(new Upgrade() { Text = new Description(item.Element("Product").Value), Value = item.Element("Amount").Value });
        }
      }
    }

    private void ProcessElement_Electric(XElement element) {
      if (element.Element("MandatoryElectricity")?.Value == "1") {
        this.GenericUpgrades.Add(new Upgrade() { Text = new Description("12508") });
      }
      else if (element.Element("ProductivityBoost")?.Value != "0" && element.Element("BoostedByElectricity")?.Value != "0") {
        this.GenericUpgrades.Add(new Upgrade() { Text = new Description("10604") });
      }
    }

    private void ProcessElement_PublicService(XElement element) {
      if (element.HasElements && element.Element("FullSatisfactionDistance")?.Value is string value) {
        var radius = value;
        if (element.Element("NoSatisfactionDistance")?.Value is string max) {
          radius += $" / {max}";
        }
        GenericUpgrades.Add(new Upgrade { Text = new Description("12504"), Value = radius });
      }
    }

    private void ProcessElement_Powerplant(XElement element) {
      if (element.HasElements && element.Element("ElectricityDistance")?.Value is string value) {
        var radius = value;
        GenericUpgrades.Add(new Upgrade { Text = new Description("12504"), Value = radius });
      }
    }

    private void ProcessElement_Market(XElement element) {
      if (element.HasElements && element.Element("FullSupplyDistance")?.Value is string value) {
        var radius = value;
        if (element.Element("NoSupplyDistance")?.Value is string max) {
          radius += $" / {max}";
        }
        GenericUpgrades.Add(new Upgrade { Text = new Description("12504"), Value = radius });
      }
    }

    private void ProcessElement_DivingBellUpgrade(XElement element) {
      if (element.HasElements) {
        this.DivingBellUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          if (item.Name.LocalName == "AllocationWeightUpgrade") {
            var results = new Dictionary<string, Upgrade>();
            foreach (var AllocationWeight in item.Elements()) {
              string key = null;
              switch (AllocationWeight.Name.LocalName) {
                case "None":
                  key = "22230";

                  break;

                case "Zoo":
                  key = "22231";
                  break;

                case "Museum":
                  key = "22232";
                  break;

                default:
                  key = "22233";
                  break;
              }
              if (results.ContainsKey(key)) {
                results[key].Additionals.Add(new Upgrade() { Text = new Description(Assets.KeyToIdDict[AllocationWeight.Name.LocalName]), Value = $"+{AllocationWeight.Element("AdditionalWeight").Value}" });
              }
              else {
                results.Add(key,
                  new Upgrade() {
                    Text = new Description(key),
                    Additionals = new List<Upgrade>{
                      new Upgrade() {
                        Text = new Description(Assets.KeyToIdDict[AllocationWeight.Name.LocalName]),
                        Value = $"+{AllocationWeight.Element("AdditionalWeight").Value}"
                      }
                    }
                  });
              }
            }
            foreach (var result in results.Values) {
              this.DivingBellUpgrades.Add(result);
            }
          }
          else {
            this.DivingBellUpgrades.Add(new Upgrade(item));
          }
        }
      }
    }

    private void ProcessElement_ItemStartExpedition(XElement element) {
      if (element.HasElements) {
        this.ItemStartExpedition = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.ItemStartExpedition.Add(new Upgrade {
            Text = new Description("2637"),
            Additionals = new List<Upgrade> {
              new Upgrade {
                Text = new Description(element.Value)}
            }
          });
        }
      }
    }

    private void ProcessElement_ItemWithUI(XElement element) {
      if (element.HasElements) {
        var actions = element.XPathSelectElement("ItemActions/Values/ActionList/Actions")?.Elements("Item")?.Select(a => a.Element("Action"));
        if (actions != null) {
          this.ItemWithUI = new List<Upgrade>();
          foreach (var action in actions) {
            switch (action.Element("Template").Value) {
              case "ActionStartTreasureMapQuest":
                this.ItemWithUI.Add(new Upgrade(action));
                break;

              case "ActionAddResource":
                //Todo: new with update 05 maybe implement. (dokuments that becomes deko buildings)
                break;

              case "ActionTriggerTextPopup":
              case "ActionRegisterTrigger":
              case "ActionNotification":
              case "ActionReplaceItem":
                //Ignore
                break;

              default:
                Debug.WriteLine(action.Element("Template").Value);
                throw new NotImplementedException();
                break;
            }
          }
        }
      }
    }

    private ConcurrentBag<SourceWithDetailsList> FindSources(String id, Details mainDetails = default) {
      mainDetails = (mainDetails == default) ? new Details() : mainDetails;
      mainDetails.PreviousIDs.Add(id);
      var mainResult = new SourceWithDetailsList();
      var resultstoadd = new ConcurrentBag<SourceWithDetailsList>();
      var links = Assets.Original.XPathSelectElements($"//*[text()={id} and not(self::GUID)]").ToArray();
      if (links.Length > 0) {
        //links.AsParallel().ForAll(link => {
        foreach (var link2 in/* new[] { link }*/ links) {
          var element = link2;
          var foundedElement = element;

          //Weight 0
          if (element.Parent.Element("Weight")?.Value == "0") {
            continue;
          }

          //Ignores
          if (foundedElement.Name.LocalName is string foundedName &&
            (foundedName.MatchOne("BaseAssetGUID", "Icon", "ItemUsed", "TradePrice", "GenPool", "NotificationIcon", "ReplacingWorkforce", "ProductFilter"))) {
            continue;
          }
          if (foundedElement.Parent?.Parent?.Name.LocalName is string gparent &&
            (gparent.MatchOne("Costs", "UpgradeCost", "CraftingCosts", "Maintenances", "StoredProducts", "UnlockAssets"))) {
            continue;
          }
          if (foundedElement.Parent?.Parent?.Parent?.Name.LocalName is string ggParent &&
            (ggParent.MatchOne("FactoryBase", "Sellable", "PublicService"))) {
            continue;
          }

          //Search Parent Asset
          while (element.Name.LocalName != "Asset" || !element.HasElements) {
            element = element.Parent;
          }

          var Details = new Details(mainDetails);
          var result = new SourceWithDetailsList();
          var key = element.XPathSelectElement("Values/Standard/GUID").Value;

          if (Details.PreviousIDs.Contains(key)) {
            continue;
          }

          switch (element.Element("Template").Value) {
            case "AssetPool":
            case "TutorialQuest":
            case "SettlementRightsFeature":
            case "GuildhouseItem":
            case "MonumentEvent":
            case "MainQuest":
            case "WarShip":
            case "ExpeditionFeature":
            case "FestivalBuff":
            case "TownhallItem":
            case "PopulationLevel7":
            case "NeedsSatisfactionNews":
            case "ProductFilter":
            case "PlayerCounterContextPool":
            case "PublicServiceBuilding":
            case "Market":
            case "GuildhouseBuff":
            case "TriggerQuest":
            case "TradeRouteFeature":
            case "HarborWarehouse7":
            case "DifficultyBalancing":
            case "RewardConfig":
            case "UplayAction":
            case "NewspaperArticle":
            case "WorkforceNewsTracker":
            case "InfluenceTitleBuff":
            case "WorkforceMenu":
            case "TownhallBuff":
            case "AudioText":
            case "TradeShip":
            case "Achievement":
            case "Audio":
            case "WorkforceSliderNewsTracker":
            case "ChannelTarget":
            case "Region":
            case "KeywordFilter":
            case "ObjectmenuCommuterHarbourScene":
            case "IslandBarScene":
            case "UplayProduct":
            case "ProductStorageList":
              // ignore
              break;

            case "TriggerCampaign":
            case "Trigger":
            case "PassiveTradeFeature":
              //Todo?
              break;

            case "Expedition":
              if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (!foundedElement.Name.LocalName.MatchOne("FillEventPool", "Reward", "EventOrEventPool")) {
                  break;
                }

                result.AddSourceAsset(element, new HashSet<AssetWithWeight> { new AssetWithWeight(element) });
              }
              break;

            case "Profile_3rdParty_ItemCrafter":
            case "Profile_3rdParty":
            case "Profile_3rdParty_Pirate":
            case "Profile_2ndParty":
              if (key.MatchOne("199", "200", "240", "117422")) {
                break;
              }
              if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (foundedElement.Name.LocalName.MatchOne("ShipDropRewardPool")) {
                  result.AddSourceAsset(element.GetProxyElement("ShipDrop"), new HashSet<AssetWithWeight> { new AssetWithWeight(element) });
                  break;
                }

                if (foundedElement.Name.LocalName == "OfferingItems" ||
                  foundedElement.Parent.Parent.Name.LocalName.MatchOne("ShipsForSale", "GoodSets")) {
                  var oldParent = foundedElement.Parent;
                  var parent = foundedElement.Parent;
                  while (parent.Name.LocalName != "Progression") {
                    oldParent = parent;
                    parent = parent.Parent;
                  }

                  result.AddSourceAsset(element.GetProxyElement("Harbor"), new HashSet<AssetWithWeight> { new AssetWithWeight(element.GetProxyElement(oldParent.Name.LocalName)) });
                  break;
                }

                if (foundedElement.Name.LocalName == "Pool" && foundedElement.Parent.Parent.Name.LocalName == "ItemPools") {
                  result.AddSourceAsset(element.GetProxyElement("Harbor"), new HashSet<AssetWithWeight> { new AssetWithWeight(element.GetProxyElement(foundedElement.Parent.Name.LocalName)) });
                  break;
                }

                if (foundedElement.Name.LocalName.MatchOne("MainIslandRewardPool", "SecondaryIslandRewardPool")) {
                  result.AddSourceAsset(element.GetProxyElement("TakeOver"), new HashSet<AssetWithWeight> { new AssetWithWeight(element.GetProxyElement($"{foundedElement.Name.LocalName}#{foundedElement.Parent.Name}")) });
                  break;
                }

                if (foundedElement.Name.LocalName == "Item" && foundedElement.Parent.Parent.Name.LocalName == "CraftableItems") {
                  result.AddSourceAsset(element.GetProxyElement("Crafting"), new HashSet<AssetWithWeight> { new AssetWithWeight(element.GetProxyElement(foundedElement.Parent.Parent.Parent.Name.LocalName)) });
                  break;
                }
                if (foundedElement.Name.LocalName.MatchOne("ItemPool", "Product", "Good")) {
                  break;
                }
                else {
                  throw new NotImplementedException();
                }
              }
              else {
                break;
              }
            case "A7_QuestEscortObject":
            case "A7_QuestDeliveryObject":
            case "A7_QuestDestroyObjects":
            case "A7_QuestPickupObject":
            case "A7_QuestFollowShip":
            case "A7_QuestPhotography":
            case "A7_QuestStatusQuo":
            case "A7_QuestItemUsage":
            case "A7_QuestSustain":
            case "A7_QuestPicturePuzzleObject":
            case "Quest":
            case "CollectablePicturePuzzle":
            case "MonumentEventReward":
            case "A7_QuestSmuggler":
            case "A7_QuestDivingBellGeneric":
            case "A7_QuestDivingBellSonar":
            case "A7_QuestSelectObject":
            case "A7_QuestDivingBellTreasureMap":
            case "A7_QuestNewspaperArticle":
            case "A7_QuestLostCargo":
            case "A7_QuestExpedition":
              if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (foundedElement.Name.LocalName.MatchOne("Reward")) {
                  result.AddSourceAsset(element, new HashSet<AssetWithWeight> { new AssetWithWeight(element) });
                }
              }
              break;

            case "DivingBellShip":
              //Ignore second divingbellship. its for a quest
              if (key == "113710") {
                break;
              }
              if (foundedElement.Name.LocalName == "ItemReplacementPools") {
                result.AddSourceAsset(element.GetProxyElement("Dive"), new HashSet<AssetWithWeight> { new AssetWithWeight(element) });
              }
              break;

            case "AirShip":
              if (foundedElement.Name.LocalName == "ItemReplacementPools") {
                result.AddSourceAsset(element.GetProxyElement("Pickup"), new HashSet<AssetWithWeight> { new AssetWithWeight(element) });
              }
              break;

            case "ItemWithUI":
              if (foundedElement.Name.LocalName == "NewItem") {
                result.AddSourceAsset(element, new HashSet<AssetWithWeight> { new AssetWithWeight(element) });
              }
              else if (foundedElement.Name.LocalName == "Ressource" && foundedElement.Parent.Name.LocalName == "ActionAddResource") {
                result.AddSourceAsset(element.GetProxyElement("Item"), new HashSet<AssetWithWeight> { new AssetWithWeight(element) });
              }
              break;

            case "TourismFeature":
              if (foundedElement.Name.LocalName == "Pool") {
                var pool = foundedElement.Parent;
                result.AddSourceAsset(element, new HashSet<AssetWithWeight> { new AssetWithWeight(pool.GetProxyElement(foundedElement.Parent.Parent.Parent.Name.LocalName)) });
              }
              break;

            case "CultureBuff":
            case "HarborOfficeItem":
              if (foundedElement.Name.LocalName == "OverrideSpecialistPool") {
                result.AddSourceAsset(Assets.TourismAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(element) });
              }
              else {
                throw new NotImplementedException();
              }
              break;

            case "ItemReplacementPool":
              if (foundedElement.Name.LocalName != "ReplacementPool") {
                break;
              }
              else {
                goto case "SearchAgain";
              }

            case "ExpeditionDecision":
            case "ExpeditionTrade":
              if (foundedElement.Name.LocalName.MatchOne("Reward", "Product", "Item")) {
                var tempresults = GetSources(Details, key);
                foreach (var sourceWithDetails in tempresults) {
                  if (sourceWithDetails.FollowingEvents.Count > 0) {
                    foreach (var expedition in sourceWithDetails.Where(r => r.Source.Element("Template").Value == "Expedition")) {
                      expedition.Details.Clear();
                      foreach (var item in sourceWithDetails.FollowingEvents) {
                        var path = element.XPathSelectElement("Values/Standard/Name").Value.Split(' ').Last();
                        expedition.Details.Add(new AssetWithWeight(item.GetProxyElement(path)));
                      }
                    }
                  }
                }
                tempresults.SaveSource(key).MergeResults(key, in result);

                break;
              }
              else if (!foundedElement.Name.LocalName.MatchOne("Option", "FollowupSuccessOption", "FollowupFailOrCancelOption", "InsertEvent")) {
                break;
              }
              goto case "SearchAgain";

            case "ExpeditionOption":
            case "ExpeditionMapOption":
              if (foundedElement.Name.LocalName != "Decision") {
                break;
              }
              goto case "SearchAgain";

            case "ExpeditionEvent":
              if (foundedElement.Name.LocalName != "StartDecision") {
                break;
              }

              result.FollowingEvents.Add(element);

              goto case "SearchAgain";

            case "ExpeditionBribe":
              if (foundedElement.Name.LocalName == "Item") {
                break;
              }
              if (!foundedElement.Name.LocalName.MatchOne("FollowupSuccessOption", "FollowupFailOrCancelOption")) {
                break;
              }
              goto case "SearchAgain";

            case "RewardPool":
            case "RewardItemPool":
              var itemList = foundedElement.Parent.Parent.Elements().ToList();
              var weightSum = itemList.Sum(item => item.Element("Weight")?.Value is string str ? double.Parse(str) : 1.0F);

              foreach (var item in itemList.Where(i => i.Element("ItemLink")?.Value != null).ToLookup(i => i.Element("ItemLink").Value)) {
                result.ElementWeights.Add(item.Key, item.Sum(i => (i.Element("Weight")?.Value is string str ? double.Parse(str) : 1.0F) / weightSum));
              }

              goto case "SearchAgain";

            case "ExpeditionEventPool":
            case "ResourcePool":
            case "A7_QuestSubQuest":
            case "ProductList":
            case "SearchAgain":
              GetSources(Details, key).SaveSource(key).MergeResults(key, in result);
              break;

            default:
              //throw new NotImplementedException(element.Element("Template").Value);
              Debug.WriteLine(element.Element("Template").Value);
              break;
          }
          if (result.Any()) {
            resultstoadd.Add(result);
          }
        }
        //});
      }

      return resultstoadd;
    }

    private ConcurrentBag<SourceWithDetailsList> GetSources(Details Details, string key) {
      ConcurrentBag<SourceWithDetailsList> tempresult = null;
      if (SavedSources.TryGetValue(key, out var saved)) {
        tempresult = new ConcurrentBag<SourceWithDetailsList>(saved.Select(i => i.Copy()));
      }
      else {
        tempresult = new ConcurrentBag<SourceWithDetailsList>(FindSources(key, Details).Select(i => i.Copy()));
      }

      return tempresult;
    }

    #endregion Private Methods
  }
}