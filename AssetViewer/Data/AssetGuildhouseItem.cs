using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using AssetViewer.Data.GuildhouseItem;
using AssetViewer.Library;

namespace AssetViewer.Data {

  [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public class AssetGuildhouseItem : Asset {

    #region Properties
    public String Rarity { get; set; }
    public Allocation Allocation { get; set; }
    public Boolean IsSpecialist { get; set; }
    public FactoryUpgrade FactoryUpgrade { get; set; }
    public BuildingUpgrade BuildingUpgrade { get; set; }
    public ElectricUpgrade ElectricUpgrade { get; set; }
    public IncidentInfectableUpgrade IncidentInfectableUpgrade { get; set; }
    public CultureUpgrade CultureUpgrade { get; set; }
    public ModuleOwnerUpgrade ModuleOwnerUpgrade { get; set; }
    public IncidentInfluencerUpgrade IncidentInfluencerUpgrade { get; set; }
    public ResidenceUpgrade ResidenceUpgrade { get; set; }
    public PopulationUpgrade PopulationUpgrade { get; set; }
    public ExpeditionAttribute ExpeditionAttribute { get; set; }
    public List<EffectTarget> EffectTargets { get; }
    public String EffectTarget {
      get {
        var sb = new StringBuilder();
        foreach (var item in this.EffectTargets) {
          if (sb.Length > 0) sb.Append("; ");
          sb.Append(item);
        }
        return sb.ToString();
      }
    }
    public Boolean HasRarity {
      get { return !String.IsNullOrWhiteSpace(this.Rarity); }
    }
    public Boolean HasFactoryUpgrade {
      get { return this.FactoryUpgrade.Content != null; }
    }
    public Boolean HasBuildingUpgrade {
      get { return this.BuildingUpgrade.Content != null; }
    }
    public Boolean HasIncidentInfectableUpgrade {
      get { return this.IncidentInfectableUpgrade.Content != null; }
    }
    public Boolean HasElectricUpgrade {
      get { return this.ElectricUpgrade.Content != null; }
    }
    public Boolean HasCultureUpgrade {
      get { return this.CultureUpgrade.Content != null; }
    }
    public Boolean HasModuleOwnerUpgrade {
      get { return this.ModuleOwnerUpgrade.Content != null; }
    }
    public Boolean HasIncidentInfluencerUpgrade {
      get { return this.IncidentInfluencerUpgrade.Content != null; }
    }
    public Boolean HasResidenceUpgrade {
      get { return this.ResidenceUpgrade.Content != null; }
    }
    public Boolean HasPopulationUpgrade {
      get { return this.PopulationUpgrade.Content != null; }
    }
    public Boolean HasExpeditionAttribute {
      get { return this.ExpeditionAttribute.Content != null; }
    }
    #endregion

    #region Constructor
    public AssetGuildhouseItem(XElement element) {
      // init
      this.Allocation = new Allocation {
        GUID = 1,
        Description = new Description("Guild House", "Gildenhaus")
      };
      this.EffectTargets = new List<EffectTarget>();
      // assign
      this.GUID = Int32.Parse(element.Element("Values").Element("Standard").Element("GUID").Value);
      this.Description = new Description {
        ShortEN = element.Element("Values").Element("Description").Element("EN").Element("Short").Value,
        LongEN = element.Element("Values").Element("Description").Element("EN").Element("Long")?.Value,
        ShortDE = element.Element("Values").Element("Description").Element("DE")?.Element("Short")?.Value,
        LongDE = element.Element("Values").Element("Description").Element("DE")?.Element("Long")?.Value
      };
      this.Rarity = element.Element("Values").Element("Item").Element("Rarity")?.Value;
      foreach (var effectTarget in element.Element("Values").Element("ItemEffect").Element("EffectTargets").Elements()) {
        this.EffectTargets.Add(new EffectTarget(effectTarget));
      }
      if (element.Element("Values").Element("Item").Element("ItemType") != null) {
        switch (element.Element("Values").Element("Item").Element("ItemType").Value) {
          case "Specialist":
            this.IsSpecialist = true;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      if (element.Element("Values").Element("Item").Element("Allocation") != null) {
        switch (element.Element("Values").Element("Item").Element("Allocation").Value) {
          case "HarborOffice":
            this.Allocation = new Allocation {
              GUID = 1,
              Description = new Description("Harbor Office", "Hafenmeisterei")
            };
            break;
          case "TownHall":
            this.Allocation = new Allocation {
              GUID = 2,
              Description = new Description("Town Hall", "Rathaus")
            };
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      } else {
        this.Allocation = new Allocation {
          GUID = 0,
          Description = new Description("Guild House", "Gildenhaus")
        };
      }
      this.FactoryUpgrade = new FactoryUpgrade(element.Element("Values").Element("FactoryUpgrade"));
      this.BuildingUpgrade = new BuildingUpgrade(element.Element("Values").Element("BuildingUpgrade"));
      this.ElectricUpgrade = new ElectricUpgrade(element.Element("Values").Element("ElectricUpgrade"));
      this.IncidentInfectableUpgrade = new IncidentInfectableUpgrade(element.Element("Values").Element("IncidentInfectableUpgrade"));
      this.CultureUpgrade = new CultureUpgrade(element.Element("Values").Element("CultureUpgrade"));
      this.ModuleOwnerUpgrade = new ModuleOwnerUpgrade(element.Element("Values").Element("ModuleOwnerUpgrade"));
      this.IncidentInfluencerUpgrade = new IncidentInfluencerUpgrade(element.Element("Values").Element("IncidentInfluencerUpgrade"));
      this.ResidenceUpgrade = new ResidenceUpgrade(element.Element("Values").Element("ResidenceUpgrade"));
      this.PopulationUpgrade = new PopulationUpgrade(element.Element("Values").Element("PopulationUpgrade"));
      this.ExpeditionAttribute = new ExpeditionAttribute(element.Element("Values").Element("ExpeditionAttribute"));
    }
    #endregion

    #region Public Methods
    public override String ToString() {
      return App.Language == Languages.English ? this.Description.ShortEN : this.Description.ShortDE;
    }
    #endregion

  }

}