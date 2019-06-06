using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using AssetViewer.Library;

namespace AssetViewer {

  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public partial class App : Application {

    #region Properties
    public static Dictionary<Int32, Description> Descriptions {
      get {
        var result = new Dictionary<Int32, Description>();
        result.Add(101, new Description("Asset:", "Gegenstand:"));
        result.Add(102, new Description("Improvements", "Verbesserungen"));
        result.Add(103, new Description("Harbor office items", "Hafenmeisterei Gegenstände"));
        result.Add(104, new Description("Town hall items", "Rathaus Gegenstände"));
        result.Add(105, new Description("World Fair", "Weltausstellung"));
        result.Add(106, new Description("Equipped", "Ausgerüstet"));
        result.Add(107, new Description("Topic", "Thema"));
        result.Add(108, new Description("Size", "Grösse"));
        result.Add(109, new Description("Level", "Stufe"));
        result.Add(110, new Description("Third Party", "Gegenspieler"));
        result.Add(120, new Description("Expedition Events", "Expedition Events"));
        result.Add(1001, new Description("Rarity", "Rarität"));
        result.Add(1002, new Description("Type", "Typ"));
        result.Add(1003, new Description("Allocation", "Zuweisung"));
        result.Add(1004, new Description("Search", "Suchen"));
        result.Add(1005, new Description("Source", "Quelle"));
        result.Add(1006, new Description("Effect", "Effekt"));
        result.Add(1007, new Description("Patch Version", "Patch Version"));
        result.Add(1011, new Description("Has Factory Upgrades", "Hat Fabrik-Erweiterungen"));
        result.Add(1012, new Description("Has Building Upgrades", "Hat Gebäude-Erweiterungen"));
        result.Add(1021, new Description("Common", "Allgemein"));
        result.Add(1022, new Description("Specialist", "Spezialist"));
        result.Add(1023, new Description("Rarity:", "Seltenheit:"));
        result.Add(1024, new Description("Effect Targets:", "Beeinflusste Gebäude:"));
        result.Add(1046, new Description("Has Population Upgrade", "Hat Bevölkerung-Erweiterungen"));
        result.Add(1047, new Description("Has Residence Upgrade", "Hat Einwohne-Erweiterungen"));
        result.Add(1048, new Description("Progression", "Fortschritt"));
        result.Add(1049, new Description("Player", "Spieler"));
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
        result.Add("Medicine", new Tuple<String, String>("Medicine", "Medizin"));
        result.Add("Melee", new Tuple<String, String>("Melee", "Nahkampf"));
        result.Add("Crafting", new Tuple<String, String>("Crafting", "Reparieren"));
        result.Add("PerkFemale", new Tuple<String, String>("Perk Female", "Vorteil weiblich"));
        result.Add("Hunting", new Tuple<String, String>("Hunting", "Jagen"));
        result.Add("PerkFormerPirate", new Tuple<String, String>("Perk Former Pirate", "Vorteil Pirat"));
        result.Add("PerkDiver", new Tuple<String, String>("Perk Diver", "Vorteil Taucher"));
        result.Add("Navigation", new Tuple<String, String>("Navigation", "Navigation"));
        result.Add("Might", new Tuple<String, String>("Might", "Können"));
        result.Add("PerkZoologist", new Tuple<String, String>("PerkZoologist", "Vorteil Zoo"));
        result.Add("Diplomacy", new Tuple<String, String>("Diplomacy", "Diplomatie"));
        result.Add("Faith", new Tuple<String, String>("Faith", "Glaube"));
        result.Add("PerkMilitaryShip", new Tuple<String, String>("Perk Military Ship", "Vorteil Militärschiff"));
        result.Add("PerkHypnotist", new Tuple<String, String>("Perk Hypnotist", "Vorteil Hypnose"));
        result.Add("PerkAnthropologist", new Tuple<String, String>("Perk Anthropologist", "Vorteil Anthropologe"));
        result.Add("PerkPolyglot", new Tuple<String, String>("Perk Polyglot", "Vorteil Mehrsprachigkeit"));
        return result;
      }
    }
    #endregion

    #region Fields
    public static Languages Language = Languages.English;
    #endregion

    #region Public Methods
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
      if (result == String.Empty) result = element.Element("EN").Element("Short").Value;
      return result;
    }
    #endregion

  }

}