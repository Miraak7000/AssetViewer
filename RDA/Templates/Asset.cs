using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;
using RDA.Library;

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
    //
    public List<TempSource> Sources { get; set; }
    #endregion

    #region Constructor
    public Asset(XElement asset) {
      foreach (var element in asset.Element("Values").Elements()) {
        switch (element.Name.LocalName) {
          case "Text":
          case "Locked":
          case "Buff":
            // ignore this nodes
            break;
          case "ItemAction":
          case "IncidentInfluencerUpgrade":
          case "IncidentInfectableUpgrade":
          case "ItemGeneratorUpgrade":
          case "PassiveTradeGoodGenUpgrade":
          case "Cost":
          case "TradeShipUpgrade":
            // TODO: should be implemented one day
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
          default:
            throw new NotImplementedException(element.Name.LocalName);
        }
      }
      var sources = this.FindSources(this.ID, new List<String>()).ToArray();
      this.Sources = sources.Select(s => new TempSource(s)).ToList();
    }
    public String Path = String.Empty;
    #endregion


    #region Public Methods
    public XElement ToXml() {
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
      result.Add(new XElement("BuildingUpgrades", this.BuildingUpgrades == null ? null : this.BuildingUpgrades.Select(s => s.ToXml())));
      result.Add(new XElement("CultureUpgrades", this.CultureUpgrades == null ? null : this.CultureUpgrades.Select(s => s.ToXml())));
      result.Add(new XElement("ModuleOwnerUpgrades", this.ModuleOwnerUpgrades == null ? null : this.ModuleOwnerUpgrades.Select(s => s.ToXml())));
      result.Add(new XElement("ResidenceUpgrades", this.ResidenceUpgrades == null ? null : this.ResidenceUpgrades.Select(s => s.ToXml())));
      result.Add(new XElement("PopulationUpgrades", this.PopulationUpgrades == null ? null : this.PopulationUpgrades.Select(s => s.ToXml())));
      result.Add(new XElement("ElectricUpgrades", this.ElectricUpgrades == null ? null : this.ElectricUpgrades.Select(s => s.ToXml())));
      result.Add(new XElement("ExpeditionAttributes", this.ExpeditionAttributes == null ? null : this.ExpeditionAttributes.Select(s => s.ToXml())));
      result.Add(new XElement("AttackableUpgrades", this.AttackableUpgrades == null ? null : this.AttackableUpgrades.Select(s => s.ToXml())));
      //
      result.Add(new XElement("TradePrice", this.TradePrice));
      //
      if (this.Info != null) result.Add(this.Info.ToXml("Info"));
      //
      result.Add(new XElement("Sources", this.Sources.Select(s => s.ToXml())));
      return result;
    }
    public override String ToString() {
      return $"{this.ID} - {this.Name}";
    }
    #endregion

    #region Private Methods
    private void ProcessElement_Standard(XElement element) {
      this.ID = element.Element("GUID").Value;
      this.Name = element.Element("Name").Value;
      this.Icon = new Icon(element.Element("IconFilename").Value);
      this.Text = new Description(element.Element("GUID").Value);
      this.Info = element.Element("InfoDescription") == null ? null : new Description(element.Element("InfoDescription").Value);
    }
    private void ProcessElement_Item(XElement element) {
      this.Rarity = element.Element("Rarity") == null ? new Description("118002") : new Description(Helper.GetDescriptionID(element.Element("Rarity").Value));
      this.ItemType = element.Element("ItemType")?.Value ?? "Common";
      this.Allocation = new Allocation(element.Parent.Parent.Element("Template").Value, element.Element("Allocation")?.Value);
      this.TradePrice = element.Element("TradePrice") == null ? null : (Int32.Parse(element.Element("TradePrice").Value) / 4).ToString();
      if (this.ItemType == "None") this.ItemType = "Common";
      if (this.ItemType != "Specialist" && this.ItemType != "Common") throw new NotImplementedException();
    }
    private void ProcessElement_ItemEffect(XElement element) {
      if (element.HasElements && element.Element("EffectTargets") == null) throw new NotImplementedException();
      if (element.HasElements && element.Element("EffectTargets").HasElements) {
        this.EffectTargets = new List<Description>();
        foreach (var item in element.Element("EffectTargets").Elements()) {
          this.EffectTargets.Add(new Description(Program.DescriptionEN[item.Value], Program.DescriptionDE[item.Value]));
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
          if (item.Name.LocalName == "ResolverUnitCountUpgrade") continue;
          if (item.Name.LocalName == "PublicServiceFullSatisfactionDistance") continue;
          if (item.Name.LocalName == "PublicServiceNoSatisfactionDistance") continue;
          if (item.Name.LocalName == "ResolverUnitMovementSpeedUpgrade") continue;
          if (item.Name.LocalName == "ResolverUnitDecreaseUpgrade") continue;
          this.BuildingUpgrades.Add(new Upgrade(item));
        }
      }
    }
    private void ProcessElement_CultureUpgrade(XElement element) {
      if (element.HasElements) {
        this.CultureUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
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
          // TODO: this needs to be implemented
          if (item.Name.LocalName == "NeedProvideNeedUpgrade") continue;
          if (item.Name.LocalName == "GoodConsumptionUpgrade") continue;
          if (item.Name.LocalName == "ChangedSupplyValueUpgrade") continue;
          this.ResidenceUpgrades.Add(new Upgrade(item));
        }
      }
    }
    private void ProcessElement_PopulationUpgrade(XElement element) {
      if (element.HasElements) {
        this.PopulationUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          this.PopulationUpgrades.Add(new Upgrade(item));
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
            if (attribute.Element("Attribute") == null) continue;
            if (attribute.Element("Attribute").Value == "PerkFemale") continue;
            if (attribute.Element("Attribute").Value == "PerkMale") continue;
            this.ExpeditionAttributes.Add(new Upgrade(attribute.Element("Attribute").Value, attribute.Element("Amount")?.Value));
          }
        }
      }
    }
    private void ProcessElement_VisitorHarborUpgrade(XElement element) {
      if (element.HasElements) {
        // TODO: needs to be implemented
      }
    }
    private void ProcessElement_AttackerUpgrade(XElement element) {
      if (element.HasElements) {
        // TODO: this needs to be implemented
      }
    }
    private void ProcessElement_ShipyardUpgrade(XElement element) {
      if (element.HasElements) {
        // TODO: needs to be implemented
      }
    }
    private void ProcessElement_AttackableUpgrade(XElement element) {
      if (element.HasElements) {
        this.AttackableUpgrades = new List<Upgrade>();
        foreach (var item in element.Elements()) {
          // TODO: this needs to be implemented
          if (item.Name.LocalName == "SelfHealUpgrade") continue;
          if (item.Name.LocalName == "SelfHealPausedTimeIfAttackedUpgrade") continue;
          if (item.Name.LocalName == "DamageReceiveFactor") continue;
          if (item.Name.LocalName == "MoralePowerUpgrade") continue;
          this.AttackableUpgrades.Add(new Upgrade(item));
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
        // TODO: this needs to be implemented
      }
    }
    private void ProcessElement_RepairCraneUpgrade(XElement element) {
      if (element.HasElements) {
        // TODO: this needs to be implemented
      }
    }
    private void ProcessElement_KontorUpgrade(XElement element) {
      if (element.HasElements) {
        // TODO: needs to be implemented
      }
    }
    private List<XElement> FindSources(String id, List<String> previousIDs) {
      previousIDs.Add(id);
      var result = new List<XElement>();
      var links = Program.Original.Root.XPathSelectElements($"//*[text()={id} and not(self::GUID) and not(self::InsertEvent)]").ToArray();
      if (links.Length > 0) {
        for (int i = 0; i < links.Length; i++) {
          var element = links[i];
          while (true) {
            if (element.Name.LocalName == "Asset" && element.HasElements) break;
            element = element.Parent;
          }
          if (element.Element("Template") == null) continue;
          switch (element.Element("Template").Value) {
            case "AssetPool":
            case "ExpeditionTrade":
            case "TutorialQuest":
            case "SettlementRightsFeature":
              // ignore
              break;
            case "Expedition":
            case "Profile_3rdParty":
            case "Profile_3rdParty_Pirate":
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
            case "TourismFeature":
              if (!element.XPathSelectElement("Values/Standard/Name").Value.Contains("Test")) result.AddSourceAsset(element);
              break;
            case "RewardPool":
            case "ExpeditionDecision":
            case "ExpeditionOption":
            case "ExpeditionEvent":
            case "ExpeditionEventPool":
            case "RewardItemPool":
              if (previousIDs.Contains(element.XPathSelectElement("Values/Standard/GUID").Value)) continue;
              var items = this.FindSources(element.XPathSelectElement("Values/Standard/GUID").Value, previousIDs);
              foreach (var item in items) {
                result.AddSourceAsset(item);
              }
              break;
            default:
              throw new NotImplementedException(element.Element("Template").Value);
          }
        }
      }
      return result;
    }
    #endregion

  }

}