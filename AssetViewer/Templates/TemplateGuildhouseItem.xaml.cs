using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Data;

namespace AssetViewer.Templates {

  public partial class TemplateGuildhouseItem : UserControl {

    #region Properties
    public LinearGradientBrush RarityBrush {
      get {
        switch (this.Asset.XPathSelectElement("Values/Item/Rarity")?.Value) {
          case "Uncommon":
            return new LinearGradientBrush(new GradientStopCollection() {
              new GradientStop(Color.FromRgb(65, 89, 41), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
          case "Rare":
            return new LinearGradientBrush(new GradientStopCollection() {
              new GradientStop(Color.FromRgb(50, 60, 83), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
          case "Epic":
            return new LinearGradientBrush(new GradientStopCollection() {
              new GradientStop(Color.FromRgb(90, 65, 89), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
          case "Legendary":
            return new LinearGradientBrush(new GradientStopCollection() {
              new GradientStop(Color.FromRgb(98, 66, 46), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
          default:
            return new LinearGradientBrush(new GradientStopCollection() {
              new GradientStop(Color.FromRgb(126, 128, 125), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
        }
      }
    }
    public String Icon {
      get { return $"/AssetViewer;component/Resources/{this.Asset.XPathSelectElement("Values/Standard/IconFilename").Value}"; }
    }
    public XElement Description {
      get { return this.Asset.XPathSelectElement("Values/Description"); }
    }
    public DataRarity Rarity {
      get {
        return new DataRarity(this.Asset);
      }
    }
    public DataAllocation Allocation {
      get { return new DataAllocation(this.Asset); }
    }
    public DataEffectTargets ItemEffect {
      get {
        if (this.Asset.XPathSelectElement("Values/ItemEffect/EffectTargets") == null) return null;
        return new DataEffectTargets(this.Asset);
      }
    }
    // FactoryUpgrade
    public DataProductivityUpgrade ProductivityUpgrade {
      get {
        if (this.Asset.XPathSelectElement("Values/FactoryUpgrade/ProductivityUpgrade") == null) return null;
        return new DataProductivityUpgrade(this.Asset);
      }
    }
    public DataAddedFertility AddedFertility {
      get {
        if (this.Asset.XPathSelectElement("Values/FactoryUpgrade/AddedFertility") == null) return null;
        return new DataAddedFertility(this.Asset);
      }
    }
    // BuildingUpgrade
    public DataWorkforceAmountUpgrade WorkforceAmountUpgrade {
      get {
        if (this.Asset.XPathSelectElement("Values/BuildingUpgrade/WorkforceAmountUpgrade") == null) return null;
        return new DataWorkforceAmountUpgrade(this.Asset);
      }
    }
    public DataMaintenanceUpgrade MaintenanceUpgrade {
      get {
        if (this.Asset.XPathSelectElement("Values/BuildingUpgrade/MaintenanceUpgrade") == null) return null;
        return new DataMaintenanceUpgrade(this.Asset);
      }
    }
    // ModuleOwnerUpgrade
    public DataModuleLimitUpgrade ModuleLimitUpgrade {
      get {
        if (this.Asset.XPathSelectElement("Values/ModuleOwnerUpgrade/ModuleLimitUpgrade") == null) return null;
        return new DataModuleLimitUpgrade(this.Asset);
      }
    }
    // IncidentInfectableUpgrade
    public DataIncidentFireIncreaseUpgrade IncidentFireIncreaseUpgrade {
      get {
        if (this.Asset.XPathSelectElement("Values/IncidentInfectableUpgrade/IncidentFireIncreaseUpgrade") == null) return null;
        return new DataIncidentFireIncreaseUpgrade(this.Asset);
      }
    }
    // CultureUpgrade
    public DataAttractivenessUpgrade AttractivenessUpgrade {
      get {
        if (this.Asset.XPathSelectElement("Values/CultureUpgrade/AttractivenessUpgrade") == null) return null;
        return new DataAttractivenessUpgrade(this.Asset);
      }
    }
    // ProvideElectricity
    public DataProvideElectricity ProvideElectricity {
      get {
        if (this.Asset.XPathSelectElement("Values/ElectricUpgrade/ProvideElectricity") == null) return null;
        return new DataProvideElectricity(this.Asset);
      }
    }
    // IncidentInfectableUpgrade
    public DataIncidentExplosionIncreaseUpgrade IncidentExplosionIncreaseUpgrade {
      get {
        if (this.Asset.XPathSelectElement("Values/IncidentInfectableUpgrade/IncidentExplosionIncreaseUpgrade") == null) return null;
        return new DataIncidentExplosionIncreaseUpgrade(this.Asset);
      }
    }
    // ExpeditionAttribute
    public DataExpeditionAttribute ExpeditionAttribute {
      get {
        if (this.Asset.XPathSelectElement("Values/ExpeditionAttribute/ExpeditionAttributes") == null) return null;
        return new DataExpeditionAttribute(this.Asset);
      }
    }
    public DataExpeditionAttributeCrafting ExpeditionAttributeCrafting {
      get {
        if (this.Asset.XPathSelectElement("Values/ExpeditionAttribute/ExpeditionAttributes/Item[Attribute='Crafting']") == null) return null;
        return new DataExpeditionAttributeCrafting(this.Asset);
      }
    }
    public DataExpeditionAttributeHunting ExpeditionAttributeHunting {
      get {
        if (this.Asset.XPathSelectElement("Values/ExpeditionAttribute/ExpeditionAttributes/Item[Attribute='Hunting']") == null) return null;
        return new DataExpeditionAttributeHunting(this.Asset);
      }
    }
    public DataExpeditionAttributeMedicine ExpeditionAttributeMedicine {
      get {
        if (this.Asset.XPathSelectElement("Values/ExpeditionAttribute/ExpeditionAttributes/Item[Attribute='Medicine']") == null) return null;
        return new DataExpeditionAttributeMedicine(this.Asset);
      }
    }
    public DataExpeditionAttributeNavigation ExpeditionAttributeNavigation {
      get {
        if (this.Asset.XPathSelectElement("Values/ExpeditionAttribute/ExpeditionAttributes/Item[Attribute='Navigation']") == null) return null;
        return new DataExpeditionAttributeNavigation(this.Asset);
      }
    }
    public DataExpeditionAttributeFaith ExpeditionAttributeFaith {
      get {
        if (this.Asset.XPathSelectElement("Values/ExpeditionAttribute/ExpeditionAttributes/Item[Attribute='Faith']") == null) return null;
        return new DataExpeditionAttributeFaith(this.Asset);
      }
    }
    #endregion

    #region Fields
    private readonly XElement Asset;
    #endregion

    #region Constructor
    public TemplateGuildhouseItem() {
      this.InitializeComponent();
    }
    public TemplateGuildhouseItem(XElement asset) {
      this.InitializeComponent();
      this.Asset = asset;
      this.DataContext = this;
    }
    #endregion

  }

}