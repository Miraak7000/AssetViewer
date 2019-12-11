using RDA.Data;
using RDA.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Templates {

  public class Asset {

    #region Properties

    public static ConcurrentDictionary<string, SourceWithDetailsList> SavedSources { get; set; } = new ConcurrentDictionary<string, SourceWithDetailsList>();
    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public string RarityType { get; private set; } = "Common";
    public Description Rarity { get; set; }
    public string ItemType { get; set; }
    public String ReleaseVersion { get; set; } = "Release";

    //
    public Allocation Allocation { get; set; }

    //
    public List<EffectTarget> EffectTargets { get; set; }

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
    public List<Upgrade> ShipyardUpgrades { get; set; }
    public List<Upgrade> VisitorHarborUpgrades { get; set; }
    public List<Upgrade> RepairCraneUpgrades { get; set; }
    public List<Upgrade> KontorUpgrades { get; set; }
    public List<Upgrade> IncidentInfectableUpgrades { get; set; }

    public List<Upgrade> IncidentArcticIllnessIncreaseUpgrades { get; set; }
    public List<Upgrade> IncidentInfluencerUpgrades { get; set; }
    public List<Upgrade> ItemGeneratorUpgrades { get; set; }
    public List<Upgrade> ItemActionUpgrades { get; set; }

    public IEnumerable<Upgrade> AllUpgrades => typeof(Asset)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(List<Upgrade>) && p.Name != nameof(Sources))
            .OrderBy(p => p.Name)
            .SelectMany(l => (List<Upgrade>)l.GetValue(this) ?? Enumerable.Empty<Upgrade>());

    //
    public List<Upgrade> ItemSets { get; set; }

    //
    public String TradePrice { get; set; }

    //
    public Description Info { get; set; }

    //
    public List<TempSource> Sources { get; set; }

    //
    public List<String> MonumentEvents { get; set; }

    public List<String> MonumentThresholds { get; set; }
    public List<String> MonumentRewards { get; set; }
    public List<Upgrade> PassiveTradeGoodGenUpgrades { get; private set; }
    public List<Upgrade> DivingBellUpgrades { get; private set; }
    public List<Upgrade> CraftableItemUpgrades { get; private set; }
    public List<Upgrade> PierUpgrade { get; private set; }
    public List<Upgrade> ItemWithUI { get; private set; }
    public List<Upgrade> ItemStartExpedition { get; private set; }
    public List<Upgrade> ItemSocketSet { get; private set; }

    #endregion Properties

    #region Fields

    public String Path = String.Empty;

    #endregion Fields

    #region Constructors

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
            this.ItemType = "Item";
            break;

          case "BuildPermitBuilding":
            this.ItemType = "Building";
            break;
          case "BuildPermitModules":
          case "CultureModule":
          case "OrnamentalModule":
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
          /// Items Without ItemTyp ///
          /////////////////////////////
          //case "ActiveItem":
          //  this.ItemType = "ActiveItem";
          //  break;
          //case "ItemSpecialActionVisualEffect":
          //  this.ItemType = "ItemSpecialActionVisualEffect";
          //  break;
          //case "GuildhouseItem":
          //  this.ItemType = "GuildhouseItem";
          //  break;
          //case "ItemSpecialAction":
          //  this.ItemType = "ItemSpecialAction";
          //  break;
          //case "HarborOfficeItem":
          //  this.ItemType = "HarborOfficeItem";
          //  break;
          //case "VehicleItem":
          //  this.ItemType = "VehicleItem";
          //  break;
          //case "TownhallItem":
          //  this.ItemType = "TownhallItem";
          //  break;
          //case "BuildPermitBuilding":
          //  this.ItemType = "BuildPermitBuilding";
          //  break;
          //if (this.ItemType == "None")
          //  this.ItemType = "Common";
          //if (this.ItemType == "Normal")
          //  this.ItemType = "Common";
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
          case "Cost":
          case "Blocking":
          case "Selection":
          case "Object":
          case "Constructable":
          case "Mesh":
          case "SoundEmitter":
          case "FeedbackController":
          case "AmbientMoodProvider":
          case "Pausable":
          case "BuildPermit":
          case "VisualEffectWhenActive":         
          case "QuestObject":         
          // ignore this nodes

          case "UpgradeList":
          case "VisitorUpgrade":
          //Maybe next patch. all empty

          case "Infolayer":
          case "ItemContainer":
          case "BuildingModule":
          case "Attackable":
          //Maybe usefull Building Informations
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

          case "FactoryUpgrade":
            this.ProcessElement_FactoryUpgrade(element);
            break;

          case "BuildingUpgrade":
            this.ProcessElement_BuildingUpgrade(element);
            break;

          case "CultureUpgrade":
            this.ProcessElement_CultureUpgrade(element);
            break;

          case "ModuleOwnerUpgrade":
            this.ProcessElement_ModuleOwnerUpgrade(element);
            break;

          case "ResidenceUpgrade":
            this.ProcessElement_ResidenceUpgrade(element);
            break;

          case "PopulationUpgrade":
            this.ProcessElement_PopulationUpgrade(element);
            break;

          case "ElectricUpgrade":
            this.ProcessElement_ElectricUpgrade(element);
            break;

          case "ExpeditionAttribute":
            this.ProcessElement_ExpeditionAttribute(element);
            break;

          case "VisitorHarborUpgrade":
            this.ProcessElement_VisitorHarborUpgrade(element);
            break;

          case "AttackerUpgrade":
            this.ProcessElement_AttackerUpgrade(element);
            break;

          case "ShipyardUpgrade":
            this.ProcessElement_ShipyardUpgrade(element);
            break;

          case "AttackableUpgrade":
            this.ProcessElement_AttackableUpgrade(element);
            break;

          case "ProjectileUpgrade":
            this.ProcessElement_ProjectileUpgrade(element);
            break;

          case "VehicleUpgrade":
            this.ProcessElement_VehicleUpgrade(element);
            break;

          case "RepairCraneUpgrade":
            this.ProcessElement_RepairCraneUpgrade(element);
            break;

          case "KontorUpgrade":
            this.ProcessElement_KontorUpgrade(element);
            break;

          case "TradeShipUpgrade":
            this.ProcessElement_TradeShipUpgrade(element);
            break;

          case "ItemAction":
            this.ProcessElement_ItemActions(element);
            break;

          case "PassiveTradeGoodGenUpgrade":
            this.ProcessElement_PassiveTradeGoodGenUpgrades(element);
            break;

          case "IncidentInfectableUpgrade":
            this.ProcessElement_IncidentInfectableUpgrades(element);
            break;

          case "IncidentArcticIllnessIncreaseUpgrade":
            this.ProcessElement_IncidentArcticIllnessIncreaseUpgrades(element);
            break;

         case "IncidentInfluencerUpgrade":
            this.ProcessElement_IncidentInfluencerUpgrades(element);
            break;

          case "ItemGeneratorUpgrade":
            this.ProcessElement_ItemGeneratorUpgrades(element);
            break;

          case "SpecialAction":
            this.ProcessElement_SpecialActions(element);
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

          case "Product":
          //Todo: maybe add some properties
          case "NewspaperUpgrade":
          case "DistributionUpgrade":
            // Todo: needs to implemented
            break;

          case "DivingBellUpgrade":
            this.ProcessElement_DivingBellUpgrade(element);
            break;

          case "CraftableItem":
            this.ProcessElement_CraftableItem(element);
            break;

          case "PierUpgrade":
            this.ProcessElement_PierUpgrade(element);
            break;

          case "ItemWithUI":
            this.ProcessElement_ItemWithUI(element);
            break;

          case "ItemStartExpedition":
            this.ProcessElement_ItemStartExpedition(element);
            break;

          case "Building":
            this.ProcessElement_Building(element);
            break;

          case "ItemSocketSet":
            this.ProcessElement_ItemSocketSet(element);
            break;

          default:
            Debug.WriteLine(element.Name.LocalName);
            if (element.Name.LocalName == "Infolayer" && (element.HasElements || element.Value != null)) {


            }
            else {

            }
            //throw new NotImplementedException(element.Name.LocalName);
            break;
        }
      }
      if (findSources) {
        var sources = this.FindSources(this.ID).ToArray();
        this.Sources = sources.Select(s => new TempSource(s)).ToList();
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(new XAttribute("ID", this.ID));
      result.Add(new XAttribute("Release", this.ReleaseVersion));
      result.Add(new XElement("Name", this.Name));
      result.Add(this.Text.ToXml("Text"));
      result.Add(this.Rarity == null ? new Description(Assets.KeyToIdDict["Common"]).ToXml("Rarity") : this.Rarity.ToXml("Rarity"));
      result.Add(new XAttribute("RarityType", RarityType));
      result.Add(new XElement("ItemType", this.ItemType));
      //
      result.Add(this.Allocation == null ? new XElement("Allocation") : this.Allocation.ToXml());
      //
      var type = typeof(Asset);
      foreach (var item in type.GetProperties().Where(p => p.PropertyType == typeof(List<Upgrade>))) {
        if (item.GetValue(this) != null) {
          result.Add(new XElement(item.Name, (item.GetValue(this) as List<Upgrade>)?.Select(s => s.ToXml())));
        }
      }
      //
      result.Add(new XElement("TradePrice", this.TradePrice));
      //
      if (this.Info != null)
        result.Add(this.Info.ToXml("Info"));
      //
      if (this.EffectTargets != null)
        result.Add(new XElement("EffectTargets", this.EffectTargets.Select(s => s.ToXml())));
      if (this.Sources != null)
        result.Add(new XElement("Sources", this.Sources?.Select(s => s.ToXml())));
      //
      if (this.MonumentEvents != null)
        result.Add(new XElement("MonumentEvents", this.MonumentEvents?.Select(s => new XElement("Event", s))));
      if (this.MonumentThresholds != null)
        result.Add(new XElement("MonumentThresholds", this.MonumentThresholds?.Select(s => new XElement("Threshold", s))));
      if (this.MonumentRewards != null)
        result.Add(new XElement("MonumentRewards", this.MonumentRewards?.Select(s => new XElement("Reward", s))));
      return result;
    }

    public override String ToString() {
      return $"{this.ID} - {this.Name}";
    }

    private void ProcessElement_ItemSocketSet(XElement element) {
      this.ItemSocketSet = new List<Upgrade>();
      if (element.Element("SetBuff")?.Value is string buff) {
        this.ItemSocketSet.AddRange(Assets.Buffs[buff].AllUpgrades.ToList());
        this.EffectTargets = Assets.Buffs[buff].EffectTargets;
      }
      else if (element.Element("RegionSetBuff") is XElement regionBuffs){
        this.EffectTargets = new List<EffectTarget>();
        foreach (var region in regionBuffs.Elements()) {
          switch (region.Name.LocalName) {
            case "Moderate":
              this.ItemSocketSet.Add(new Upgrade { Text = new Description("113322"), Additionals = Assets.Buffs[region.Element("SetBuff").Value].AllUpgrades.ToList() });
              break;
            case "Colony01":
              this.ItemSocketSet.Add(new Upgrade { Text = new Description("113395"), Additionals = Assets.Buffs[region.Element("SetBuff").Value].AllUpgrades.ToList() });
              break;
            case "Arctic Region":
              this.ItemSocketSet.Add(new Upgrade { Text = new Description("115487"), Additionals = Assets.Buffs[region.Element("SetBuff").Value].AllUpgrades.ToList() });
              break;
            default:
              throw new NotImplementedException();
              break;
          }
          this.EffectTargets.AddRange(Assets.Buffs[region.Element("SetBuff").Value].EffectTargets);
        }
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
      if (element.Element("ItemSet") != null) {
        this.ItemSets = new List<Upgrade>();
        this.ItemSets.Add(new Upgrade(element.Element("ItemSet")));
      }
    }

    private void ProcessElement_ItemEffect(XElement element) {
      if (element.HasElements && element.Element("EffectTargets") == null)
        throw new NotImplementedException();
      if (element.HasElements && element.Element("EffectTargets").HasElements) {
        this.EffectTargets = new List<EffectTarget>();
        foreach (var item in element.Element("EffectTargets").Elements()) {
          EffectTargets.Add(new EffectTarget(item));
        }
      }
    }

    private void ProcessElement_FactoryUpgrade(XElement element) {
      if (element.HasElements) {
        this.FactoryUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.FactoryUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_BuildingUpgrade(XElement element) {
      if (element.HasElements) {
        this.BuildingUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          // TODO: this needs to be implemented
          if (item.Name.LocalName == "PublicServiceNoSatisfactionDistance")
            continue;
          this.BuildingUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_CultureUpgrade(XElement element) {
      if (element.HasElements) {
        this.CultureUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          if (item.Name.LocalName == "ChangeModule")
            continue;
          if (item.Name.LocalName == "ForcedFeedbackVariation")
            continue;
          if (item.Name.LocalName == "AdditionalModuleSoundLoop")
            continue;
          if (item.Name.LocalName == "HideBuff")
            continue;
          this.CultureUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_ModuleOwnerUpgrade(XElement element) {
      if (element.HasElements) {
        this.ModuleOwnerUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.ModuleOwnerUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_ResidenceUpgrade(XElement element) {
      if (element.HasElements) {
        this.ResidenceUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.ResidenceUpgrades.Add(new Upgrade(item));
        }
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

    private void ProcessElement_ElectricUpgrade(XElement element) {
      if (element.HasElements) {
        this.ElectricUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.ElectricUpgrades.Add(new Upgrade(item));
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

    private void ProcessElement_VisitorHarborUpgrade(XElement element) {
      if (element.HasElements) {
        this.VisitorHarborUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.VisitorHarborUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_PassiveTradeGoodGenUpgrades(XElement element) {
      if (element.HasElements) {
        this.PassiveTradeGoodGenUpgrades = new List<Upgrade>();
        PassiveTradeGoodGenUpgrades.Add(new Upgrade(element));
      }
    }

    private void ProcessElement_IncidentInfectableUpgrades(XElement element) {
      if (element.HasElements) {
        this.IncidentInfectableUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          // TODO: this needs to be implemented
          if (item.Name.LocalName == "OverrideIncidentAttractiveness")
            continue;
          this.IncidentInfectableUpgrades.Add(new Upgrade(item));
        }
      }
    }

        private void ProcessElement_IncidentArcticIllnessIncreaseUpgrades(XElement element)
        {
            if (element.HasElements)
            {
                this.IncidentArcticIllnessIncreaseUpgrades = new List<Upgrade>();
                foreach (var item in element.Elements())
                {
                    this.IncidentArcticIllnessIncreaseUpgrades.Add(new Upgrade(item));
                }
            }
        }

        private void ProcessElement_IncidentInfluencerUpgrades(XElement element) {
      if (element.HasElements) {
        foreach (var item in element.Elements()) {
          if (item.Name.LocalName == "RiotInfluenceUpgrade")
            continue;
          if (item.Name.LocalName == "FireInfluenceUpgrade")
            continue;
          if (item.Name.LocalName == "IllnessInfluenceUpgrade")
            continue;
          if (item.Name.LocalName == "IncidentArcticIllnessIncreaseUpgrade")
            continue;
          if (item.Name.LocalName == "DistanceUpgrade")
            continue;
          this.IncidentInfluencerUpgrades = new List<Upgrade>();
          IncidentInfluencerUpgrades.Add(new Upgrade(item));
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
          if (Projectile.XPathSelectElement("Values/Exploder/InnerDamage")?.Value is string damage) {
            if (damage != "0") {
              this.AttackerUpgrades.Add(new Upgrade() { Text = new Description("20621"), Value = damage });
            }
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

    private void ProcessElement_ShipyardUpgrade(XElement element) {
      if (element.HasElements) {
        ShipyardUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          if (item.Name.LocalName == "ReplaceAssemblyOptions") {
            continue;
          }
          ShipyardUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_AttackableUpgrade(XElement element) {
      if (element.HasElements) {
        this.AttackableUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          switch (item.Name.LocalName) {
            case "DamageReceiveFactor":
              foreach (var subItem in item.Elements()) {
                this.AttackableUpgrades.Add(new Upgrade(subItem));
              }
              break;

            default:
              this.AttackableUpgrades.Add(new Upgrade(item));
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

    private void ProcessElement_VehicleUpgrade(XElement element) {
      if (element.HasElements) {
        this.VehicleUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.VehicleUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_RepairCraneUpgrade(XElement element) {
      if (element.HasElements) {
        this.RepairCraneUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          // TODO: this needs to be implemented
          if (item.Name.LocalName == "HealBuildingsPerMinuteUpgrade")
            continue;
          this.RepairCraneUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_KontorUpgrade(XElement element) {
      if (element.HasElements) {
        KontorUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.KontorUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_TradeShipUpgrade(XElement element) {
      if (element.HasElements) {
        this.TradeShipUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.TradeShipUpgrades.Add(new Upgrade(item));
        }
      }
    }

    private void ProcessElement_ItemActions(XElement element) {
      if (element.HasElements) {
        var itemAction = element.Element("ItemAction")?.Value;
        if (itemAction == null) {
          switch (element.Parent.Parent.Element("Template").Value) {
            case "ItemSpecialAction":
            case "ItemSpecialActionVisualEffect":
              element.Add(new XElement("ItemAction", "NUKE"));
              break;

            default:
              element.Add(new XElement("ItemAction", "NOACTION"));
              break;
          }
        }
        var action = element.Element("ItemAction").Value;
        // Ignore
        Description ActionText = null;
        switch (action) {
          case "REPAIR":
          case "DRAG":
          case "CHANNEL":
            return;

          case "HEAL_INCIDENT":
          case "PLACE_MINE":
            break;

          default:
            ActionText = new Description("20072", DescriptionFontStyle.Light) { Icon = null };

            break;
        }

        this.ItemActionUpgrades = new List<Upgrade>();

        if (element.Element("ActionDescription")?.Value is string desc) {
          ActionText = new Description(desc, DescriptionFontStyle.Light);
        }
        if (ActionText != null) {
          this.ItemActionUpgrades.Add(new Upgrade() { Text = ActionText, Value = element.Element("Charges")?.Value ?? "" });
        }
        if (action == "KAMIKAZE") {
          this.ItemActionUpgrades.Add(new Upgrade() { Text = new Description("21347") { AdditionalInformation = new Description("21348", DescriptionFontStyle.Light) } });
          this.ItemActionUpgrades.Add(new Upgrade() { Text = new Description("21353") });
          return;
        }

        if (element.Element("ActiveBuff")?.Value is string buff) {
          this.ItemActionUpgrades.AddRange(Assets.Buffs[buff].AllUpgrades.ToList());
        }
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

    private void ProcessElement_ItemGeneratorUpgrades(XElement element) {
      if (element.HasElements) {
        // TODO: this needs to be implemented
      }
    }

    private void ProcessElement_SpecialActions(XElement element) {
      if (element.HasElements) {
        // TODO: this needs to be implemented
      }
    }

    private void ProcessElement_MonumentEventCategory(XElement element) {
      this.MonumentEvents = element.XPathSelectElements("Events/Item/Event").Select(s => s.Value).ToList();
    }

    private void ProcessElement_MonumentEvent(XElement element) {
      this.MonumentThresholds = element.XPathSelectElements("RewardThresholds/Item/Reward").Select(s => s.Value).ToList();
    }

    private void ProcessElement_MonumentEventReward(XElement element) {
      this.MonumentRewards = element.XPathSelectElements("RewardAssets/Item/Reward").Select(s => s.Value).ToList();
    }

    private void ProcessElement_CraftableItem(XElement element) {
      if (element.HasElements) {
        this.CraftableItemUpgrades = new List<Upgrade>();
        foreach (var item in element.Element("CraftingCosts").Elements()) {
          this.CraftableItemUpgrades.Add(new Upgrade() { Text = new Description(item.Element("Product").Value), Value = item.Element("Amount").Value });
        }
      }
    }

    private void ProcessElement_DivingBellUpgrade(XElement element) {
      if (element.HasElements) {
        this.DivingBellUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          if (item.Name.LocalName == "AllocationWeightUpgrade") {
            var results = new Dictionary<string, Upgrade>();
            //var results = new Collection<Upgrade>();
            foreach (var AllocationWeight in item.Elements()) {
              string key = null;
              switch (AllocationWeight.Name.LocalName) {
                case "None":
                  key = "22230";

                  break;

                case "Zoo":
                  key = "22231";
                  //results.Add(new Upgrade() { Text = new Description("22231"), Value = $"+{AllocationWeight.Element("AdditionalWeight").Value}" });
                  break;

                case "Museum":
                  key = "22232";
                  //results.Add(new Upgrade() { Text = new Description("22232"), Value = $"+{AllocationWeight.Element("AdditionalWeight").Value}" });
                  break;

                default:
                  key = "22233";
                  ////results.Add(new Upgrade() { Text = new Description("22233"), Value = $"+{AllocationWeight.Element("AdditionalWeight").Value}" });
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

    private void ProcessElement_PierUpgrade(XElement element) {
      if (element.HasElements) {
        this.PierUpgrade = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.PierUpgrade.Add(new Upgrade(item));
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
            if (element.HasElements)
            {
                var actions = element.XPathSelectElement("ItemActions/Values/ActionList/Actions").Elements("Item").Select(a => a.Element("Action"));
                if (actions != null)
                {
                    this.ItemWithUI = new List<Upgrade>();
                    foreach (var action in actions)
                    {
                        switch (action.Element("Template").Value)
                        {
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
                                //              throw new NotImplementedException();
                                break;
                        }
                    }
                }
            }
    }

    private void ProcessElement_Building(XElement element) {
      //Todo
    }

    private SourceWithDetailsList FindSources(String id, Details mainDetails = default, SourceWithDetailsList inResult = default) {
      mainDetails = (mainDetails == default) ? new Details() : mainDetails;
      mainDetails.PreviousIDs.Add(id);
      var mainResult = inResult ?? new SourceWithDetailsList();
      var resultstoadd = new List<SourceWithDetailsList>();
      var links = Assets.Original.XPathSelectElements($"//*[text()={id} and not(self::GUID)]").ToArray();
      if (links.Length > 0) {
        for (var i = 0; i < links.Length; i++) {
          var element = links[i];
          //Weight 0
          if (element.Parent.Element("Weight")?.Value == "0") {
            continue;
          }
          var isShipDrop = element.Name.LocalName == "ShipDropRewardPool";
          while (element.Name.LocalName != "Asset" || !element.HasElements) {
            element = element.Parent;
          }

          var Details = new Details(mainDetails);
          var result = mainResult.Copy();
          var key = element.XPathSelectElement("Values/Standard/GUID").Value;

          if (element.Element("Template") == null || isShipDrop) {
            // Ship Drop
            if (element.XPathSelectElement("Values/Profile/ShipDropRewardPool")?.Value == id) {
              result.AddSourceAsset(element.GetProxyElement("ShipDrop"), new HashSet<XElement> { element.GetProxyElement("ShipDrop") });
              resultstoadd.Add(result);
            }
            // Hafen Hugo Mercier
            if (element.XPathSelectElement("Values/Standard/GUID")?.Value == "220") {
              if (element.XPathSelectElements("Values/ConstructionAI/ItemTradeConfig/ItemPools").Elements().Any(f => f.Element("Pool").Value == id)) {
                var progressions = GetProgession(element, id);
                if (progressions != null) {
                  foreach (var item in progressions) {
                    result.AddSourceAsset(element.GetProxyElement("Harbor"), new HashSet<XElement> { element.GetProxyElement(item) });
                  }

                  break;
                }
                else {
                }
              }
            }
            continue;
          }
          if (mainDetails.PreviousIDs.Contains(key)) {
            continue;
          }

          switch (element.Element("Template").Value) {
            case "AssetPool":
            case "TutorialQuest":
            case "SettlementRightsFeature":
            case "Profile_2ndParty":
            case "GuildhouseItem":
            case "HarborOfficeItem":
            case "HarbourOfficeBuff":
            case "MonumentEvent":
            case "MainQuest":
            case "ObjectmenuMilitary":
            case "WarShip":
            case "ExpeditionFeature":
            case "ConstructionCategory":
            case "FarmBuilding":
            case "FactoryBuilding7":
            case "FestivalBuff":
            case "VehicleItem":
            case "TownhallItem":
            case "ItemSpecialAction":
            case "PopulationLevel7":
            case "NeedsSatisfactionNews":
            case "ProductFilter":
            case "PlayerCounterContextPool":
            case "OrnamentalBuilding":
            case "CultureModule":
            case "ResidenceBuilding7":
            case "FreeAreaBuilding":
            case "HeavyFreeAreaBuilding":
            case "SlotFactoryBuilding7":
            case "HeavyFactoryBuilding":
            case "Farmfield":
            case "OilPumpBuilding":
            case "PublicServiceBuilding":
            case "CityInstitutionBuilding":
            case "Market":
            case "Warehouse":
            case "CultureBuilding":
            case "Monument":
            case "PowerplantBuilding":
            case "Street":
            case "Guildhouse":
            case "HarborOffice":
            case "GuildhouseBuff":
            case "TriggerQuest":
            case "TradeRouteFeature":
            case "HarborWarehouse7":
            case "HarborDepot":
            case "Shipyard":
            case "HarborBuildingAttacker":
            case "RepairCrane":
            case "HarborLandingStage7":
            case "VisitorPier":
            case "HarborWarehouseStrategic":
            case "WorkforceConnector":
            case "Headquarter":
            case "QuestVehicleTrade":
            case "DifficultyBalancing":
            case "RewardConfig":
            case "UplayAction":
            case "NegotiationPopup":
            case "NewspaperArticle":
            case "WorkforceNewsTracker":
            case "InfluenceTitleBuff":
            case "WorkforceMenu":
            case "TownhallBuff":
            case "AudioText":
            case "TradeShip":
            case "ActiveItem":
            case "CultureItem":
            case "QuestVehicle":
            case "Achievement":
            case "EventTradeShip":
            case "Audio":
            case "WorkforceSliderNewsTracker":
            case "ChannelTarget":
            case "ResolveActionCost":
            case "Region":
            case "OrnamentalModule":
            case "ObjectBuildNewsTracker":
            case "FeedbackBuildingGroup":
            case "QuestItem":
            case "Notification":
              // ignore
              break;

            case "TriggerCampaign":
            case "Trigger":
            case "PassiveTradeFeature":
              //Todo?
              break;
            //
            case "Expedition":
              if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (Details.Items.Count == 0) {
                  Details.Add(element);
                }
                result.AddSourceAsset(element, Details.Items);
              }
              break;

            case "Profile_3rdParty":
            case "Profile_3rdParty_Pirate":
              if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                if (element.XPathSelectElement("Standard/Profile/ShipDropRewardPool")?.Value == id) {
                  result.AddSourceAsset(element.GetProxyElement("ShipDrop"), new HashSet<XElement> { element.GetProxyElement("ShipDrop") });
                  break;
                }
                var progressions = GetProgession(element, id);
                if (progressions != null) {
                  foreach (var item in progressions) {
                    result.AddSourceAsset(element.GetProxyElement("Harbor"), new HashSet<XElement> { element.GetProxyElement(item) });
                  }

                  break;
                }
              }
              goto case "A7_QuestEscortObject";
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
              if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) {
                result.AddSourceAsset(element, new HashSet<XElement> { element });
              }
              break;

            case "DivingBellShip":
              if (element.Descendants("DivingBell").FirstOrDefault()?.Descendants("ReplacementPool").Any(rp => rp.Value == id) == true) {
                result.AddSourceAsset(element.GetProxyElement("Dive"), new HashSet<XElement> { element.GetProxyElement("Dive") });
              }
              break;

            case "ItemWithUI":
              if (element.Descendants("NewItem").Any(ni => ni.Value == id)) {
                result.AddSourceAsset(element, new HashSet<XElement> { element });
              }
              else if (element.Descendants("ActionAddResource").Any(e => e.Element("Resource").Value == id)) {
                result.AddSourceAsset(element.GetProxyElement("Item"), new HashSet<XElement> { element.GetProxyElement("Item") });
              }
              else {

              }
              break;



            case "Profile_3rdParty_ItemCrafter":
              var craftable = element.Descendants("CraftableItems").FirstOrDefault()?.Descendants("Item").Any(item => item?.Value == id);
              if (craftable == true) {
                result.AddSourceAsset(element.GetProxyElement("Crafting"), new HashSet<XElement> { element.GetProxyElement("Crafting") });
              }
              else {
                var progressions = GetProgession(element, id);
                if (progressions != null) {
                  foreach (var item in progressions) {
                    result.AddSourceAsset(element.GetProxyElement("Harbor"), new HashSet<XElement> { element.GetProxyElement(item) });
                  }

                  break;
                }
              }
              break;

            case "TourismFeature":
              var pool = element.Descendants("Pool").FirstOrDefault(p => p.Value == id)?.Parent;
              if (pool != null) {
                result.AddSourceAsset(element, new HashSet<XElement> { pool });
              }
              break;

            case "ExpeditionDecision":
              //Add Detail if has reward or is InsertEvent
              var reward = element.XPathSelectElement("Values/Reward/RewardAssets");
              if ((reward?.Elements("Item").Any(l => l.Element("Reward").Value == id) ?? false)
                  || element.XPathSelectElements($"//*[text()={id} and not(self::GUID) and (self::InsertEvent)]").Any()) {
                Details.Add(element);
              }
              goto case "ExpeditionOption";
            case "ExpeditionOption":
            case "ExpeditionTrade":
            case "ExpeditionEvent":
            case "ExpeditionEventPool":
              if (SavedSources.ContainsKey(key)) {
                result.AddSourceAsset(SavedSources[key].Copy(), Details);
                break;
              }
              goto case "RewardPool";
            case "RewardPool":
            case "RewardItemPool":
            case "ResourcePool":

              if (SavedSources.ContainsKey(key)) {
                result.AddSourceAsset(SavedSources[key].Copy());
                break;
              }
              var weight = element.Descendants("Item").FirstOrDefault(f => f.Elements().Any(l => l.Value == id))?.Element("Weight")?.Value;
              if (weight == "0") {
                break;
              }
              FindSources(key, Details, result);
              if (!SavedSources.ContainsKey(key)) {
                SavedSources.TryAdd(key, result.Copy());
              }
              break;

            default:
              //throw new NotImplementedException(element.Element("Template").Value);
              Debug.WriteLine(element.Element("Template").Value);
              break;
          }
          resultstoadd.Add(result);
        }
      }

      foreach (var item in resultstoadd) {
        mainResult.AddSourceAsset(item);
      }
      return mainResult;
    }

    private IEnumerable<string> GetProgession(XElement element, string id) {
      var progressions = element.Descendants().FirstOrDefault(e => e.Name.LocalName == "Progression")?.Elements();
      if (progressions != null) {
        return progressions.Where(p => p.Descendants().Any(e => e.Value == id && e.Parent.Element("Weight")?.Value != "0")).Select(s => s.Name.LocalName);
      }
      else {
        var strs = element.Descendants("Pool").Where(e => e.Value == id).Select(e => e.Parent.Name.LocalName);
        return strs;
      }
    }

    #endregion Methods
  }
}