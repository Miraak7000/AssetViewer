using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class Upgrade {

    #region Properties
    public Description Text { get; set; }
    public String Value { get; set; }
    public List<AdditionalOutput> AdditionalOutputs { get; set; }
    public List<ReplaceInput> ReplaceInputs { get; set; }
    public List<InputAmountUpgrade> InputAmountUpgrades { get; set; }
    public ReplacingWorkforce ReplacingWorkforce { get; set; }
    public List<Upgrade> Additionals { get; set; }

    #endregion Properties

    #region Constructors

    public Upgrade() {
    }

    public Upgrade(XElement element) {
      var isPercent = element.Element("Percental") == null ? false : element.Element("Percental").Value == "1";
      var value = element.Element("Value") == null ? null : (Int32?)Int32.Parse(element.Element("Value").Value);
      var factor = 1;
      switch (element.Name.LocalName) {
        case "PassiveTradeGoodGenUpgrade":
          this.Text = new Description("12920") { AdditionalInformation = new Description("20327", DescriptionFontStyle.Light) };
          var genpool = element.Element("GenPool").Value;
          var items = Assets
            .Original
            .Descendants("Asset")
            .FirstOrDefault(a => a.XPathSelectElement("Values/Standard/GUID")?.Value == genpool)
            .XPathSelectElement("Values/RewardPool")
            .Elements("Item")
            .Select(i => i.Element("ItemLink").Value)
            .Select(i => new Description(i));
          this.Text.AdditionalInformation.EN.Replace("[ItemAssetData([RefGuid]) GoodGenerationPoolFormatted]", string.Join(",", items.Select(d => d.EN)));
          this.Text.AdditionalInformation.DE.Replace("[ItemAssetData([RefGuid]) GoodGenerationPoolFormatted]", string.Join(",", items.Select(d => d.DE)));
          value = Convert.ToInt32(element.Value);
          isPercent = true;
          break;

        case "AddAssemblyOptions":
          this.Text = new Description("12693") { AdditionalInformation = new Description("20325", DescriptionFontStyle.Light) };
          var descs = element.Elements("Items").Select(i => new Description(i.Element("NewOption").Value));
          this.Text.AdditionalInformation.EN.Replace("[ItemAssetData([RefGuid]) AddAssemblyOptionsFormatted]", string.Join(",", descs.Select(d => d.EN)));
          this.Text.AdditionalInformation.DE.Replace("[ItemAssetData([RefGuid]) AddAssemblyOptionsFormatted]", string.Join(",", descs.Select(d => d.DE)));
          break;

        case "MoraleDamage":
          this.Text = new Description("21588") { AdditionalInformation = new Description("21586", DescriptionFontStyle.Light) };
          break;

        case "HitpointDamage":
          switch (element.Parent.Parent.Element("Item").Element("Allocation").Value) {
            case "Ship":
            case "SailShip":
            case "Warship":
            case "SteamShip":
              this.Text = new Description("21587") { AdditionalInformation = new Description("21585", DescriptionFontStyle.Light) };
              break;

            default:
              this.Text = new Description("21587") { AdditionalInformation = new Description("21589", DescriptionFontStyle.Light) };
              break;
          }

          break;

        case "SpecialUnitHappinessThresholdUpgrade":
          this.Text = new Description("19625");
          this.Text.AdditionalInformation = new Description("21584", DescriptionFontStyle.Light);
          var target = element.Parent.Parent.Element("ItemEffect").Element("EffectTargets").Elements().FirstOrDefault().Element("GUID").Value;
          Description unit = null;
          switch (target) {
            case "190777": //Hospital
              unit = new Description("100584");
              //volunteer = new Description("100583");
              break;

            case "190776": //Police Station
              unit = new Description("100582");
              //volunteer = new Description("100581");
              break;

            case "190775": //Fire Station
              unit = new Description("100580");
              //volunteer = new Description("100579");
              break;

            default:
              throw new NotImplementedException(target);
          }
          this.Text.AdditionalInformation.EN = this.Text.EN.Replace("[AssetData([ToolOneHelper IncidentResolverUnitsForTargetBuildings([RefGuid], 1) AT(0)]) Text]", unit.EN);
          this.Text.AdditionalInformation.DE = this.Text.DE.Replace("[AssetData([ToolOneHelper IncidentResolverUnitsForTargetBuildings([RefGuid], 1) AT(0)]) Text]", unit.DE);
          break;

        case "BlockBuyShare":
          this.Text = new Description("15802");
          break;

        case "HappinessIgnoresMorale":
          this.Text = new Description("15811") { AdditionalInformation = new Description("20326", DescriptionFontStyle.Light) };
          break;

        case "BlockHostileTakeover":
          this.Text = new Description("15801");
          break;

        case "MaintainanceUpgrade":
          this.Text = new Description("2320");
          break;

        case "MoralePowerUpgrade":
          this.Text = new Description("15231");
          break;

        case "ConstructionCostInPercent":
          this.Text = new Description("12679");
          value = Int32.Parse(element.Value);
          isPercent = true;
          break;

        case "ConstructionTimeInPercent":
          this.Text = new Description("12678");
          value = Int32.Parse(element.Value);
          isPercent = true;
          break;

        case "AdditionalSupply":
          this.Text = new Description("12687");
          this.Text.DE = this.Text.DE.Replace(" [AssetData([ItemAssetData([RefGuid]) InputBenefitModifierProduct(index)]) Text]", "");
          this.Text.EN = this.Text.DE.Replace(" [AssetData([ItemAssetData([RefGuid]) InputBenefitModifierProduct(index)]) Text]", "");
          value = Int32.Parse(element.Value);
          break;

        case "ChangedSupplyValueUpgrade":
          this.Text = new Description("12649");
          this.Additionals = new List<Upgrade>();
          foreach (var item in element.Elements("Item")) {
            this.Additionals.Add(new Upgrade() { Text = new Description(item.Element("Need").Value), Value = (item.Element("AmountInPercent").Value.StartsWith("-") ? "" : "+") + $"{item.Element("AmountInPercent").Value}%" });
          }
          break;

        case "ResolverUnitDecreaseUpgrade":
          target = element.Parent.Parent.Element("ItemEffect").Element("EffectTargets").Elements().FirstOrDefault().Element("GUID").Value;
          unit = null;
          switch (target) {
            case "190777": //Hospital
              this.Text = new Description("12012");
              //unit = new Description("100583");
              break;

            case "190776": //Police Station
              this.Text = new Description("21509");
              //unit = new Description("100581");
              break;

            case "190775": //Fire Station
            case "1010463": //Fire Department
              this.Text = new Description("21508");
              //unit = new Description("100579");
              break;

            default:
              throw new NotImplementedException(target);
          }
          break;

        case "ResolverUnitCountUpgrade":
          target = element
            .Parent
            .Parent
            .Element("ItemEffect")
            .Element("EffectTargets")
            .Elements()
            .FirstOrDefault()
            .Element("GUID")
            .Value;
          unit = null;
          switch (target) {
            case "190777": //Hospital
              this.Text = new Description("100583");
              break;

            case "190776": //Police Station
              this.Text = new Description("100581");
              break;

            case "190775": //Fire Station
            case "1010463": //Fire Department
              this.Text = new Description("100579");
              break;

            default:
              throw new NotImplementedException(target);
          }
          break;

        case "ResolverUnitMovementSpeedUpgrade":
          this.Text = new Description("12014");
          this.Value = null;
          break;

        case "ProductivityUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_options.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "AdditionalOutput":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_plus.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          this.AdditionalOutputs = new List<AdditionalOutput>();
          foreach (var item in element.Elements()) {
            this.AdditionalOutputs.Add(new AdditionalOutput(item));
          }
          break;

        case "ReplaceInputs":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_traderoutes.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          this.ReplaceInputs = new List<ReplaceInput>();
          foreach (var item in element.Elements()) {
            this.ReplaceInputs.Add(new ReplaceInput(item));
          }
          break;

        case "InputAmountUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_options.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          this.InputAmountUpgrades = new List<InputAmountUpgrade>();
          foreach (var item in element.Elements()) {
            this.InputAmountUpgrades.Add(new InputAmountUpgrade(item));
          }
          break;

        case "OutputAmountFactorUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_options.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "NeededAreaPercentUpgrade":
          // this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_general_module_01.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          isPercent = true;
          break;

        case "IncidentIllnessIncreaseUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_incident_diseases.png");
          this.Text = new Description("12226");
          isPercent = true;
          factor = 10;
          break;

        case "AddedFertility":
          this.Text = new Description(element.Value);
          this.Text.EN += " Provided";
          this.Text.DE += " bereitgestellt";
          break;

        case "NeedsElectricity":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_electricity.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "AttractivenessUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_attractiveness.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "MaintenanceUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_credits.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "WorkforceAmountUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_options.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "ReplacingWorkforce":
          // this.Icon = new Icon("data/ui/2kimages/main/icons/icon_build_menu.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          this.ReplacingWorkforce = new ReplacingWorkforce(element.Value);
          break;

        case "ModuleLimitUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/3dicons/icon_general_module_01.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "AdditionalHappiness":
          // this.Icon = new Icon("data/ui/2kimages/main/icons/icon_happy.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          value = Int32.Parse(element.Value);
          break;

        case "ResidentsUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_house.png");
          this.Text = new Description("2322");
          break;

        case "StressUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_incident_riot.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "ProvideElectricity":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_electricity.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "TaxModifierInPercent":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_credits.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          value = Int32.Parse(element.Value);
          isPercent = true;
          break;

        case "WorkforceModifierInPercent":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_kontor_2d.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          value = Int32.Parse(element.Value);
          isPercent = true;
          break;

        case "MaxHitpointsUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_plus.png");
          this.Text = new Description(Assets.GetDescriptionID(element.Name.LocalName));
          break;

        case "ActiveTradePriceInPercent":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_credits.png");
          this.Text = new Description("15198");
          if (value == null && !element.HasElements) {
            value = Int32.Parse(element.Value);
            if (value < 100) {
              value = -(100 - value);
            }
            else {
              value = (value - 100);
            }
          }
          isPercent = true;
          break;

        case "ForwardSpeedUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_forward.png");
          this.Text = new Description("2339");
          break;

        case "IgnoreWeightFactorUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_diplomacy_options_support_fleet.png");
          this.Text = new Description("15261");
          value = -value;
          break;

        case "IgnoreDamageFactorUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_diplomacy_options_support_fleet.png");
          this.Text = new Description("15262");
          value = -value;
          break;

        case "AttackRangeUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_melee_tint.png");
          this.Text = new Description("12021");
          break;

        case "ActivateWhiteFlag":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_claim_island.png");
          this.Text = new Description("19538");
          this.Text.Icon = new Icon("data/ui/2kimages/main/icons/icon_claim_island.png");
          this.Text.AdditionalInformation = new Description("19487", DescriptionFontStyle.Light);
          break;

        case "ActivatePirateFlag":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_melee_tint.png");
          this.Text = new Description("17392");
          this.Text.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_melee_tint.png");
          this.Text.AdditionalInformation = new Description("17393", DescriptionFontStyle.Light);
          break;

        case "Normal":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_stance_attack.png");
          this.Text = new Description("19136");
          value = -Convert.ToInt32((100M - (100M * Decimal.Parse(element.Element("Factor").Value, CultureInfo.InvariantCulture))));
          isPercent = true;
          break;

        case "Cannon":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_stance_attack.png");
          this.Text = new Description("19138");
          value = -Convert.ToInt32((100M - (100M * Decimal.Parse(element.Element("Factor").Value, CultureInfo.InvariantCulture))));
          isPercent = true;
          break;

        case "BigBertha":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_stance_attack.png");
          this.Text = new Description("19139");
          value = -Convert.ToInt32((100M - (100M * Decimal.Parse(element.Element("Factor").Value, CultureInfo.InvariantCulture))));
          isPercent = true;
          break;

        case "Torpedo":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_stance_attack.png");
          this.Text = new Description("19137");
          value = -Convert.ToInt32((100M - (100M * Decimal.Parse(element.Element("Factor").Value, CultureInfo.InvariantCulture))));
          isPercent = true;
          break;

        case "AttackSpeedUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_go_to.png");
          this.Text = new Description("17230");
          if (value == null) {
            value = element.Value == null ? null : (Int32?)Int32.Parse(element.Value);
          }
          isPercent = true;
          break;

        case "SpawnProbabilityFactor":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_add_slot_guild.png");
          this.Text = new Description("20603");
          break;

        case "SelfHealUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_plus.png");
          this.Text = new Description("15195");
          break;

        case "SelfHealPausedTimeIfAttackedUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_morale.png");
          this.Text = new Description("15196");
          this.Text.AdditionalInformation = new Description("21590", DescriptionFontStyle.Light);
          if (value == -100) {
            value = null;
          }
          else {
            throw new NotImplementedException();
          }
          break;

        case "HealRadiusUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_build_menu.png");
          this.Text = new Description("15264");
          break;

        case "HealPerMinuteUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_build_menu.png");
          this.Text = new Description("15265");
          break;

        case "IncidentRiotIncreaseUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_incident_riot.png");
          this.Text = new Description("12227");
          factor = 10;
          isPercent = true;
          break;

        case "PublicServiceFullSatisfactionDistance":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_church_2d.png");
          this.Text = new Description("2321");
          break;

        case "NeedProvideNeedUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_plus.png");
          this.Text = new Description("12315");

          var SubstituteNeeds = element.Descendants("SubstituteNeed").Select(i => new Description(i.Value));
          var ProvidedNeeds = element.Descendants("SubstituteNeed").Select(i => new Description(i.Value));
          this.Text.AdditionalInformation = new Description("20323", DescriptionFontStyle.Light);
          this.Text.AdditionalInformation.EN.Replace("[ItemAssetData([RefGuid]) AllSubstituteNeedsFormatted]", string.Join(",", SubstituteNeeds.Select(d => d.EN)));
          this.Text.AdditionalInformation.EN.Replace("[ItemAssetData([RefGuid]) AllProvidedNeedsFormatted]", string.Join(",", ProvidedNeeds.Select(d => d.EN)));

          this.Text.AdditionalInformation.DE.Replace("[ItemAssetData([RefGuid]) AllSubstituteNeedsFormatted]", string.Join(",", SubstituteNeeds.Select(d => d.DE)));
          this.Text.AdditionalInformation.DE.Replace("[ItemAssetData([RefGuid]) AllProvidedNeedsFormatted]", string.Join(",", ProvidedNeeds.Select(d => d.DE)));
          break;

        case "ProvidedNeed":
          var providedNeed = Assets.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={element.Value}]");
          // this.Icon = new Icon(providedNeed.Element("IconFilename").Value);
          this.Text = new Description(element.Value);
          break;

        case "AdditionalMoney":
          // this.Icon = new Icon("data/ui/2kimages/main/icons/icon_credits.png");
          this.Text = new Description("12690");
          value = Int32.Parse(element.Value);
          break;

        case "IncidentFireIncreaseUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_incident_fire.png");
          this.Text = new Description("12225");
          factor = 10;
          isPercent = true;
          break;

        case "IncidentExplosionIncreaseUpgrade":
          // this.Icon = new Icon("data/ui/2kimages/main/icons/icon_bomb.png");
          this.Text = new Description("21489");
          factor = 10;
          isPercent = true;
          break;

        case "GoodConsumptionUpgrade":
          // this.Icon = new Icon("data/ui/2kimages/main/icons/icon_marketplace_2d.png");
          this.Text = new Description("21386");
          this.Additionals = new List<Upgrade>();
          foreach (var item in element.Elements("Item")) {
            this.Additionals.Add(new Upgrade() { Text = new Description(item.Element("ProvidedNeed").Value), Value = (item.Element("AmountInPercent").Value.StartsWith("-") ? "" : "+") + $"{item.Element("AmountInPercent").Value}%" });
          }
          break;

        case "Building":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/ship_info/icon_damage.png");
          this.Text = new Description("17394");
          value = Convert.ToInt32((Decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "SailShip":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/ship_info/icon_damage.png");
          this.Text = new Description("17395");
          value = Convert.ToInt32((Decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "SteamShip":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/ship_info/icon_damage.png");
          this.Text = new Description("17396");
          value = Convert.ToInt32((Decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "LoadingSpeedUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_load_ships.png");
          this.Text = new Description("15197");
          break;

        case "ItemSet":
          var itemSet = Assets.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={element.Value}]");
          //this.Icon = itemSet.Element("IconFilename") == null ? null : new Icon(itemSet.Element("IconFilename").Value);
          this.Text = new Description(element.Value);
          break;

        case "UseProjectile":
          var Projectile = Assets
            .Original
            .Root
            .Descendants("Asset")
            .FirstOrDefault(a => a.XPathSelectElement($"Values/Standard/GUID")?.Value == element.Value);

          var infodesc = Projectile.XPathSelectElement("Values/Standard/InfoDescription")?.Value;
          if (infodesc == null) {
            this.Text = new Description(element.Parent.Parent.XPathSelectElement($"Standard/GUID").Value);
            break;
          }
          var infodescAsset = Assets.Original.Root.Descendants("Asset").FirstOrDefault(a => a.XPathSelectElement($"Values/Standard/GUID")?.Value == infodesc);
          if (infodescAsset != null) {
            this.Text = new Description(infodescAsset.XPathSelectElement("Values/Standard/InfoDescription").Value);
            this.Text.AdditionalInformation = new Description(infodescAsset.XPathSelectElement("Values/Standard/GUID").Value, DescriptionFontStyle.Light);
          }
          break;



        case "ActionDuration":
          this.Text = new Description("2423", DescriptionFontStyle.Light);
          this.Text.DE = "Dauer";
          this.Text.EN = "Duration";
          this.Value = TimeSpan.FromMilliseconds(Convert.ToInt64(element.Value)).ToString("mm':'ss");
          return;

        case "ActionCooldown":
          this.Text = new Description("2424", DescriptionFontStyle.Light);
          this.Text.DE = "Aufladung";
          this.Text.EN = "Cooldown";
          this.Value = TimeSpan.FromMilliseconds(Convert.ToInt64(element.Value)).ToString("mm':'ss");
          return;

        case "IsDestroyedAfterCooldown":
          this.Text = new Description("2421", DescriptionFontStyle.Light);
          this.Text.DE = "Wird nach Gebrauch zerstört";
          this.Text.EN = "Destroyed after use";
          break;

        case "LineOfSightRangeUpgrade":
          this.Text = new Description("15266");
          break;

        case "BaseDamageUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/ship_info/icon_damage.png");
          this.Text = new Description("2334");
          if (value == null) {
            value = 0;
          }
          break;

        case "AccuracyUpgrade":
          //this.Icon = new Icon("data/ui/2kimages/main/icons/icon_diplomacy_options_support_fleet.png");
          this.Text = new Description("12062");
          break;

        default:
          throw new NotImplementedException(element.Name.LocalName);
      }
      if (value == null) {
        this.Value = String.Empty;
      }
      else {
        if (isPercent) {
          this.Value = value > 0 ? $"+{value * factor}%" : $"{value * factor}%";
        }
        else {
          this.Value = value > 0 ? $"+{value * factor}" : $"{value * factor}";
        }
      }
    }
    public Upgrade(String key, String amount) {
      var value = amount == null ? null : (Int32?)Int32.Parse(amount);
      this.Text = new Description(Assets.GetDescriptionID(key));
      switch (key) {
        case "PerkFormerPirate":
        case "PerkDiver":
        case "PerkZoologist":
          value = null;
          this.Text = Text.InsertBefore("Trait: ", "Merkmal: ");
          break;

        case "PerkMilitaryShip":
        case "PerkHypnotist":
        case "PerkAnthropologist":
        case "PerkPolyglot":
        case "PerkArcheologist":
        case "PerkMale":
        case "PerkFemale":
          value = null;
          this.Text = Text.InsertBefore("Trait: ", "Merkmal: ");
          break;
        default:
          break;
      }
      if (value == null) {
        this.Value = String.Empty;
      }
      else {
        this.Value = value.ToString();
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement(this.GetType().Name);
      if (this.Text != null)
        result.Add(this.Text.ToXml("Text"));
      if (this.Value != null)
        result.Add(new XElement("Value", this.Value));
      if (this.AdditionalOutputs != null)
        result.Add(new XElement("AdditionalOutputs", this.AdditionalOutputs.Select(s => s.ToXml())));
      if (this.ReplaceInputs != null)
        result.Add(new XElement("ReplaceInputs", this.ReplaceInputs.Select(s => s.ToXml())));
      if (this.InputAmountUpgrades != null)
        result.Add(new XElement("InputAmountUpgrades", this.InputAmountUpgrades.Select(s => s.ToXml())));
      if (this.ReplacingWorkforce != null)
        result.Add(new XElement("ReplacingWorkforce", this.ReplacingWorkforce.ToXml()));
      if (this.Additionals != null)
        result.Add(new XElement("Additionals", this.Additionals.Select(s => s.ToXml())));
      return result;
    }

    #endregion Methods
  }
}