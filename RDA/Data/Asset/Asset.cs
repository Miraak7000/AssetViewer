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
    public string ID { get; set; }
    public string Name { get; set; }
    public Description Text { get; set; }
    public Description UpgradeText { get; set; }
    public Description AssociatedRegions { get; set; }
    public string RarityType { get; private set; } = "Common";
    public Description Rarity { get; set; }
    public string ItemType { get; set; }
    public string ReleaseVersion { get; set; } = "Release";
    public string TradePrice { get; set; }
    public string HiringFee { get; private set; }
    public Description Info { get; set; }
    public Modules Modules { get; set; }
    public bool IsPausable { get; set; }
    public bool IsResearchable { get; set; }
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

    public List<string> MonumentEvents { get; set; }
    public List<string> MonumentThresholds { get; set; }
    public List<string> MonumentRewards { get; set; }
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

    public Asset(XElement asset, bool findSources) {
      //Set Item Typ
      ItemType = asset.Element("Item")?.Element("ItemType")?.Value;
      if (ItemType == "Normal" || ItemType == "None" || string.IsNullOrWhiteSpace(ItemType)) {
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
            ItemType = "Item";
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
          case "BuffFactory":
          case "IrrigationPropagationSource":
          case "WorkAreaRiverBuilding":
          case "CityInstitutionBuilding_Africa":
          case "HarborLandingStage7_BuildPermit":
          case "FertilizerBaseBuilding":
          case "ResidenceBuilding7_BuildPermit":
          case "FactoryBuilding7_BuildPermit":
          case "ResearchCenter":
          case "WorkAreaRiverSlot":
          case "QuestObjectInfectable":
          case "QuestObjectHarborBuildingAttacker":
          case "DocklandMain":
          case "HarborOrnament":
          case "ResidenceBuilding":
          case "PublicServiceBuildingWithBus":
          case "ResidenceBuilding7_Colony":
          case "Restaurant":
          case "TowerRestaurant":
          case "Multifactory":
          case "FreeAreaRecipeBuilding":
          case "Busstop":
          case "ResidenceBuilding7_Hotel":
          case "CultureBuildingColony":
          case "VisitorPierColony":
          case "Mall":
          case "ResidenceBuilding-Unique":
          case "ItemCrafterHarbor":
          case "FreeAreaBuilding_BuildPermit":
          case "FarmBuilding_BuildPermit":
          case "ScenarioRuin":
            ItemType = "Building";
            break;

          case "BuildPermitModules":
          case "CultureModule":
          case "OrnamentalModule":
          case "Farmfield":
          case "PalaceModule":
          case "AdditionalModule":
          case "FertilizerBaseModule":
          case "DocklandPierModule":
          case "DocklandItemModule":
          case "DocklandModule":
          case "DocklandStorageModule":
          case "DocklandModuleRepair":
            ItemType = "Module";
            break;

          case "CultureItem":
            ItemType = "Animal";
            break;

          case "Product":
          case "Resource":
            ItemType = "Product";
            break;

          case "FluffItem":
            ItemType = "Character Item";
            break;

          case "ItemWithUI":
            ItemType = "Document";
            break;

          case "QuestItem":
          case "QuestItemMagistrate":
            ItemType = "Quest Item";
            break;

          case "ItemSet":
            ItemType = "Item Set";
            break;

          default:
            Debug.WriteLine(asset.Element("Template").Value);

           throw new NotImplementedException(asset.Element("Template").Value);
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
          case "WorkAreaPath":
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
          case "MonoCulture":
          // Todo: needs to implemented

          case "Product":
          case "Watered":
          case "Collectable":
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
          case "Industrializable":
          case "Electrifiable":
          case "PalaceMonumentTracker":
          case "BuffFactory":
          case "Dockland":
          case "Nameable":
          //Maybe usefull Building Informations
          case "InfluenceSource":
          case "BusActivation":
          case "RecipeBuilding":
          //Building influence gain
          case "EffectForward":
          case "MonumentUpgrade":
          case "InfluenceSourceUpgrade":
          case "PalaceMinistry":
          case "Palace":
          //Ministary stuff

          case "FloorStackOwner": //Update 11 nothing inside
          case "BusStop": //Costs
            break;

          case "Standard":
            ProcessElement_Standard(element);
            break;

          case "Item":
            ProcessElement_Item(element);
            break;

          case "ItemEffect":
            ProcessElement_ItemEffect(element);
            break;

          case "Building": //Building category, Terrain
            ProcessElement_Building(element);
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
          case "IrrigationUpgrade":
          //Buildings
          case "Culture":
          case "LoadingPier":
          case "Attacker":
          case "Shipyard":
          case "RepairCrane":
          case "TrainStation":
          case "PowerplantUpgrade":
          case "IndustrializableUpgrade":
          case "Motorizable":
          case "ResearchCenter":
          case "ResourceUpgrade":
          case "EcoSystemProviderUpgrade":
            ProcessElement_GenericUpgradeChilds(element, element.Name.LocalName);
            break;

          case "PassiveTradeGoodGenUpgrade":
          //Buildings
          case "Residence7":
          case "EcoSystemUpgrade":
          case "EcoSystemProvider":
            ProcessElement_GenericUpgradeElement(element, element.Name.LocalName);
            break;

          //Non Generics
          case "PopulationUpgrade":
            ProcessElement_PopulationUpgrade(element);
            break;

          case "ExpeditionAttribute":
            ProcessElement_ExpeditionAttribute(element);
            break;

          case "AttackerUpgrade":
            ProcessElement_AttackerUpgrade(element);
            break;

          case "AttackableUpgrade":
            ProcessElement_AttackableUpgrade(element);
            break;

          case "ProjectileUpgrade":
            ProcessElement_ProjectileUpgrade(element);
            break;

          case "ItemAction":
            ProcessElement_ItemActions(element);
            break;

          case "MonumentEventCategory":
            ProcessElement_MonumentEventCategory(element);
            break;

          case "MonumentEvent":
            ProcessElement_MonumentEvent(element);
            break;

          case "Ornament":
            Info = element.Element("OrnamentDescritpion") == null ? null : new Description(element.Element("OrnamentDescritpion").Value);
            break;

          case "Reward":
            ProcessElement_MonumentEventReward(element);
            break;

          case "DivingBellUpgrade":
            ProcessElement_DivingBellUpgrade(element);
            break;

          case "CraftableItem":
            ProcessElement_CraftableItem(element);
            break;

          case "ItemWithUI":
            ProcessElement_ItemWithUI(element);
            break;

          case "ItemStartExpedition":
            ProcessElement_ItemStartExpedition(element);
            break;

          case "ItemSocketSet":
            ProcessElement_ItemSocketSet(element);
            break;

          //Buildings
          case "Upgradable":
            ProcessElement_Upgradable(element);
            break;

          case "Cost":
            ProcessElement_Cost(element);
            break;

          case "Pausable":
            ProcessElement_Pausable(element);
            break;

          case "FactoryBase":
            FactoryBase = new FactoryBase(element);
            break;

          case "ModuleOwner":
            Modules = new Modules(element);
            break;

          case "Maintenance":
            Maintenance = new Maintenance(element);
            break;

          case "Electric":
            ProcessElement_Electric(element);
            break;

          case "PublicService":
            ProcessElement_PublicService(element);
            break;

          case "Powerplant":
            ProcessElement_Powerplant(element);
            break;

          case "Market":
            ProcessElement_Market(element);
            break;

          case "Heated":
            ProcessElement_Heated(element);
            break;

          case "Warehouse":
            ProcessElement_Warehouse(element);
            break;

          case "FreeAreaProductivity":
            ProcessElement_FreeAreaProductivity(element);
            break;

          case "Monument":
            ProcessElement_Monument(element);
            break;

          case "ModuleIrrigation":
            ProcessElement_ModuleIrrigation(element);
            break;

          case "IrrigationSource":
            ProcessElement_IrrigationSource(element);
            break;

          default:
            Debug.WriteLine(element.Name.LocalName);
            //throw new NotImplementedException(element.Name.LocalName);
            break; 
        }
      }
      if (findSources) {
        var results = FindSources(ID, asset);

        FilterOutRegions(asset, results);
        var sources = results.MergeResults(ID);

        if (IsResearchable) {
          sources.AddSourceAsset(Assets.ResearchFeatureAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(Assets.ResearchFeatureAsset) });
        }

        Sources = sources.Select(s => new TempSource(s)).ToList();
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("Asset");
      result.Add(new XAttribute("ID", ID));
      result.Add(new XAttribute("RV", ReleaseVersion));
      result.Add(new XElement("N", Name));
      result.Add(Text.ToXml("T"));
      result.Add(Rarity == null ? new Description(Assets.KeyToIdDict["Common"]).ToXml("R") : Rarity.ToXml("R"));
      result.Add(new XAttribute("RT", RarityType));
      result.Add(new XElement("IT", ItemType));
      if (Sources?.Any(s => s.IsRollable) ?? false) {
        result.Add(new XAttribute("IR", true));
      }
      if (FestivalName != null) {
        result.Add(FestivalName.ToXml("FN"));
      }
      if (Allocation != null) {
        result.Add(Allocation.ToXml());
      }
      if (AssociatedRegions != null) {
        result.Add(AssociatedRegions.ToXml("AR"));
      }

      var type = typeof(Asset);
      foreach (var item in type.GetProperties().Where(p => p.PropertyType == typeof(List<Upgrade>))) {
        if (item.GetValue(this) != null) {
          var builder = new StringBuilder();
          foreach (var character in item.Name) {
            if (char.IsUpper(character)) {
              builder.Append(character);
            }
          }
          result.Add(new XElement(builder.ToString(), (item.GetValue(this) as List<Upgrade>)?.Select(s => s.ToXml())));
        }
      }

      result.Add(new XElement("TP", TradePrice));
      result.Add(new XElement("HF", HiringFee));

      if (Info != null)
        result.Add(Info.ToXml("I"));
      if (FactoryBase != null)
        result.Add(FactoryBase.ToXml());
      if (Maintenance != null)
        result.Add(Maintenance.ToXml());
      if (UpgradeText != null)
        result.Add(UpgradeText.ToXml("UT"));
      if (Modules != null)
        result.Add(Modules.ToXml());
      if (IsPausable)
        result.Add(new XAttribute("IP", true));

      if (EffectTargets != null)
        result.Add(new XElement("ET", EffectTargets.Select(s => s.ToXml())));
      if (Sources != null)
        result.Add(new XElement("S", Sources?.Select(s => s.ToXml())));

      if (MonumentEvents != null)
        result.Add(new XElement("ME", MonumentEvents?.Select(s => new XElement("E", s))));
      if (MonumentThresholds != null)
        result.Add(new XElement("MT", MonumentThresholds?.Select(s => new XElement("T", s))));
      if (MonumentRewards != null)
        result.Add(new XElement("MR", MonumentRewards?.Select(s => new XElement("R", s))));
      if (SetParts != null)
        result.Add(new XElement("SP", SetParts?.Select(s => new XElement("P", s))));
      return result;
    }

    public override string ToString() {
      return $"{ID} - {Name}";
    }

    #endregion Public Methods

    #region Private Methods

    private static void FilterOutRegions(XElement asset, ConcurrentBag<SourceWithDetailsList> results) {
      //Region Filter
      var toDelete = new List<SourceWithDetails>();
      foreach (var sourceWithDetails in results) {
        foreach (var expedition in sourceWithDetails.Where(r => r.Source.Element("Template").Value == "Expedition")) {
          expedition.Details.RemoveWhere(d => RegionFilter(d, asset));
          if (expedition.Details.Count == 0) {
            toDelete.Add(expedition);
          }
        }
        foreach (var item in toDelete) {
          sourceWithDetails.Remove(item);
        }
      }
    }

    private static bool RegionFilter(AssetWithWeight assetWithWeight, XElement asset) {
      var region = assetWithWeight.Asset.XPathSelectElement("Values/Expedition/ExpeditionRegion")?.Value ?? "None";
      var difficulty = assetWithWeight.Asset.XPathSelectElement("Values/Expedition/ExpeditionDifficulty")?.Value ?? "Easy";

      var assetRegions = asset.XPathSelectElement("Values/ExpeditionAttribute/ItemRegions")?.Value ?? "None";
      var assetDiffs = asset.XPathSelectElement("Values/ExpeditionAttribute/ItemDifficulties")?.Value ?? "None";
      if (assetRegions == "None" || (assetRegions.Contains(region) && (assetDiffs == "None" || assetDiffs.Contains(difficulty)))) {
        return false;
      }
      return true;

      //return !((asset.XPathSelectElement("Values/ExpeditionAttribute/ItemRegions")?.Value.Contains(region) ?? true) &&
      //    (asset.XPathSelectElement("Values/ExpeditionAttribute/ItemDifficulties")?.Value.Contains(difficulty) ?? true));
    }

    private void ProcessElement_Building(XElement element) {
      if (element.Element("AssociatedRegions")?.Value is string str) {
        var regions = str.Replace("Meta", "").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(s => new Description(Assets.KeyToIdDict[s]));
        AssociatedRegions = Description.Join(regions, ", ");
      }
    }

    private void ProcessElement_Monument(XElement element) {
      if ((element.Element("UpgradeTarget")?.Value ?? "0") != "0") {
        UpgradeText = new Description("10580").Remove("&lt;br/&gt;[AssetData([Conditions QuestCondition Context]) Text] [Conditions QuestCondition CurrentAmount]/[Conditions QuestCondition Amount]");
        UpgradeText.AppendWithSpace(new Description(element.Element("UpgradeTarget").Value));
      }
    }

    private void ProcessElement_ModuleIrrigation(XElement element) {
      if ((element.Element("RequiresIrrigation")?.Value ?? "0") != "0") {
        GenericUpgrades.Add(new Upgrade { Text = new Description("117782") });
      }
    }

    private void ProcessElement_IrrigationSource(XElement element) {
      if (element.Element("PipeFillCapacity")?.Value is string value) {
        GenericUpgrades.Add(new Upgrade { Text = new Description("124958"), Value = value });
      }
    }

    private void ProcessElement_Heated(XElement element) {
      if (element.Element("RequiresHeat")?.Value == "1") {
        GenericUpgrades.Add(new Upgrade { Text = new Description("116353") });
      }
    }

    private void ProcessElement_FreeAreaProductivity(XElement element) {
      if (element.Element("InfluenceRadius")?.Value is string value) {
        var radius = value;
        if (element.Element("NeededAreaPercent")?.Value is string needed) {
          radius += $" / {needed}%";
        }
        GenericUpgrades.Add(new Upgrade { Text = new Description("12504"), Value = radius });
      }
    }

    private void ProcessElement_Warehouse(XElement element) {
      if (element.Element("WarehouseStorage")?.Element("StorageMax")?.Value is string value && value != "0") {
        GenericUpgrades.Add(new Upgrade { Text = new Description("22426"), Value = "+" + value });
      }
    }

    private void ProcessElement_ItemSocketSet(XElement element) {
      ItemSocketSet = new List<Upgrade>();
      if (element.Element("SetBuff")?.Value is string buff) {
        ItemSocketSet.AddRange(Assets.Buffs[buff].AllUpgrades.ToList());
        EffectTargets = Assets.Buffs[buff].EffectTargets;
      }
      else if (element.Element("RegionSetBuff") is XElement regionBuffs) {
        EffectTargets = new List<EffectTarget>();
        foreach (var region in regionBuffs.Elements()) {
          switch (region.Name.LocalName) {
            case "Moderate":
              ItemSocketSet.Add(new Upgrade { Text = new Description("113322"), Additionals = Assets.Buffs[region.Element("SetBuff").Value].AllUpgrades.ToList() });
              break;

            case "Colony01":
              ItemSocketSet.Add(new Upgrade { Text = new Description("113395"), Additionals = Assets.Buffs[region.Element("SetBuff").Value].AllUpgrades.ToList() });
              break;

            default:
              throw new NotImplementedException();
          }
          EffectTargets.AddRange(Assets.Buffs[region.Element("SetBuff").Value].EffectTargets);
        }
      }
      var allocation = Assets.All.Descendants("Asset")
        .FirstOrDefault(a => a.Descendants("Set").Any(s => s.Value == ID));
      if (allocation != null) {
        Allocation = new Allocation(allocation.XPathSelectElement("Values/Standard/GUID").Value, null);
      }
    }

    private void ProcessElement_Standard(XElement element) {
      ID = element.Element("GUID").Value;
      Name = element.Element("Name").Value;
      Text = new Description(element.Element("GUID").Value);
      Info = element.Element("InfoDescription") == null ? null : new Description(element.Element("InfoDescription").Value);
    }

    private void ProcessElement_Item(XElement element) {
      RarityType = element.Element("Rarity")?.Value ?? "Common";
      Rarity = element.Element("Rarity") == null ? new Description("118002") : new Description(Assets.GetDescriptionID(element.Element("Rarity").Value));
      Allocation = new Allocation(element.Parent.Parent.Element("Template").Value, element.Element("Allocation")?.Value);
      IsResearchable = RarityType == "Common";
      if (element.Element("Allocation") == null) {
        element.Add(new XElement("Allocation", Allocation.ID));
      }
      TradePrice = element.Element("TradePrice") == null ? null : (int.Parse(element.Element("TradePrice").Value) / 4).ToString();
      HiringFee = element.Element("TradePrice") == null ? null : (int.Parse(element.Element("TradePrice").Value)).ToString();
      if (element.Element("ItemSet") != null) {
        ItemSets = new List<Upgrade> {
          new Upgrade(element.Element("ItemSet"))
        };
      }
    }

    private void ProcessElement_ItemEffect(XElement element) {
      if (element.HasElements) {
        if (element.Element("EffectTargets") == null)
          throw new NotImplementedException();
        if (element.Element("EffectTargets").HasElements) {
          EffectTargets = new List<EffectTarget>();
          foreach (var item in element.Element("EffectTargets").Elements()?.Where(e=> !string.IsNullOrWhiteSpace(e.Value))) {
            EffectTargets.Add(new EffectTarget(item));
          }
        }
      }
    }

    private void ProcessElement_GenericUpgradeElement(XElement element, string category) {
      if (string.IsNullOrEmpty(element.Value))
        return;
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
        case "AllowExclusiveTrading":
        case "WeaponActivationTime":
        case "Feedback_AllowSpawnAtEntrance":

        //Ministary buffs (maybe todo)
        case "ElectricityRangeUpgrade":
          break;

        default:
          GenericUpgrades.Add(new Upgrade(element) { Category = category });
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
        UpgradeText = new Description("10580").Remove("&lt;br/&gt;[AssetData([Conditions QuestCondition Context]) Text] [Conditions QuestCondition CurrentAmount]/[Conditions QuestCondition Amount]");
        UpgradeText.AppendWithSpace(new Description(element.Element("NextGUID").Value));

        UpgradeCosts = new List<Upgrade>();
        foreach (var item in element.Descendants("Item").Where(i => i.Element("Amount")?.Value != null)) {
          UpgradeCosts.Add(new Upgrade { Text = new Description(item.Element("Ingredient").Value), Value = item.Element("Amount").Value });
        }
      }
    }

    private void ProcessElement_Cost(XElement element) {
      if (element.HasElements) {
        BuildCosts = new List<Upgrade>();
        foreach (var item in element.Descendants("Item").Where(i => i.Element("Amount")?.Value != null)) {
          if (item.Element("Ingredient")?.Value is string value) {
            BuildCosts.Add(new Upgrade { Text = new Description(value), Value = item.Element("Amount").Value });
          }
          else {
            var index = element.Element("Costs").Elements().ToList().IndexOf(item);
            var ingedient = Assets.DefaultValues["Cost"].Element("Costs").Elements().ElementAt(index).Element("Ingredient").Value;
            BuildCosts.Add(new Upgrade { Text = new Description(ingedient), Value = item.Element("Amount").Value });
          }
        }
        if (element.Element("InfluenceCostPoints")?.Value is string influencecost) {
          BuildCosts.Add(new Upgrade { Text = new Description("1010190"), Value = influencecost });
        }
      }
    }

    private void ProcessElement_Pausable(XElement element) {
      if (element.Element("CanPauseManually")?.Value == "1") {
        IsPausable = true;
      }
    }

    private void ProcessElement_PopulationUpgrade(XElement element) {
      if (element.HasElements) {
        PopulationUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          switch (item.Name.LocalName) {
            case "InputBenefitModifier":
              var buffs = item.Elements("Item").SelectMany(e => e.Elements().Where(ele => ele.Name.LocalName != "Product"));
              foreach (var buffname in buffs.Select(b => b.Name.LocalName).Distinct()) {
                var firstBuff = new Upgrade(buffs.FirstOrDefault(b => b.Name.LocalName == buffname)) {
                  Additionals = new List<Upgrade>(),
                  Value = null
                };
                foreach (var buff in buffs.Where(b => b.Name.LocalName == buffname)) {
                  var secBuff = new Upgrade(buff) {
                    Text = new Description(buff.Parent.Element("Product").Value)
                  };
                  firstBuff.Additionals.Add(secBuff);
                }
                PopulationUpgrades.Add(firstBuff);
              }
              break;

            default:
              PopulationUpgrades.Add(new Upgrade(item));
              break;
          }
        }
      }
    }

    private void ProcessElement_ExpeditionAttribute(XElement element) {
      if (element.HasElements) {
        var attributes = element.XPathSelectElements("ExpeditionAttributes/Item").Where(w => w.HasElements).ToArray();
        if (attributes.Length > 0) {
          ExpeditionAttributes = new List<Upgrade>();
          foreach (var attribute in attributes) {
            if (attribute.Element("Attribute") == null)
              continue;
            if (attribute.Element("Attribute").Value == "PerkEntertainer")
              continue;
            ExpeditionAttributes.Add(new Upgrade(attribute.Element("Attribute").Value, attribute.Element("Amount")?.Value));
          }
        }
      }
    }

    private void ProcessElement_AttackerUpgrade(XElement element) {
      if (element.HasElements) {
        AttackerUpgrades = new List<Upgrade>();
        var projektile = element.Element("UseProjectile");
        if (projektile != null) {
          AttackerUpgrades.Add(new Upgrade(projektile));
          var Projectile = Assets
            .All
            .Descendants("Asset")
            .FirstOrDefault(a => a.XPathSelectElement($"Values/Standard/GUID")?.Value == projektile.Value);
          if (Projectile.XPathSelectElement("Values/Exploder/InnerDamage")?.Value is string damage && damage != "0") {
            AttackerUpgrades.Add(new Upgrade { Text = new Description("20621"), Value = damage });
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
              AttackerUpgrades.Add(new Upgrade(factor));
            }
            continue;
          }
          var upgrade = new Upgrade(item);
          if (upgrade.Text == null) {
            continue;
          }
          AttackerUpgrades.Add(upgrade);
        }
      }
    }

    private void ProcessElement_AttackableUpgrade(XElement element) {
      if (element.HasElements) {
        DefenceUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          switch (item.Name.LocalName) {
            case "DamageReceiveFactor":
              foreach (var subItem in item.Elements()) {
                DefenceUpgrades.Add(new Upgrade(subItem));
              }
              break;

            default:
              DefenceUpgrades.Add(new Upgrade(item));
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

        ItemActionUpgrades = new List<Upgrade>();

        if (element.Element("ActionDescription")?.Value is string desc) {
          ActionText = new Description(desc, DescriptionFontStyle.Light);
        }

        if (ActionText != null) {
          ItemActionUpgrades.Add(new Upgrade { Text = ActionText, Value = element.Element("Charges")?.Value ?? "" });
        }

        if (itemAction == "KAMIKAZE") {
          ItemActionUpgrades.Add(new Upgrade { Text = new Description("21347") { AdditionalInformation = new Description("21348", DescriptionFontStyle.Light) } });
          ItemActionUpgrades.Add(new Upgrade { Text = new Description("21353") });
          return;
        }

        if (element.Element("ActiveBuff")?.Value is string buff) {
          if (ActionText == null) {
            ItemActionUpgrades.Add(new Upgrade { Text = new Description("20071", DescriptionFontStyle.Light), Value = element.Element("Charges")?.Value ?? "" });
          }

          ItemActionUpgrades.AddRange(Assets.Buffs[buff].AllUpgrades.ToList());

          if (element.Element("ActionDuration")?.Value != null) {
            ItemActionUpgrades.Add(new Upgrade(element.Element("ActionDuration")));
          }
          if (element.Element("ActionCooldown")?.Value != null) {
            ItemActionUpgrades.Add(new Upgrade(element.Element("ActionCooldown")));
          }
          if (element.Element("IsDestroyedAfterCooldown")?.Value != null) {
            ItemActionUpgrades.Add(new Upgrade(element.Element("IsDestroyedAfterCooldown")));
          }
          return;
        }

        if (ActionText != null) {
          if (element.Element("ActionDuration")?.Value != null) {
            ItemActionUpgrades.Add(new Upgrade(element.Element("ActionDuration")));
          }
          if (element.Element("ActionCooldown")?.Value != null) {
            ItemActionUpgrades.Add(new Upgrade(element.Element("ActionCooldown")));
          }
          if (element.Element("IsDestroyedAfterCooldown")?.Value != null) {
            ItemActionUpgrades.Add(new Upgrade(element.Element("IsDestroyedAfterCooldown")));
          }
        }
      }
    }

    private void ProcessElement_MonumentEventCategory(XElement element) {
      if (element.HasElements) {
        MonumentEvents = element.XPathSelectElements("Events/Item/Event").Select(s => s.Value).ToList();
      }
    }

    private void ProcessElement_MonumentEvent(XElement element) {
      if (element.HasElements) {
        MonumentThresholds = element.XPathSelectElements("RewardThresholds/Item/Reward").Select(s => s.Value).ToList();
      }
    }

    private void ProcessElement_MonumentEventReward(XElement element) {
      if (element.HasElements) {
        MonumentRewards = element.XPathSelectElements("RewardAssets/Item/Reward").Select(s => s.Value).ToList();
      }
    }

    private void ProcessElement_CraftableItem(XElement element) {
      if (element.HasElements) {
        CraftableItemUpgrades = new List<Upgrade>();
        foreach (var item in element.Element("CraftingCosts").Elements()?.Where(e=> e.Element("Product") != null)) {
          CraftableItemUpgrades.Add(new Upgrade { Text = new Description(item.Element("Product").Value), Value = item.Element("Amount").Value });
        }
      }
    }

    private void ProcessElement_Electric(XElement element) {
      if (element.Element("MandatoryElectricity")?.Value == "1") {
        GenericUpgrades.Add(new Upgrade { Text = new Description("12508") });
      }
      else if (element.Element("ProductivityBoost")?.Value != "0" && element.Element("BoostedByElectricity")?.Value != "0") {
        GenericUpgrades.Add(new Upgrade { Text = new Description("10604") });
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
        DivingBellUpgrades = new List<Upgrade>();
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
                results[key].Additionals.Add(new Upgrade { Text = new Description(Assets.KeyToIdDict[AllocationWeight.Name.LocalName]), Value = $"+{AllocationWeight.Element("AdditionalWeight").Value}" });
              }
              else {
                results.Add(key,
                  new Upgrade {
                    Text = new Description(key),
                    Additionals = new List<Upgrade>{
                      new Upgrade {
                        Text = new Description(Assets.KeyToIdDict[AllocationWeight.Name.LocalName]),
                        Value = $"+{AllocationWeight.Element("AdditionalWeight").Value}"
                      }
                    }
                  });
              }
            }
            foreach (var result in results.Values) {
              DivingBellUpgrades.Add(result);
            }
          }
          else {
            DivingBellUpgrades.Add(new Upgrade(item));
          }
        }
      }
    }

    private void ProcessElement_ItemStartExpedition(XElement element) {
      if (element.HasElements) {
        ItemStartExpedition = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          ItemStartExpedition.Add(new Upgrade {
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
          ItemWithUI = new List<Upgrade>();
          foreach (var action in actions) {
            switch (action.Element("Template").Value) {
              case "ActionStartTreasureMapQuest":
                ItemWithUI.Add(new Upgrade(action));
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
            }
          }
        }
      }
    }

    private ConcurrentBag<SourceWithDetailsList> FindSources(string id, XElement asset, Details mainDetails = default) {
      mainDetails = (mainDetails == default) ? new Details() : mainDetails;
      mainDetails.PreviousIDs.Add(id);
      //var mainResult = new SourceWithDetailsList();
      var resultstoadd = new ConcurrentBag<SourceWithDetailsList>();

      //if (Assets.ResearchableItems.ContainsKey(id))
      //{
      //  var result = new SourceWithDetailsList();
      //  var details = Assets.ResearchableItems[id];
      //  result.AddSourceAsset(details.Source, details.Details);
      //  resultstoadd.Add(result);
      //}
      //else
      //{
      //  var item = asset.XPathSelectElement("Values/Item");
      //  var template = asset.Element("Template").Value;
      //  if (item != null && (item.Element("Rarity") == null || "Common".Equals(item.Element("Rarity").Value)) &&
      //    Assets.templatesResearchableItems.Contains(template))
      //  {
      //    var result = new SourceWithDetailsList();
      //    result.AddSourceAsset(Assets.GUIDs["118940"]);
      //    resultstoadd.Add(result);
      //  }
      //}

      if (!Assets.References.ContainsKey(id)) {
        return resultstoadd;
      }
      var cachedLinks = Assets.References[id];

      foreach (var referencingAsset in cachedLinks) {
        foreach (var reference in referencingAsset.Descendants()) {

          if ("GUID".Equals(reference.Name.LocalName) || !id.Equals(reference.Value) || reference.HasElements)
            continue;

          //Weight 0
          if (reference.Parent.Element("Weight")?.Value == "0") {
            continue;
          }

          //Ignores
          if (reference.Name.LocalName is string foundedName &&
            foundedName.MatchOne("BaseAssetGUID", "Icon", "ItemUsed", "TradePrice", "GenPool", "NotificationIcon", "ReplacingWorkforce", "ProductFilter", "BusNeed", "LineID", "PosX", "Context", "ProductionOutputInfotip", "UnlockNeeded")) {
            continue;
          }
          if (reference.Parent?.Parent?.Name.LocalName is string gparent &&
            gparent.MatchOne("Costs", "UpgradeCost", "CraftingCosts", "Maintenances", "StoredProducts", "UnlockAssets")) {
            continue;
          }
          if (reference.Parent?.Parent?.Parent?.Name.LocalName is string ggParent &&
            ggParent.MatchOne("FactoryBase", "Sellable", "PublicService")) {
            continue;
          }

          var Details = new Details(mainDetails);
          var result = new SourceWithDetailsList();
          var key = referencingAsset.XPathSelectElement("Values/Standard/GUID").Value;

          if (Details.PreviousIDs.Contains(key)) {
            continue;
          }

          switch (referencingAsset.Element("Template").Value) {
            //case "AssetPool":
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
            case "IrrigationUpgrade":
            case "TradeContractFeature": //export import update 10
            case "PaMSy_Base":
            case "MaintenanceBarConfig":
            case "FeatureUnlock":
            case "GoodValueBalancing":  //Update 11
            case "Busstop":  //Update 11
            case "ResidenceBuilding":  //Update 12
            case "TowerRestaurant":  //Update 12
            case "ObjectmenuResidenceScene":  //Update 12
            case "Mall":  //Update 12
            case "ScenarioInformationInternal":  //Update 13
            case "MonumentScene":  //Update 13
            case "ScenarioRuin":  //Update 13
              // ignore
              break;

            case "TriggerCampaign":
            case "Trigger":
            case "PassiveTradeFeature":
              //Todo?
              break;

            case "Expedition":
              if (!referencingAsset.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (!reference.Name.LocalName.MatchOne("FillEventPool", "Reward", "EventOrEventPool")) {
                  break;
                }

                result.AddSourceAsset(referencingAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
              }
              break;

            case "Profile_3rdParty_NoTrader_NoProperty3rdParty":
            case "Profile_3rdParty_ItemCrafter":
            case "Profile_3rdParty":
            case "Profile_3rdParty_Pirate":
            case "Profile_2ndParty":
            case "Profile_3rdParty_ItemCrafter-NoTrader":
              if (key.MatchOne("199", "200", "240", "117422")) {
                break;
              }
              if (!referencingAsset.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (reference.Name.LocalName.MatchOne("ShipDropRewardPool")) {
                  result.AddSourceAsset(referencingAsset.GetProxyElement("ShipDrop"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
                  break;
                }

                if (reference.Name.LocalName == "OfferingItems" ||
                  reference.Parent.Parent.Name.LocalName.MatchOne("ShipsForSale", "GoodSets")) {
                  var oldParent = reference.Parent;
                  var parent = reference.Parent;
                  while (parent.Name.LocalName != "Progression") {
                    oldParent = parent;
                    parent = parent.Parent;
                  }
                  var isRollable = reference.Name.LocalName == "OfferingItems";
                  result.AddSourceAsset(referencingAsset.GetProxyElement("Harbor"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset.GetProxyElement(oldParent.Name.LocalName)) }, isRollable);
                  break;
                }

                if (reference.Name.LocalName == "Pool" && reference.Parent.Parent.Name.LocalName == "ItemPools") {
                  result.AddSourceAsset(referencingAsset.GetProxyElement("Harbor"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset.GetProxyElement(reference.Parent.Name.LocalName)) }, true);
                  break;
                }

                if (reference.Name.LocalName.MatchOne("MainIslandRewardPool", "SecondaryIslandRewardPool")) {
                  result.AddSourceAsset(referencingAsset.GetProxyElement("TakeOver"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset.GetProxyElement($"{reference.Name.LocalName}#{reference.Parent.Name}")) });
                  break;
                }

                if (reference.Name.LocalName == "Item" && reference.Parent.Parent.Name.LocalName == "CraftableItems") {
                  result.AddSourceAsset(referencingAsset.GetProxyElement("Crafting"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset.GetProxyElement(reference.Parent.Parent.Parent.Name.LocalName)) });
                  break;
                }
                if (reference.Name.LocalName.MatchOne("ItemPool", "Product", "Good")) {
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
            case "A7_QuestNewspaperArticle":
            case "A7_QuestLostCargo":
            case "A7_QuestExpedition":
            case "A7_QuestDecision":
            case "A7_QuestSmugglerWOScanners":
              if (!referencingAsset.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (reference.Name.LocalName.MatchOne("Reward")) {
                  result.AddSourceAsset(referencingAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
                }
              }
              break;

            case "A7_QuestDivingBellTreasureMap":
              if (!referencingAsset.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (reference.Name.LocalName.MatchOne("Reward", "TreasureItem", "ScrapDummyItem")) {
                  GetSources(Details, key, asset).SaveSource(key).MergeResults(key, in result);
                  if (result.Count == 0) {
                    result.AddSourceAsset(referencingAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
                  }
                }
              }
              break;

            case "DivingBellShip":
              //Ignore second divingbellship. its for a quest
              if (key == "113710") {
                break;
              }
              if (reference.Name.LocalName == "ItemReplacementPools") {
                result.AddSourceAsset(referencingAsset.GetProxyElement("Dive"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
              }
              break;

            case "AirShip":
              if (reference.Name.LocalName == "ItemReplacementPools") {
                result.AddSourceAsset(referencingAsset.GetProxyElement("Pickup"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
              }
              break;

            case "ItemWithUI":
              if (reference.Name.LocalName == "NewItem") {
                result.AddSourceAsset(referencingAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
              }
              else if (reference.Name.LocalName == "Ressource" && reference.Parent.Name.LocalName == "ActionAddResource") {
                result.AddSourceAsset(referencingAsset.GetProxyElement("Item"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
              }
              else if (reference.Name.LocalName == "TreasureMapQuest") {
                result.AddSourceAsset(referencingAsset.GetProxyElement("Dive"), new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
              }
              break;

            case "TourismFeature":
              if (reference.Name.LocalName == "Pool") {
                var pool = reference.Parent;
                result.AddSourceAsset(referencingAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(pool.GetProxyElement(reference.Parent.Parent.Parent.Name.LocalName)) });
              }
              break;

            case "CultureBuff":
            case "HarborOfficeItem":
              if (reference.Name.LocalName == "OverrideSpecialistPool") {
                result.AddSourceAsset(Assets.TourismFeatureAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
              }
              else {
                throw new NotImplementedException();
              }
              break;

            case "ResearchSubcategory":
              if (reference.Name.LocalName == "Pool") {
                result.AddSourceAsset(referencingAsset, new HashSet<AssetWithWeight> { new AssetWithWeight(referencingAsset) });
              }
              break;

            case "ResearchFeature":
              if (reference.Name.LocalName == "CommonRecipesBlacklist") {
                IsResearchable = false;
              }
              break;

            case "ExpeditionDecision":
            case "ExpeditionTrade":
              if (reference.Name.LocalName.MatchOne("Reward", "Product", "Item")) {
                var tempresults = GetSources(Details, key, asset);

                //Inject Expedition Events
                foreach (var sourceWithDetails in tempresults) {
                  foreach (var expedition in sourceWithDetails.Where(r => r.Source.Element("Template").Value == "Expedition")) {
                    if (sourceWithDetails.FollowingEvents.Count > 0) {
                      expedition.Details.Clear();
                      foreach (var item in sourceWithDetails.FollowingEvents) {
                        var path = referencingAsset.XPathSelectElement("Values/Standard/Name").Value.Split(' ').Last();
                        expedition.Details.Add(new AssetWithWeight(item.GetProxyElement(path)));
                      }
                    }
                  }
                }
                tempresults.SaveSource(key).MergeResults(key, in result);

                break;
              }
              else if (!reference.Name.LocalName.MatchOne("Option", "FollowupSuccessOption", "FollowupFailOrCancelOption", "InsertEvent")) {
                break;
              }
              goto case "SearchAgain";

            case "ExpeditionOption":
            case "ExpeditionMapOption":
              if (reference.Name.LocalName != "Decision") {
                break;
              }
              goto case "SearchAgain";

            case "ExpeditionEvent":
              if (reference.Name.LocalName != "StartDecision") {
                break;
              }

              result.FollowingEvents.Add(referencingAsset);

              goto case "SearchAgain";

            case "ExpeditionBribe":
              if (reference.Name.LocalName == "Item") {
                break;
              }
              if (!reference.Name.LocalName.MatchOne("FollowupSuccessOption", "FollowupFailOrCancelOption")) {
                break;
              }
              goto case "SearchAgain";
            
            case "QuestObject":
            case "Collectable":
              if (reference.Parent?.Parent?.Parent?.Name.LocalName != "ActionAddGoodsToItemContainer") {
                break;
              }
              goto case "SearchAgain";

            case "ItemReplacementPool":
              if (reference.Name.LocalName != "ReplacementPool") {
                break;
              }
              else {
                if (reference.Parent.Element("DummyItem").Value is string dummy) {
                  //new ConcurrentBag<SourceWithDetailsList>(GetSources(Details, dummy).SaveSource(dummy).Concat(GetSources(Details, key))).MergeResults(dummy, result);
                  switch (key) {
                    case "193854": // DivingShipReplacementPool
                      GetSources(Details, dummy, asset).SaveSource(dummy).MergeResults(dummy, result);
                      if (result.Count == 0) { // No Treasure map found?  Nevertheless Dive loot ???
                        GetSources(Details, key, asset).MergeResults(key, result);
                      }
                      break;

                    case "193855": // AirShipReplacementPool
                      GetSources(Details, key, asset).SaveSource(key).MergeResults(key, result);
                      break;

                    default:
                      throw new NotImplementedException();
                  }
                }
                break;
              }

            case "RewardPool":
            case "RewardItemPool":
              var itemListRewards = reference.Parent.Parent.Elements().ToList();
              var weightSumRewards = itemListRewards.Sum(item => (item.Element("Weight")?.Value is string str) ? double.Parse(str) : 1.0F);

              foreach (var item in itemListRewards.Where(i => i.Element("ItemLink")?.Value != null).ToLookup(i => i.Element("ItemLink").Value)) {
                result.ElementWeights.Add(item.Key, item.Sum(i => (i.Element("Weight")?.Value is string str ? double.Parse(str) : 1.0F) / weightSumRewards));
              }

              goto case "SearchAgain";

            case "AssetPool":
              var itemListAssets = reference.Parent.Parent.Elements().ToList();
              var weightSumAssets = itemListAssets.Sum(item => (item.Element("Probability")?.Value is string str) ? double.Parse(str) : 1.0F);

              foreach (var item in itemListAssets.Where(i => i.Element("Asset")?.Value != null).ToLookup(i => i.Element("Asset").Value)) {
                result.ElementWeights.Add(item.Key, item.Sum(i => (i.Element("Probability")?.Value is string str ? double.Parse(str) : 1.0F) / weightSumAssets));
              }

              goto case "SearchAgain";

            case "ExpeditionEventPool":
            case "ResourcePool":
            case "A7_QuestSubQuest":
            case "ProductList":
            case "SearchAgain":
              GetSources(Details, key, asset).SaveSource(key).MergeResults(key, in result);
              break;

            default:
              Debug.WriteLine(referencingAsset.Element("Template").Value);
              //throw new NotImplementedException(referencingAsset.Element("Template").Value);
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

    private ConcurrentBag<SourceWithDetailsList> GetSources(Details Details, string key, XElement asset) {
      ConcurrentBag<SourceWithDetailsList> tempresult = new ConcurrentBag<SourceWithDetailsList>(SavedSources.TryGetValue(key, out var saved) ? saved.Select(i => i.Copy()) : FindSources(key, asset, Details).Select(i => i.Copy()));

      return tempresult;
    }

    #endregion Private Methods
  }
}