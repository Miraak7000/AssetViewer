using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class Upgrade {
    private Description text;

    #region Public Properties

    public Description Text { get => text; set => text = value; }
    public string Value { get; set; }
    public string Category { get; set; }
    public List<AdditionalOutput> AdditionalOutputs { get; set; }
    public List<ReplaceInput> ReplaceInputs { get; set; }
    public List<InputAmountUpgrade> InputAmountUpgrades { get; set; }
    public ReplacingWorkforce ReplacingWorkforce { get; set; }
    public List<Upgrade> Additionals { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Upgrade() {
    }

    public Upgrade(XElement element) {
      var isPercent = element.Element("Percental")?.Value == "1";
      float? value = element.Element("Value") == null ? null : (float?)float.Parse(element.Element("Value").Value);
      var factor = 1;
      if (Assets.TryGetDescriptionID(element.Name.LocalName, out var text)) {
        Text = new Description(text);
      }

      switch (element.Name.LocalName) {
        case "Action":
          switch (element.Element("Template").Value) {
            case "ActionStartTreasureMapQuest":
              Additionals = new List<Upgrade>();
              Text = new Description("2734").AppendWithSpace("-").AppendWithSpace(new Description(element.XPathSelectElement("Values/ActionStartTreasureMapQuest/TreasureSessionOrRegion").Value));
              Additionals.Add(new Upgrade { Text = new Description(element.XPathSelectElement("Values/ActionStartTreasureMapQuest/TreasureMapQuest").Value) });
              break;

            default:
              break;
          }
          break;

        case "PassiveTradeGoodGenUpgrade":
          Text.AdditionalInformation = new Description("20327", DescriptionFontStyle.Light);
          var genpool = element.Element("GenPool").Value;
          var items = Assets.GUIDs[genpool]?
            .XPathSelectElement("Values/RewardPool/ItemsPool")
            .Elements("Item")
            .Select(i => new Description(i.Element("ItemLink").Value));
          Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) GoodGenerationPoolFormatted]", items, (s) => string.Join(", ", s));
          value = Convert.ToSingle(element.Element("GenProbability").Value);
          isPercent = true;
          break;

        case "AddAssemblyOptions":
          Text.AdditionalInformation = new Description("20325", DescriptionFontStyle.Light);
          var descs = element.Elements("Item").Select(i => new Description(i.Element("NewOption").Value));
          Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) AddAssemblyOptionsFormatted]", descs, (s) => string.Join(", ", s));
          break;

        case "AssemblyOptions":
          Text.AdditionalInformation = new Description("20325", DescriptionFontStyle.Light);
          descs = element.Descendants("Vehicle").Select(i => new Description(i.Value));
          Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) AddAssemblyOptionsFormatted]", descs, (s) => string.Join(", ", s));
          break;

        case "MoraleDamage":
          Text.AdditionalInformation = new Description("21586", DescriptionFontStyle.Light);
          break;

        case "HitpointDamage":
          switch (element.Parent.Parent.Element("Item")?.Element("Allocation").Value ?? "Ship") {
            case "Ship":
            case "SailShip":
            case "Warship":
            case "SteamShip":
              Text.AdditionalInformation = new Description("21585", DescriptionFontStyle.Light);
              break;

            default:
              Text.AdditionalInformation = new Description("21589", DescriptionFontStyle.Light);
              break;
          }
          break;

        case "SpecialUnitHappinessThresholdUpgrade":
          Text.AdditionalInformation = new Description("21584", DescriptionFontStyle.Light);
          var target = element.Parent.Parent.Element("ItemEffect").Element("EffectTargets").Elements().FirstOrDefault()?.Element("GUID").Value;
          Description unit = null;
          switch (target) {
            case "190777": //Hospital
              unit = new Description("100584");
              break;

            case "190776": //Police Station
              unit = new Description("100582");
              break;

            case "190775": //Fire Station
              unit = new Description("100580");
              break;
            //case "112669": //Polar Station
            //  unit = new Description("114896");
            //  break;

            default:
              throw new NotImplementedException(target);
          }
          Text.AdditionalInformation.Replace("[AssetData([ToolOneHelper IncidentResolverUnitsForTargetBuildings([RefGuid], 1) AT(0)]) Text]", unit);
          break;

        case "ItemSet":
        case "ProvidedNeed":
          Text = new Description(element.Value);
          break;

        case "HappinessIgnoresMorale":
          Text.AdditionalInformation = new Description("20326", DescriptionFontStyle.Light);
          break;

        case "ChangedSupplyValueUpgrade":
          Text = new Description("12649");
          Additionals = new List<Upgrade>();
          foreach (var item in element.Elements("Item")) {
            Additionals.Add(new Upgrade { Text = new Description(item.Element("Need").Value), Value = (item.Element("AmountInPercent").Value.StartsWith("-") ? "" : "+") + $"{item.Element("AmountInPercent").Value}%" });
          }
          break;

        case "ResolverUnitDecreaseUpgrade":
          target = element.Parent.Parent.Element("ItemEffect").Element("EffectTargets").Elements().FirstOrDefault()?.Element("GUID").Value;
          switch (target) {
            case "190777": //Hospital
              Text = new Description("12012");
              break;

            case "190776": //Police Station
              Text = new Description("21509");
              break;

            case "112669": //Polar Station
              Text = new Description("22983");
              break;

            case "190775": //Fire Station
            case "1010463": //Fire Department
              Text = new Description("21508");
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
            .FirstOrDefault()?
            .Element("GUID")
            .Value;

          switch (target) {
            case "190777": //Hospital
              Text = new Description("100583");
              break;

            case "190776": //Police Station
              Text = new Description("100581");
              break;

            case "112669": //Polar Station
              Text = new Description("114895");
              break;

            case "190775": //Fire Station
            case "1010463": //Fire Department
              Text = new Description("100579");
              break;

            default:
              throw new NotImplementedException(target);
          }
          break;

        case "AdditionalOutput":
          AdditionalOutputs = new List<AdditionalOutput>();
          foreach (var item in element.Elements()) {
            AdditionalOutputs.Add(new AdditionalOutput(item));
          }
          break;

        case "ReplaceInputs":
          ReplaceInputs = new List<ReplaceInput>();
          foreach (var item in element.Elements()) {
            ReplaceInputs.Add(new ReplaceInput(item));
          }
          break;

        case "InputAmountUpgrade":
          InputAmountUpgrades = new List<InputAmountUpgrade>();
          foreach (var item in element.Elements()) {
            InputAmountUpgrades.Add(new InputAmountUpgrade(item));
          }
          break;

        case "AddedFertility":
          Text = new Description("21371").Replace("[AssetData([ItemAssetData([RefGuid]) AddedFertility]) Text]", new Description(element.Value));

          break;

        case "ActiveTradePriceInPercent":
          if (value == null && !element.HasElements) {
            value = Single.Parse(element.Value);
            if (value < 100) {
              value = -(100 - value);
            }
            else {
              value -= 100;
            }
          }
          isPercent = true;
          break;

        case "ActivateWhiteFlag":
          Text.Icon = new Icon("data/ui/2kimages/main/icons/icon_claim_island.png");
          Text.AdditionalInformation = new Description("19487", DescriptionFontStyle.Light);
          break;

        case "ActivatePirateFlag":
          Text.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_melee_tint.png");
          Text.AdditionalInformation = new Description("17393", DescriptionFontStyle.Light);
          break;

        case "AttackSpeedUpgrade":
          if (value == null) {
            value = element.Value == null ? null : (int?)Single.Parse(element.Value);
          }
          isPercent = true;
          break;

        case "SelfHealPausedTimeIfAttackedUpgrade":
          Text.AdditionalInformation = new Description("21590", DescriptionFontStyle.Light);
          value = value == -100 ? null : value;
          break;

        case "NeedProvideNeedUpgrade":
          var SubstituteNeeds = element.Descendants("SubstituteNeed").Select(i => new Description(i.Value));
          var ProvidedNeeds = element.Descendants("ProvidedNeed").Select(i => new Description(i.Value));
          Text.AdditionalInformation = new Description("20323", DescriptionFontStyle.Light);
          Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) AllSubstituteNeedsFormatted]", SubstituteNeeds, s => string.Join(", ", s.Distinct()));
          Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) AllProvidedNeedsFormatted]", ProvidedNeeds, s => string.Join(", ", s.Distinct()));
          break;

        case "GoodConsumptionUpgrade":
          Additionals = new List<Upgrade>();
          foreach (var item in element.Elements("Item").Where(e => e.HasElements)) {
            Additionals.Add(new Upgrade { Text = new Description(item.Element("ProvidedNeed").Value), Value = ((item.Element("AmountInPercent")?.Value?.StartsWith("-") ?? false) ? "" : "+") + $"{item.Element("AmountInPercent")?.Value ?? "100"}%" });
          }
          break;

        case "UseProjectile":
          var Projectile = Assets.GUIDs[element.Value];

          var infodesc = Projectile.XPathSelectElement("Values/Standard/InfoDescription")?.Value;
          if (infodesc == null) {
            Text = new Description(element.Parent.Parent.XPathSelectElement("Standard/GUID").Value);
            break;
          }
          var infodescAsset = Assets.GUIDs[infodesc];
          if (infodescAsset != null) {
            Text = new Description(infodescAsset.XPathSelectElement("Values/Standard/InfoDescription").Value) {
              AdditionalInformation = new Description(infodescAsset, DescriptionFontStyle.Light)
            };
          }
          break;

        case "ActionDuration":
          Text.FontStyle = DescriptionFontStyle.Light;
          Text.Languages = new Description("3898").Languages;
          Value = TimeSpan.FromMilliseconds(Convert.ToInt64(element.Value)).ToString("hh':'mm':'ss");
          while (Value.StartsWith("00:00:")) {
            Value = Value.Remove(0, 3);
          }
          return;

        case "ActionCooldown":
          Text.FontStyle = DescriptionFontStyle.Light;
          Text.Languages = new Description("3899").Languages;
          Value = TimeSpan.FromMilliseconds(Convert.ToInt64(element.Value)).ToString("hh':'mm':'ss");
          while (Value.StartsWith("00:00:")) {
            Value = Value.Remove(0, 3);
          }
          return;

        case "IsDestroyedAfterCooldown":
          Text.FontStyle = DescriptionFontStyle.Light;
          Text.Languages = new Description("2421").Remove("&lt;font color='0xff817f87'&gt;").Remove("&lt;/font&gt;").Languages;
          break;

        case "Building":
          Text = new Description("17394");
          value = Convert.ToSingle((decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "SailShip":
          Text = new Description("17395");
          value = Convert.ToSingle((decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "SteamShip":
          Text = new Description("17396");
          value = Convert.ToSingle((decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "ReplacingWorkforce":
          ReplacingWorkforce = new ReplacingWorkforce(element.Value);
          break;

        case "BaseDamageUpgrade":
          value = value ?? 0;
          break;

        case "IncidentIllnessIncreaseUpgrade":
        case "IncidentArcticIllnessIncreaseUpgrade":
        case "IncidentFireIncreaseUpgrade":
        case "IncidentExplosionIncreaseUpgrade":
        case "ScrapAmountLevelUpgrade":
          factor = 10;
          isPercent = true;
          break;

        case "Normal":
        case "Cannon":
        case "BigBertha":
        case "Torpedo":
          value = -Convert.ToSingle(100f - (100f * float.Parse(element.Element("Factor").Value, CultureInfo.InvariantCulture)));
          isPercent = true;
          break;

        case "ModuleLimitPercent":
        case "ConstructionTimeInPercent":
        case "ConstructionCostInPercent":
        case "WorkforceModifierInPercent":
          value = float.Parse(element.Value);
          isPercent = true;
          break;

        case "TaxModifierInPercent":
          var val = float.Parse(element.Value);
          Value = val > 0 ? $"+{val}%" : $"{val}%";
          return;

        case "IgnoreWeightFactorUpgrade":
        case "IgnoreDamageFactorUpgrade":
          value = -value;
          break;

        case "NeededAreaPercentUpgrade":
          isPercent = true;
          break;

        case "ResolverUnitMovementSpeedUpgrade":
          Value = null;
          break;

        case "IncidentRiotIncreaseUpgrade":
          if (element.Element("Percental")?.Value != "1") {
            factor = 10;
          }
          isPercent = true;
          break;

        case "AccuracyUpgrade":
        case "LineOfSightRangeUpgrade":
        case "LoadingSpeedUpgrade":
        case "PublicServiceFullSatisfactionDistance":
        case "HealRadiusUpgrade":
        case "HealPerMinuteUpgrade":
        case "SpawnProbabilityFactor":
        case "SelfHealUpgrade":
        case "AttackRangeUpgrade":
        case "ForwardSpeedUpgrade":
        case "MaxHitpointsUpgrade":
        case "ResidentsUpgrade":
        case "StressUpgrade":
        case "ProvideElectricity":
        case "NeedsElectricity":
        case "AttractivenessUpgrade":
        case "MaintenanceUpgrade":
        case "WorkforceAmountUpgrade":
        case "OutputAmountFactorUpgrade":
        case "ProductivityUpgrade":
        case "BlockBuyShare":
        case "BlockHostileTakeover":
        case "MaintainanceUpgrade":
        case "MoralePowerUpgrade":
        case "PierSpeedUpgrade":
        case "HeatRangeUpgrade":
        case "HasPollution":

        case "MinPickupTimeUpgrade":
        case "MaxPickupTimeUpgrade":
        case "AttractivenessPerSetUpgrade":
        case "SocketCountUpgrade":
        case "ProductivityBoostUpgrade":
        case "ProvideIndustrialization":
        case "ElectricityBoostUpgrade":
        case "PipeCapacityUpgrade":
        case "MaxDynamicFillCapacityUpgrade":
        case "FiniteResourceRegrowFactorUpgrade":
        case "FiniteResourceRegrowIntervalUpgrade":
        case "DeltaValueUpgrade":
        case "MaxWorkerAmountUpgrade":
          break;

        case "AdditionalHappiness":
        case "AdditionalSupply":
        case "AdditionalMoney":
        case "AdditionalHeat":
        case "Attractiveness":
        case "NumOfPiers":
        case "LoadingSpeed":
        case "MinLoadingTime":
        case "AttackRange":
        case "LineOfSightRange":
        case "ReloadTime":
        case "BaseDamage":
        case "HealRadius":
        case "HealPerMinute":
        case "MaxTrainCount":
        case "StorageCapacityModifier":
        case "AdditionalResearch":
          value = float.Parse(element.Value);
          break;

        case "OverrideSpecialistPool":
          Text.AdditionalInformation = new Description("269571", DescriptionFontStyle.Light);
          break;

        case "RarityWeightUpgrade":
          Additionals = new List<Upgrade>();
          Text = new Description("22227");
          foreach (var item in element.Elements()) {
            if (item.Name.LocalName == "None") {
              //this.Additionals.Add(new Upgrade() { Text = new Description("None", "None"), Value = $"+{item.Element("AdditionalWeight").Value}" });
            }
            else {
              Additionals.Add(new Upgrade { Text = new Description(Assets.GetDescriptionID(item.Name.LocalName)), Value = $"+{item.Element("AdditionalWeight").Value}" });
            }
          }
          break;

        case "ItemSetUpgrade":
          Additionals = new List<Upgrade>();
          Text = new Description("145011");
          foreach (var item in element.Elements()) {
            Additionals.Add(new Upgrade { Text = new Description(item.Element("ItemSet").Value), Value = $"+{item.Element("AttractivenessUpgradePercent").Value}%" });
          }
          break;

        case "Residence7":
          Text = new Description("22379");
          Additionals = new List<Upgrade> {
            new Upgrade { Text = new Description(element.Element("PopulationLevel7").Value), Value = element.Element("ResidentMax").Value }
          };
          break;

        case "IndustrializationRangeUpgrade":
          Text = new Description("249983").Remove("+[ItemAssetData([ToolOneHelper ForwardedEffectGuidOrSelf([RefGuid])]) AdditionalServiceRange]").Trim();
          break;

        case "MotorizableType":
          Text = new Description(Assets.GetDescriptionID(element.Value));
          break;

        case "EcoSystemUpgrade":
          Additionals = new List<Upgrade>();
          Text = new Description("861");
          foreach (var item in element.Elements()) {
            var va = int.Parse(item.Element("Value").Value) / 100;
            var u = new Upgrade {
              Value = va > 0 ? $"+{va}" : $"{va}",
              Text = new Description(Assets.GetDescriptionID(item.Name.LocalName))
            };
            Additionals.Add(u);
          }
          break;


        case "EcoSystemProvider":
          Additionals = new List<Upgrade>();
          Text = new Description("24225");
          var v = int.Parse((element.Element("DeltaValue") != null ? element.Element("DeltaValue").Value : "0"));
          var upgrade = new Upgrade {
            Value = v > 0 ? $"+{v}" : $"{v}",
            Text = new Description(Assets.GetDescriptionID((element.Element("AffectedQuality") != null ? element.Element("AffectedQuality").Value : "Water")))
          };
          Additionals.Add(upgrade);
          break;

        default:
          Debug.WriteLine(element.Name.LocalName);
          throw new NotImplementedException(element.Name.LocalName);
          break;
      }
      
      if (value == null) {
        Value = string.Empty;
      }
      else {
        if (isPercent) {
          Value = value > 0 ? $"+{value * factor}%" : $"{value * factor}%";
        }
        else {
          Value = value > 0 ? $"+{value * factor}" : $"{value * factor}";
        }
      }
    }

    public Upgrade(string key, string amount) {
      var value = amount == null ? null : (int?)int.Parse(amount);
      Text = new Description(Assets.GetDescriptionID(key));
      switch (key) {
        case "PerkFormerPirate":
        case "PerkDiver":
        case "PerkZoologist":
        case "PerkMilitaryShip":
        case "PerkHypnotist":
        case "PerkAnthropologist":
        case "PerkPolyglot":
        case "PerkArcheologist":
        case "PerkMale":
        case "PerkFemale":
          value = null;
          Text = Text.InsertBefore(new Description("-1"));
          break;

        default:
          break;
      }
      if (value == null) {
        Value = string.Empty;
      }
      else {
        Value = value.ToString();
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("U");
      if (Text != null)
        result.Add(Text.ToXml("T"));
      if (Value != null)
        result.Add(new XAttribute("V", Value));
      if (Category != null)
        result.Add(new XAttribute("C", Category));
      if (AdditionalOutputs != null)
        result.Add(new XElement("AO", AdditionalOutputs.Select(s => s.ToXml())));
      if (ReplaceInputs != null)
        result.Add(new XElement("RI", ReplaceInputs.Select(s => s.ToXml())));
      if (InputAmountUpgrades != null)
        result.Add(new XElement("IAUp", InputAmountUpgrades.Select(s => s.ToXml())));
      if (ReplacingWorkforce != null)
        result.Add(new XElement("RW", ReplacingWorkforce.ToXml()));
      if (Additionals != null)
        result.Add(new XElement("A", Additionals.Select(s => s.ToXml())));
      return result;
    }

    #endregion Public Methods
  }
}