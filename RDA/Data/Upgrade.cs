using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class Upgrade {

    #region Properties
    public Icon Icon { get; set; }
    public Description Text { get; set; }
    public String Value { get; set; }
    public List<AdditionalOutput> AdditionalOutputs { get; set; }
    public List<ReplaceInput> ReplaceInputs { get; set; }
    public List<InputAmountUpgrade> InputAmountUpgrades { get; set; }
    public ReplacingWorkforce ReplacingWorkforce { get; set; }
    public List<Upgrade> Additionals { get; set; }
    #endregion

    #region Constructor
    public Upgrade(XElement element) {
      var isPercent = element.Element("Percental") == null ? false : element.Element("Percental").Value == "1";
      var value = element.Element("Value") == null ? null : (Int32?)Int32.Parse(element.Element("Value").Value);
      var factor = 1;
      switch (element.Name.LocalName) {
        case "ProductivityUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_options.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "AdditionalOutput":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_plus.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          this.AdditionalOutputs = new List<AdditionalOutput>();
          foreach (var item in element.Elements()) {
            this.AdditionalOutputs.Add(new AdditionalOutput(item));
          }
          break;
        case "ReplaceInputs":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_traderoutes.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          this.ReplaceInputs = new List<ReplaceInput>();
          foreach (var item in element.Elements()) {
            this.ReplaceInputs.Add(new ReplaceInput(item));
          }
          break;
        case "InputAmountUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_options.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          this.InputAmountUpgrades = new List<InputAmountUpgrade>();
          foreach (var item in element.Elements()) {
            this.InputAmountUpgrades.Add(new InputAmountUpgrade(item));
          }
          break;
        case "OutputAmountFactorUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_options.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "NeededAreaPercentUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_general_module_01.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          isPercent = true;
          break;
        case "AddedFertility":
          var addedFertility = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={element.Value}]");
          this.Icon = new Icon(addedFertility.Element("IconFilename").Value);
          this.Text = new Description("Provided", "bereitgestellt");
          break;
        case "NeedsElectricity":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_electricity.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "AttractivenessUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_attractiveness.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "MaintenanceUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_credits.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "WorkforceAmountUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_options.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "ReplacingWorkforce":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_build_menu.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          this.ReplacingWorkforce = new ReplacingWorkforce(element.Value);
          break;
        case "ModuleLimitUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_general_module_01.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "AdditionalHappiness":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_happy.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          value = Int32.Parse(element.Value);
          break;
        case "ResidentsUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_kontor_2d.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "StressUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_incident_riot.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "ProvideElectricity":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_electricity.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "TaxModifierInPercent":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_credits.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          isPercent = true;
          break;
        case "WorkforceModifierInPercent":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_kontor_2d.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          value = Int32.Parse(element.Value);
          isPercent = true;
          break;
        case "MaxHitpointsUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_plus.png");
          this.Text = new Description(Helper.GetDescriptionID(element.Name.LocalName));
          break;
        case "ActiveTradePriceInPercent":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_credits.png");
          this.Text = new Description("15198");
          if (value == null && !element.HasElements) {
            value = Int32.Parse(element.Value);
            if (value < 100) {
              value = -(100 - value);
            } else {
              value = (value - 100);
            }
          }
          isPercent = true;
          break;
        case "ForwardSpeedUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_forward.png");
          this.Text = new Description("2339");
          break;
        case "IgnoreWeightFactorUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_diplomacy_options_support_fleet.png");
          this.Text = new Description("15261");
          value = -value;
          break;
        case "IgnoreDamageFactorUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_diplomacy_options_support_fleet.png");
          this.Text = new Description("15262");
          value = -value;
          break;
        case "AttackRangeUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_melee_tint.png");
          this.Text = new Description("12021");
          break;
        case "ActivateWhiteFlag":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_claim_island.png");
          this.Text = new Description("19538");
          break;
        case "ActivatePirateFlag":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_melee_tint.png");
          this.Text = new Description("500937");
          break;
        case "Cannon":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_stance_attack.png");
          this.Text = new Description("19138");
          value = -Convert.ToInt32((100M - (100M * Decimal.Parse(element.Element("Factor").Value))));
          isPercent = true;
          break;
        case "BigBertha":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_stance_attack.png");
          this.Text = new Description("19139");
          value = -Convert.ToInt32((100M - (100M * Decimal.Parse(element.Element("Factor").Value))));
          isPercent = true;
          break;
        case "Torpedo":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_stance_attack.png");
          this.Text = new Description("19137");
          value = -Convert.ToInt32((100M - (100M * Decimal.Parse(element.Element("Factor").Value))));
          isPercent = true;
          break;
        case "AttackSpeedUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_go_to.png");
          this.Text = new Description("17230");
          isPercent = true;
          break;
        case "SpawnProbabilityFactor":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_add_slot_guild.png");
          this.Text = new Description("20603");
          break;
        case "SelfHealUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_plus.png");
          this.Text = new Description("15195");
          break;
        case "SelfHealPausedTimeIfAttackedUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_morale.png");
          this.Text = new Description("15196");
          if (value == -100) {
            value = null;
          } else {
            throw new NotImplementedException();
          }
          break;
        case "HealRadiusUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_build_menu.png");
          this.Text = new Description("15264");
          break;
        case "HealPerMinuteUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_build_menu.png");
          this.Text = new Description("15265");
          break;
        case "IncidentRiotIncreaseUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_incident_riot.png");
          this.Text = new Description("12227");
          factor = 10;
          isPercent = true;
          break;
        case "PublicServiceFullSatisfactionDistance":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_church_2d.png");
          this.Text = new Description("2321");
          break;
        case "NeedProvideNeedUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_plus.png");
          this.Text = new Description("12315");
          this.Additionals = new List<Upgrade>();
          foreach (var item in element.XPathSelectElements("Item/ProvidedNeed")) {
            this.Additionals.Add(new Upgrade(item));
          }
          break;
        case "ProvidedNeed":
          var providedNeed = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={element.Value}]");
          this.Icon = new Icon(providedNeed.Element("IconFilename").Value);
          this.Text = new Description(element.Value);
          break;
        case "AdditionalMoney":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_credits.png");
          this.Text = new Description("12690");
          value = Int32.Parse(element.Value);
          break;
        case "IncidentFireIncreaseUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_incident_fire.png");
          this.Text = new Description("12225");
          factor = 10;
          isPercent = true;
          break;
        case "IncidentExplosionIncreaseUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_bomb.png");
          this.Text = new Description("14292");
          factor = 10;
          isPercent = true;
          break;
        case "GoodConsumptionUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_marketplace_2d.png");
          this.Text = new Description("21386");
          this.Additionals = new List<Upgrade>();
          foreach (var item in element.XPathSelectElements("Item/ProvidedNeed")) {
            this.Additionals.Add(new Upgrade(item));
          }
          break;
        case "DamageFactor":
          var damageFactor = element.Elements().Single();
          switch (damageFactor.Name.LocalName) {
            case "Building":
              this.Icon = new Icon("data/ui/2kimages/main/icons/ship_info/icon_damage.png");
              this.Text = new Description("17394");
              value = Convert.ToInt32((Decimal.Parse(damageFactor.Value) * 100) - 100);
              isPercent = true;
              break;
            case "SailShip":
              this.Icon = new Icon("data/ui/2kimages/main/icons/ship_info/icon_damage.png");
              this.Text = new Description("17395");
              value = Convert.ToInt32((Decimal.Parse(damageFactor.Value) * 100) - 100);
              isPercent = true;
              break;
            case "SteamShip":
              this.Icon = new Icon("data/ui/2kimages/main/icons/ship_info/icon_damage.png");
              this.Text = new Description("17396");
              value = Convert.ToInt32((Decimal.Parse(damageFactor.Value) * 100) - 100);
              isPercent = true;
              break;
            default:
              throw new NotImplementedException();
          }
          break;
        case "LoadingSpeedUpgrade":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_load_ships.png");
          this.Text = new Description("15197");
          break;
        default:
          throw new NotImplementedException(element.Name.LocalName);
      }
      if (value == null) {
        this.Value = String.Empty;
      } else {
        if (isPercent) {
          this.Value = value > 0 ? $"+{value * factor}%" : $"{value * factor}%";
        } else {
          this.Value = value > 0 ? $"+{value * factor}" : $"{value * factor}";
        }
      }
    }
    public Upgrade(String key, String amount) {
      var value = amount == null ? null : (Int32?)Int32.Parse(amount);
      switch (key) {
        case "Hunting":
          this.Icon = new Icon("data/ui/2kimages/main/icons/ship_info/icon_damage.png");
          this.Text = new Description(Helper.GetDescriptionID(key));
          break;
        case "Navigation":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_navigation_tint.png");
          this.Text = new Description(Helper.GetDescriptionID(key));
          break;
        case "Crafting":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_build_menu.png");
          this.Text = new Description(Helper.GetDescriptionID(key));
          break;
        case "Might":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_navalbattle_tint.png");
          this.Text = new Description(Helper.GetDescriptionID("Might"));
          break;
        case "Melee":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_melee_tint.png");
          this.Text = new Description("3921");
          break;
        case "Diplomacy":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_diplomacy_option_negotiate.png");
          this.Text = new Description(Helper.GetDescriptionID(key));
          break;
        case "Faith":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_church_2d.png");
          this.Text = new Description(Helper.GetDescriptionID(key));
          break;
        case "Medicine":
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_diseases_tint.png");
          this.Text = new Description(Helper.GetDescriptionID(key));
          break;
        case "PerkFormerPirate":
          value = null;
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_expedition_pirate.png");
          this.Text = new Description(Helper.GetDescriptionID(key)).InsertBefore("Trait: ", "Merkmal: ");
          break;
        case "PerkDiver":
          value = null;
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_expedition_diver.png");
          this.Text = new Description(Helper.GetDescriptionID(key)).InsertBefore("Trait: ", "Merkmal: ");
          break;
        case "PerkZoologist":
          value = null;
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_expedition_zoologist.png");
          this.Text = new Description(Helper.GetDescriptionID(key)).InsertBefore("Trait: ", "Merkmal: ");
          break;
        case "PerkMilitaryShip":
          value = null;
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_expedition_goto.png");
          this.Text = new Description(Helper.GetDescriptionID(key)).InsertBefore("Trait: ", "Merkmal: ");
          break;
        case "PerkHypnotist":
          value = null;
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_expedition_hypnotist.png");
          this.Text = new Description(Helper.GetDescriptionID(key)).InsertBefore("Trait: ", "Merkmal: ");
          break;
        case "PerkAnthropologist":
          value = null;
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_expedition_anthropologist.png");
          this.Text = new Description(Helper.GetDescriptionID(key)).InsertBefore("Trait: ", "Merkmal: ");
          break;
        case "PerkPolyglot":
          value = null;
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_generic_expedition.png");
          this.Text = new Description(Helper.GetDescriptionID(key)).InsertBefore("Trait: ", "Merkmal: ");
          break;
        case "PerkArcheologist":
          value = null;
          this.Icon = new Icon("data/ui/2kimages/main/icons/icon_generic_expedition.png");
          this.Text = new Description(Helper.GetDescriptionID(key)).InsertBefore("Trait: ", "Merkmal: ");
          break;
        default:
          throw new NotImplementedException(key);
      }
      if (value == null) {
        this.Value = String.Empty;
      } else {
        this.Value = value.ToString();
      }
    }
    #endregion

    #region Public Methods
    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      result.Add(this.Icon.ToXml());
      result.Add(this.Text.ToXml("Text"));
      result.Add(new XElement("Value", this.Value));
      if (this.AdditionalOutputs != null) result.Add(new XElement("AdditionalOutputs", this.AdditionalOutputs.Select(s => s.ToXml())));
      if (this.ReplaceInputs != null) result.Add(new XElement("ReplaceInputs", this.ReplaceInputs.Select(s => s.ToXml())));
      if (this.InputAmountUpgrades != null) result.Add(new XElement("InputAmountUpgrades", this.InputAmountUpgrades.Select(s => s.ToXml())));
      if (this.ReplacingWorkforce != null) result.Add(new XElement("ReplacingWorkforce", this.ReplacingWorkforce.ToXml()));
      if (this.Additionals != null) result.Add(new XElement("Additionals", this.Additionals.Select(s => s.ToXml())));
      return result;
    }
    #endregion

  }

}