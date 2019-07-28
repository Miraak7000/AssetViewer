using AssetViewer.Data;
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

    public static Dictionary<Int32, Data.Description> Descriptions { get; } = new Dictionary<int, Data.Description> {
      [101] = new Description("Asset:", "Gegenstand:"),
      [102] = new Description("Improvements", "Verbesserungen"),
      [103] = new Description("Harbor office items", "Hafenmeisterei Gegenstände"),
      [104] = new Description("Town hall items", "Rathaus Gegenstände"),
      [105] = new Description("World Fair", "Weltausstellung"),
      [106] = new Description("Equipped in", "Ausgerüstet in"),
      [107] = new Description("Topic", "Thema"),
      [108] = new Description("Size", "Grösse"),
      [109] = new Description("Level", "Stufe"),
      [110] = new Description("Third Party", "Gegenspieler"),
      [120] = new Description("Expedition Events", "Expedition Events"),
      [121] = new Description("Tourism", "Tourismus"),
      [1001] = new Description("Rarity", "Rarität"),
      [1002] = new Description("Type", "Typ"),
      [1003] = new Description("Affect Building Group", "Beeinflusst Gebäude Gruppe"),
      [1004] = new Description("Search", "Suchen"),
      [1005] = new Description("Source", "Quelle"),
      [1006] = new Description("Attribute", "Eigenschaft"),
      [1007] = new Description("Patch Version", "Patch Version"),
      [1008] = new Description("Detailed Sources", "Detaillierte Quellen"),
      [1011] = new Description("Has Factory Upgrades", "Hat Fabrik-Erweiterungen"),
      [1012] = new Description("Has Building Upgrades", "Hat Gebäude-Erweiterungen"),
      [1021] = new Description("Common", "Allgemein"),
      [1022] = new Description("Specialist", "Spezialist"),
      [1023] = new Description("Rarity", "Seltenheit"),
      [1024] = new Description("Effect Targets:", "Beeinflusste Gebäude:"),
      [1046] = new Description("Has Population Upgrade", "Hat Bevölkerung-Erweiterungen"),
      [1047] = new Description("Has Residence Upgrade", "Hat Einwohne-Erweiterungen"),
      [1048] = new Description("Progression", "Fortschritt"),
      [1049] = new Description("Player", "Spieler"),
      [1100] = new Description("Reset Filters", "Filter zurücksetzen"),
      [1101] = new Description("Only available items ", "Nur verfügbare Items "),
      [1102] = new Description("Affect Building", "Beeinflusst Gebäude"),
      [1103] = new Description("Sort by", "Sortieren nach"),
      [1104] = new Description("Advanced Filters", "Erweiterte Filter")
    };

    public static Dictionary<String, Description> Translations { get; } = new Dictionary<string, Description> {
      {"IncidentInfectableUpgrade", new Description("Infectable", "Infizierungen")},
      {"IncidentRiotIncreaseUpgrade", new Description("Riot", "Aufstand")},
      {"IncidentFireIncreaseUpgrade", new Description("Fire", "Feuer")},
      {"IncidentExplosionIncreaseUpgrade", new Description("Explosion", "Explosion")},
      {"IncidentIllnessIncreaseUpgrade", new Description("Illness", "Krankheit")},
      {"ElectricUpgrade", new Description("Electric", "Elektrizität")},
      {"ProvideElectricity", new Description("Provide Electricity", "Liefert Elektrizität")},
      {"BuildingUpgrade", new Description("Building", "Gebäude")},
      {"ReplacingWorkforce", new Description("Replacing Workforce", "Ersetzt Arbeitskraft")},
      {"MaintenanceUpgrade", new Description("Maintenance", "Wartung")},
      {"WorkforceAmountUpgrade", new Description("Workforce Amount", "Arbeitskraft")},
      {"ResolverUnitCountUpgrade", new Description("Resolver Unit Count", "Einheitenanzahl")},
      {"PublicServiceFullSatisfactionDistance", new Description("Public Service Full Satisfaction Distance", "Public Service Full Satisfaction Distance")},
      {"PublicServiceNoSatisfactionDistance", new Description("Public Service No Satisfaction Distance", "Public Service No Satisfaction Distance")},
      {"ResolverUnitMovementSpeedUpgrade", new Description("Resolver Unit Movement Speed", "Einheitenschnelligkeit")},
      {"AdditionalOutput", new Description("Additional Output", "Zusatzprodukte")},
      {"Cycle", new Description("Cycle", "Zyklus")},
      {"Amount", new Description("Amount", "Anzahl")},
      {"InputAmountUpgrade", new Description("Input Amount", "Eingabeanzahl")},
      {"NeededAreaPercentUpgrade", new Description("Needed Area", "Benötigte Fläche")},
      {"OutputAmountFactorUpgrade", new Description("Output Amount", "Ausgabebetrag")},
      {"ProductivityUpgrade", new Description("Productivity", "Produktivität")},
      {"AddedFertility", new Description("Added Fertility", "Fruchtbarkeit")},
      {"NeedsElectricity", new Description("Needs Electricity", "Benötigt Energie")},
      {"ReplaceInputs", new Description("Replace Inputs", "Ersetzt Eingabematerialien")},
      {"Old", new Description("Old", "Alt")},
      {"New", new Description("New", "Neu")},
      {"FactoryUpgrade", new Description("Factory", "Fabrik")},
      {"CultureUpgrade", new Description("Culture", "Kultur")},
      {"AttractivenessUpgrade", new Description("Attractiveness", "Attraktivität")},
      {"ModuleOwnerUpgrade", new Description("Module Owner", "Module")},
      {"ModuleLimitUpgrade", new Description("Module Limit", "Modullimit")},
      {"IncidentInfluencerUpgrade", new Description("Influencer", "Einflüsse")},
      {"SpecialUnitHappinessThresholdUpgrade", new Description("Happiness Threshold", "Glücksschwelle")},
      {"FireInfluenceUpgrade", new Description("Fire Influence", "Feuereinfluss")},
      {"IllnessInfluenceUpgrade", new Description("Illness Influence", "Krankheitseinfluss")},
      {"RiotInfluenceUpgrade", new Description("Riot Influence", "Aufstand Einfluss")},
      {"DistanceUpgrade", new Description("Distance", "Distanz")},
      {"ResidenceUpgrade", new Description("Residence", "Wohnsitz")},
      {"AdditionalHappiness", new Description("Additional Happiness", "Zusätzliche Zufriedenheit")},
      {"PopulationUpgrade", new Description("Population", "Bevölkerung")},
      {"ResidentsUpgrade", new Description("Residents", "Einwohner")},
      {"StressUpgrade", new Description("Stress", "Stress")},
      {"ExpeditionAttribute", new Description("Expedition", "Expedition")},
      {"BaseMorale", new Description("Base Morale", "Basismoral")},
      {"ItemDifficulties", new Description("Difficulties", "Schwierigkeit")},
      {"PerkMale", new Description("Per kMale", "Vorteil männlich")},
      {"PerkFemale", new Description("Perk Female", "Vorteil weiblich")},
      {"PerkFormerPirate", new Description("Perk Former Pirate", "Vorteil Pirat")},
      {"PerkDiver", new Description("Perk Diver", "Vorteil Taucher")},
      {"PerkZoologist", new Description("PerkZoologist", "Vorteil Zoo")},
      {"PerkMilitaryShip", new Description("Perk Military Ship", "Vorteil Militärschiff")},
      {"PerkHypnotist", new Description("Perk Hypnotist", "Vorteil Hypnose")},
      {"PerkAnthropologist", new Description("Perk Anthropologist", "Vorteil Anthropologe")},
      {"PerkPolyglot", new Description("Perk Polyglot", "Vorteil Mehrsprachigkeit")},
      {"PerkSteamShip", new Description("Perk Steam Ship", "Vorteil Dampfschiff")},
      {"PerkSailingShip", new Description("Perk Sailing Ship", "Vorteil Segelschiff")},
      {"Medicine", new Description("Medicine", "Heilkunst")},
      {"Melee", new Description("Force", "Kampfkraft")},
      {"Crafting", new Description("Crafting", "Geschick")},
      {"Hunting", new Description("Hunting", "Jagdglück")},
      {"Navigation", new Description("Navigation", "Navigation")},
      {"Might", new Description("Naval Power", "Gefechtskunde")},
      {"Diplomacy", new Description("Diplomacy", "Redegabe")},
      {"Faith", new Description("Faith", "Glaube")}
    };

    #endregion Properties

    #region Fields

    public static Library.Languages Language = Library.Languages.English;

    #endregion Fields

    #region Constructors

    public App() {
      if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName == "DEU") {
        Language = Library.Languages.German;
      }
    }

    #endregion Constructors

    #region Methods

    public static String GetTranslation(String key) {
      return Translations[key].CurrentLang;
    }
    public static String GetTranslation(XElement element) {
      var result = String.Empty;
      switch (App.Language) {
        case Library.Languages.German:
          return element.Element("DE").Element("Short").Value;
      }
      if (result == String.Empty)
        result = element.Element("EN").Element("Short").Value;
      return result;
    }

    #endregion Methods
  }
}