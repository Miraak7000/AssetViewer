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
                var result = new Dictionary<Int32, Description> {
                    { 101, new Description("Asset:", "Gegenstand:") },
                    { 102, new Description("Improvements", "Verbesserungen") },
                    { 103, new Description("Harbor office items", "Hafenmeisterei Gegenstände") },
                    { 104, new Description("Town hall items", "Rathaus Gegenstände") },
                    { 105, new Description("World Fair", "Weltausstellung") },
                    { 106, new Description("Equipped", "Ausgerüstet") },
                    { 107, new Description("Topic", "Thema") },
                    { 108, new Description("Size", "Grösse") },
                    { 109, new Description("Level", "Stufe") },
                    { 110, new Description("Third Party", "Gegenspieler") },
                    { 201, new Description("Expedition Events", "Expedition Events") },
                    { 1001, new Description("Rarity", "Rarität") },
                    { 1002, new Description("Type", "Typ") },
                    { 1003, new Description("Allocation", "Zuweisung") },
                    { 1004, new Description("Search", "Suchen") },
                    { 1009, new Description("Source", "Quelle") },
                    { 1010, new Description("Effect", "Effekt") },
                    { 1011, new Description("Has Factory Upgrades", "Hat Fabrik-Erweiterungen") },
                    { 1012, new Description("Has Building Upgrades", "Hat Gebäude-Erweiterungen") },
                    { 1021, new Description("Common", "Allgemein") },
                    { 1022, new Description("Specialist", "Spezialist") },
                    { 1023, new Description("Rarity:", "Seltenheit:") },
                    { 1024, new Description("Effect Targets:", "Beeinflusste Gebäude:") },
                    { 1046, new Description("Has Population Upgrade", "Hat Bevölkerung-Erweiterungen") },
                    { 1047, new Description("Has Residence Upgrade", "Hat Einwohne-Erweiterungen") },
                    { 1048, new Description("Progression", "Fortschritt") },
                    { 1049, new Description("Player", "Spieler") }
                };
                return result;
            }
        }

        public static Dictionary<String, Tuple<String, String>> Translations {
            get {
                var result = new Dictionary<String, Tuple<String, String>> {
                    { "IncidentInfectableUpgrade", new Tuple<String, String>("Infectable", "Infizierungen") },
                    { "IncidentRiotIncreaseUpgrade", new Tuple<String, String>("Riot", "Aufstand") },
                    { "IncidentFireIncreaseUpgrade", new Tuple<String, String>("Fire", "Feuer") },
                    { "IncidentExplosionIncreaseUpgrade", new Tuple<String, String>("Explosion", "Explosion") },
                    { "IncidentIllnessIncreaseUpgrade", new Tuple<String, String>("Illness", "Krankheit") },
                    { "ElectricUpgrade", new Tuple<String, String>("Electric", "Elektrizität") },
                    { "ProvideElectricity", new Tuple<String, String>("Provide Electricity", "Liefert Elektrizität") },
                    { "BuildingUpgrade", new Tuple<String, String>("Building", "Gebäude") },
                    { "ReplacingWorkforce", new Tuple<String, String>("Replacing Workforce", "Ersetzt Arbeitskraft") },
                    { "MaintenanceUpgrade", new Tuple<String, String>("Maintenance", "Wartung") },
                    { "WorkforceAmountUpgrade", new Tuple<String, String>("Workforce Amount", "Arbeitskraft") },
                    { "ResolverUnitCountUpgrade", new Tuple<String, String>("Resolver Unit Count", "Einheitenanzahl") },
                    { "PublicServiceFullSatisfactionDistance", new Tuple<String, String>("Public Service Full Satisfaction Distance", "Public Service Full Satisfaction Distance") },
                    { "PublicServiceNoSatisfactionDistance", new Tuple<String, String>("Public Service No Satisfaction Distance", "Public Service No Satisfaction Distance") },
                    { "ResolverUnitMovementSpeedUpgrade", new Tuple<String, String>("Resolver Unit Movement Speed", "Einheitenschnelligkeit") },
                    { "AdditionalOutput", new Tuple<String, String>("Additional Output", "Zusatzprodukte") },
                    { "Cycle", new Tuple<String, String>("Cycle", "Zyklus") },
                    { "Amount", new Tuple<String, String>("Amount", "Anzahl") },
                    { "InputAmountUpgrade", new Tuple<String, String>("Input Amount", "Eingabeanzahl") },
                    { "NeededAreaPercentUpgrade", new Tuple<String, String>("Needed Area", "Benötigte Fläche") },
                    { "OutputAmountFactorUpgrade", new Tuple<String, String>("Output Amount", "Ausgabebetrag") },
                    { "ProductivityUpgrade", new Tuple<String, String>("Productivity", "Produktivität") },
                    { "AddedFertility", new Tuple<String, String>("Added Fertility", "Fruchtbarkeit") },
                    { "NeedsElectricity", new Tuple<String, String>("Needs Electricity", "Benötigt Energie") },
                    { "ReplaceInputs", new Tuple<String, String>("Replace Inputs", "Ersetzt Eingabematerialien") },
                    { "Old", new Tuple<String, String>("Old", "Alt") },
                    { "New", new Tuple<String, String>("New", "Neu") },
                    { "FactoryUpgrade", new Tuple<String, String>("Factory", "Fabrik") },
                    { "CultureUpgrade", new Tuple<String, String>("Culture", "Kultur") },
                    { "AttractivenessUpgrade", new Tuple<String, String>("Attractiveness", "Attraktivität") },
                    { "ModuleOwnerUpgrade", new Tuple<String, String>("Module Owner", "Module") },
                    { "ModuleLimitUpgrade", new Tuple<String, String>("Module Limit", "Modullimit") },
                    { "IncidentInfluencerUpgrade", new Tuple<String, String>("Influencer", "Einflüsse") },
                    { "SpecialUnitHappinessThresholdUpgrade", new Tuple<String, String>("Happiness Threshold", "Glücksschwelle") },
                    { "FireInfluenceUpgrade", new Tuple<String, String>("Fire Influence", "Feuereinfluss") },
                    { "IllnessInfluenceUpgrade", new Tuple<String, String>("Illness Influence", "Krankheitseinfluss") },
                    { "RiotInfluenceUpgrade", new Tuple<String, String>("Riot Influence", "Aufstand Einfluss") },
                    { "DistanceUpgrade", new Tuple<String, String>("Distance", "Distanz") },
                    { "ResidenceUpgrade", new Tuple<String, String>("Residence", "Wohnsitz") },
                    { "AdditionalHappiness", new Tuple<String, String>("Additional Happiness", "Zusätzliche Zufriedenheit") },
                    { "PopulationUpgrade", new Tuple<String, String>("Population", "Bevölkerung") },
                    { "ResidentsUpgrade", new Tuple<String, String>("Residents", "Einwohner") },
                    { "StressUpgrade", new Tuple<String, String>("Stress", "Stress") },
                    { "ExpeditionAttribute", new Tuple<String, String>("Expedition", "Expedition") },
                    { "BaseMorale", new Tuple<String, String>("Base Morale", "Basismoral") },
                    { "ItemDifficulties", new Tuple<String, String>("Difficulties", "Schwierigkeit") },
                    { "PerkMale", new Tuple<String, String>("Per kMale", "Vorteil männlich") },
                    { "Medicine", new Tuple<String, String>("Medicine", "Medizin") },
                    { "Melee", new Tuple<String, String>("Melee", "Nahkampf") },
                    { "Crafting", new Tuple<String, String>("Crafting", "Reparieren") },
                    { "PerkFemale", new Tuple<String, String>("Perk Female", "Vorteil weiblich") },
                    { "Hunting", new Tuple<String, String>("Hunting", "Jagen") },
                    { "PerkFormerPirate", new Tuple<String, String>("Perk Former Pirate", "Vorteil Pirat") },
                    { "PerkDiver", new Tuple<String, String>("Perk Diver", "Vorteil Taucher") },
                    { "Navigation", new Tuple<String, String>("Navigation", "Navigation") },
                    { "Might", new Tuple<String, String>("Might", "Können") },
                    { "PerkZoologist", new Tuple<String, String>("PerkZoologist", "Vorteil Zoo") },
                    { "Diplomacy", new Tuple<String, String>("Diplomacy", "Diplomatie") },
                    { "Faith", new Tuple<String, String>("Faith", "Glaube") },
                    { "PerkMilitaryShip", new Tuple<String, String>("Perk Military Ship", "Vorteil Militärschiff") },
                    { "PerkHypnotist", new Tuple<String, String>("Perk Hypnotist", "Vorteil Hypnose") },
                    { "PerkAnthropologist", new Tuple<String, String>("Perk Anthropologist", "Vorteil Anthropologe") },
                    { "PerkPolyglot", new Tuple<String, String>("Perk Polyglot", "Vorteil Mehrsprachigkeit") }
                };
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
            if (result?.Length == 0)
                result = element.Element("EN").Element("Short").Value;
            return result;
        }
        #endregion

    }
}