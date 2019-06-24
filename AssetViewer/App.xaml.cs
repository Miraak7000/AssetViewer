using AssetViewer.Data;
using AssetViewer.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;

namespace AssetViewer {

  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public partial class App : Application {

    #region Properties

    public static Dictionary<Int32, Data.Description> Descriptions {
      get {
        var result = new Dictionary<Int32, Data.Description>();
        result.Add(101, new Data.Description("Asset:", "Gegenstand:"));
        result.Add(102, new Data.Description("Improvements", "Verbesserungen"));
        result.Add(103, new Data.Description("Harbor office items", "Hafenmeisterei Gegenstände"));
        result.Add(104, new Data.Description("Town hall items", "Rathaus Gegenstände"));
        result.Add(105, new Data.Description("World Fair", "Weltausstellung"));
        result.Add(106, new Data.Description("Equipped in", "Ausgerüstet in"));
        result.Add(107, new Data.Description("Topic", "Thema"));
        result.Add(108, new Data.Description("Size", "Grösse"));
        result.Add(109, new Data.Description("Level", "Stufe"));
        result.Add(110, new Data.Description("Third Party", "Gegenspieler"));
        result.Add(120, new Data.Description("Expedition Events", "Expedition Events"));
        result.Add(121, new Data.Description("Tourism", "Tourismus"));
        result.Add(1001, new Data.Description("Rarity", "Rarität"));
        result.Add(1002, new Data.Description("Type", "Typ"));
        result.Add(1003, new Data.Description("Affect Building Group", "Beeinflusst Gebäude Gruppe"));
        result.Add(1004, new Data.Description("Search", "Suchen"));
        result.Add(1005, new Data.Description("Source", "Quelle"));
        result.Add(1006, new Data.Description("Attribute", "Eigenschaft"));
        result.Add(1007, new Data.Description("Patch Version", "Patch Version"));
        result.Add(1008, new Data.Description("Detailed Sources", "Detaillierte Quellen"));
        result.Add(1011, new Data.Description("Has Factory Upgrades", "Hat Fabrik-Erweiterungen"));
        result.Add(1012, new Data.Description("Has Building Upgrades", "Hat Gebäude-Erweiterungen"));
        result.Add(1021, new Data.Description("Common", "Allgemein"));
        result.Add(1022, new Data.Description("Specialist", "Spezialist"));
        result.Add(1023, new Data.Description("Rarity", "Seltenheit"));
        result.Add(1024, new Data.Description("Effect Targets:", "Beeinflusste Gebäude:"));
        result.Add(1046, new Data.Description("Has Population Upgrade", "Hat Bevölkerung-Erweiterungen"));
        result.Add(1047, new Data.Description("Has Residence Upgrade", "Hat Einwohne-Erweiterungen"));
        result.Add(1048, new Data.Description("Progression", "Fortschritt"));
        result.Add(1049, new Data.Description("Player", "Spieler"));
        result.Add(1100, new Data.Description("Reset Filters", "Filter zurücksetzen"));
        result.Add(1101, new Data.Description("Only available items ", "Nur verfügbare Items "));
        result.Add(1102, new Data.Description("Affect Building", "Beeinflusst Gebäude"));
        result.Add(1103, new Data.Description("Sort by", "Sortieren nach"));
        return result;
      }
    }

    public static Dictionary<String, Tuple<String, String>> Translations {
      get {
        var result = new Dictionary<String, Tuple<String, String>>();
        result.Add("IncidentInfectableUpgrade", new Tuple<String, String>("Infectable", "Infizierungen"));
        result.Add("IncidentRiotIncreaseUpgrade", new Tuple<String, String>("Riot", "Aufstand"));
        result.Add("IncidentFireIncreaseUpgrade", new Tuple<String, String>("Fire", "Feuer"));
        result.Add("IncidentExplosionIncreaseUpgrade", new Tuple<String, String>("Explosion", "Explosion"));
        result.Add("IncidentIllnessIncreaseUpgrade", new Tuple<String, String>("Illness", "Krankheit"));
        result.Add("ElectricUpgrade", new Tuple<String, String>("Electric", "Elektrizität"));
        result.Add("ProvideElectricity", new Tuple<String, String>("Provide Electricity", "Liefert Elektrizität"));
        result.Add("BuildingUpgrade", new Tuple<String, String>("Building", "Gebäude"));
        result.Add("ReplacingWorkforce", new Tuple<String, String>("Replacing Workforce", "Ersetzt Arbeitskraft"));
        result.Add("MaintenanceUpgrade", new Tuple<String, String>("Maintenance", "Wartung"));
        result.Add("WorkforceAmountUpgrade", new Tuple<String, String>("Workforce Amount", "Arbeitskraft"));
        result.Add("ResolverUnitCountUpgrade", new Tuple<String, String>("Resolver Unit Count", "Einheitenanzahl"));
        result.Add("PublicServiceFullSatisfactionDistance", new Tuple<String, String>("Public Service Full Satisfaction Distance", "Public Service Full Satisfaction Distance"));
        result.Add("PublicServiceNoSatisfactionDistance", new Tuple<String, String>("Public Service No Satisfaction Distance", "Public Service No Satisfaction Distance"));
        result.Add("ResolverUnitMovementSpeedUpgrade", new Tuple<String, String>("Resolver Unit Movement Speed", "Einheitenschnelligkeit"));
        result.Add("AdditionalOutput", new Tuple<String, String>("Additional Output", "Zusatzprodukte"));
        result.Add("Cycle", new Tuple<String, String>("Cycle", "Zyklus"));
        result.Add("Amount", new Tuple<String, String>("Amount", "Anzahl"));
        result.Add("InputAmountUpgrade", new Tuple<String, String>("Input Amount", "Eingabeanzahl"));
        result.Add("NeededAreaPercentUpgrade", new Tuple<String, String>("Needed Area", "Benötigte Fläche"));
        result.Add("OutputAmountFactorUpgrade", new Tuple<String, String>("Output Amount", "Ausgabebetrag"));
        result.Add("ProductivityUpgrade", new Tuple<String, String>("Productivity", "Produktivität"));
        result.Add("AddedFertility", new Tuple<String, String>("Added Fertility", "Fruchtbarkeit"));
        result.Add("NeedsElectricity", new Tuple<String, String>("Needs Electricity", "Benötigt Energie"));
        result.Add("ReplaceInputs", new Tuple<String, String>("Replace Inputs", "Ersetzt Eingabematerialien"));
        result.Add("Old", new Tuple<String, String>("Old", "Alt"));
        result.Add("New", new Tuple<String, String>("New", "Neu"));
        result.Add("FactoryUpgrade", new Tuple<String, String>("Factory", "Fabrik"));
        result.Add("CultureUpgrade", new Tuple<String, String>("Culture", "Kultur"));
        result.Add("AttractivenessUpgrade", new Tuple<String, String>("Attractiveness", "Attraktivität"));
        result.Add("ModuleOwnerUpgrade", new Tuple<String, String>("Module Owner", "Module"));
        result.Add("ModuleLimitUpgrade", new Tuple<String, String>("Module Limit", "Modullimit"));
        result.Add("IncidentInfluencerUpgrade", new Tuple<String, String>("Influencer", "Einflüsse"));
        result.Add("SpecialUnitHappinessThresholdUpgrade", new Tuple<String, String>("Happiness Threshold", "Glücksschwelle"));
        result.Add("FireInfluenceUpgrade", new Tuple<String, String>("Fire Influence", "Feuereinfluss"));
        result.Add("IllnessInfluenceUpgrade", new Tuple<String, String>("Illness Influence", "Krankheitseinfluss"));
        result.Add("RiotInfluenceUpgrade", new Tuple<String, String>("Riot Influence", "Aufstand Einfluss"));
        result.Add("DistanceUpgrade", new Tuple<String, String>("Distance", "Distanz"));
        result.Add("ResidenceUpgrade", new Tuple<String, String>("Residence", "Wohnsitz"));
        result.Add("AdditionalHappiness", new Tuple<String, String>("Additional Happiness", "Zusätzliche Zufriedenheit"));
        result.Add("PopulationUpgrade", new Tuple<String, String>("Population", "Bevölkerung"));
        result.Add("ResidentsUpgrade", new Tuple<String, String>("Residents", "Einwohner"));
        result.Add("StressUpgrade", new Tuple<String, String>("Stress", "Stress"));
        result.Add("ExpeditionAttribute", new Tuple<String, String>("Expedition", "Expedition"));
        result.Add("BaseMorale", new Tuple<String, String>("Base Morale", "Basismoral"));
        result.Add("ItemDifficulties", new Tuple<String, String>("Difficulties", "Schwierigkeit"));
        result.Add("PerkMale", new Tuple<String, String>("Per kMale", "Vorteil männlich"));
        result.Add("PerkFemale", new Tuple<String, String>("Perk Female", "Vorteil weiblich"));
        result.Add("PerkFormerPirate", new Tuple<String, String>("Perk Former Pirate", "Vorteil Pirat"));
        result.Add("PerkDiver", new Tuple<String, String>("Perk Diver", "Vorteil Taucher"));
        result.Add("PerkZoologist", new Tuple<String, String>("PerkZoologist", "Vorteil Zoo"));
        result.Add("PerkMilitaryShip", new Tuple<String, String>("Perk Military Ship", "Vorteil Militärschiff"));
        result.Add("PerkHypnotist", new Tuple<String, String>("Perk Hypnotist", "Vorteil Hypnose"));
        result.Add("PerkAnthropologist", new Tuple<String, String>("Perk Anthropologist", "Vorteil Anthropologe"));
        result.Add("PerkPolyglot", new Tuple<String, String>("Perk Polyglot", "Vorteil Mehrsprachigkeit"));
        result.Add("PerkSteamShip", new Tuple<String, String>("Perk Steam Ship", "Vorteil Dampfschiff"));
        result.Add("PerkSailingShip", new Tuple<String, String>("Perk Sailing Ship", "Vorteil Segelschiff"));
        result.Add("Medicine", new Tuple<String, String>("Medicine", "Heilkunst"));
        result.Add("Melee", new Tuple<String, String>("Force", "Kampfkraft"));
        result.Add("Crafting", new Tuple<String, String>("Crafting", "Geschick"));
        result.Add("Hunting", new Tuple<String, String>("Hunting", "Jagdglück"));
        result.Add("Navigation", new Tuple<String, String>("Navigation", "Navigation"));
        result.Add("Might", new Tuple<String, String>("Naval Power", "Gefechtskunde"));
        result.Add("Diplomacy", new Tuple<String, String>("Diplomacy", "Redegabe"));
        result.Add("Faith", new Tuple<String, String>("Faith", "Glaube"));
        return result;
      }
    }

    #endregion Properties

    #region Fields

    public static Languages Language = Languages.English;

    #endregion Fields

    #region Constructors

    public App() {
      if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName == "DEU") {
        Language = Languages.German;
      }
    }

    #endregion Constructors

    #region Methods

    public static String GetTranslation(String key) {
      switch (App.Language) {
        case Languages.German:
          return App.Translations[key].Item2;

        default:
          return App.Translations[key].Item1;
      }
    }
    public static String GetTranslation(XElement element) {
      var result = String.Empty;
      switch (App.Language) {
        case Languages.German:
          return element.Element("DE").Element("Short").Value;
      }
      if (result == String.Empty)
        result = element.Element("EN").Element("Short").Value;
      return result;
    }

    #endregion Methods
  }
}