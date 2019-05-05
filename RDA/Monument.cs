using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable CoVariantArrayConversion
namespace RDA {

  /// <summary>
  ///   World Fair
  /// </summary>
  public static class Monument {

    #region Public Methods
    public static void Create() {
      Monument.MonumentEventCategory();
      Monument.MonumentEvent();
      Monument.MonumentEventReward();
    }
    #endregion

    #region Private Methods
    private static void MonumentEventCategory() {
      var assets = Program.Original.Root.XPathSelectElements("//Asset[Template='MonumentEventCategory']").ToArray();
      foreach (var asset in assets) {
        var assetGuid = asset.XPathSelectElement("Values/Standard/GUID").Value;
        var textEN = asset.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
        var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={assetGuid}]/Text").Value;
        asset.XPathSelectElement("Values/Standard/Name").Remove();
        asset.XPathSelectElement("Values/Locked").Remove();
        asset.XPathSelectElement("Values/MonumentEventCategory/CategoryDescriptionFluff").Remove();
        asset.XPathSelectElement("Values/MonumentEventCategory/EventAudioState")?.Remove();
        asset.XPathSelectElement("Values/MonumentEventCategory/CategoryBackground").Remove();
        asset.XPathSelectElement("Values/MonumentEventCategory/SizeSelectionBackground").Remove();
        // modify description
        asset.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
        asset.XPathSelectElement("Values/Text").Remove();
        // modify reward
        var rewardGuid = asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={rewardGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={rewardGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward").Add(new XElement("Reward", rewardGuid));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/Description/DE").Add(new XElement("Short", textDE));
        var iconFilename = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={rewardGuid}]/IconFilename").Value;
        asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward").Add(new XElement("IconFilename", iconFilename));
        // images
        Helper.SetImage(asset.XPathSelectElement("Values/Standard/IconFilename"));
        Helper.SetImage(asset.XPathSelectElement("Values/MonumentEventCategory/PotentialCategoryReward/IconFilename"));
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentEventCategory", assets));
      document.Save(Program.PathRoot + "/Modified/Assets_MonumentEventCategory.xml");
    }
    private static void MonumentEvent() {
      var assets = Program.Original.Root.XPathSelectElements("//Asset[Template='MonumentEvent']").ToArray();
      foreach (var asset in assets) {
        var assetGuid = asset.XPathSelectElement("Values/Standard/GUID").Value;
        var textEN = asset.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
        var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={assetGuid}]/Text").Value;
        asset.XPathSelectElement("Values/Standard/Name").Remove();
        asset.XPathSelectElement("Values/Locked").Remove();
        asset.XPathSelectElement("Values/Cost").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/PreparationTime").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/EventDuration").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/NeededWorkforceAmount").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/MaxUsableWorkforceAmount").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/DescriptionFluff").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/MaxPreparationAttractiveness").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/MinPreparationAttractiveness").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/ShortSizeName").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/PreparationGoods").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/ReduceAttractivenessPerMinute").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/RequirementText").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/BackgroundPreparation").Remove();
        asset.XPathSelectElement("Values/MonumentEvent/BackgroundRunning").Remove();
        // modify description
        asset.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
        asset.XPathSelectElement("Values/Text").Remove();
        // modify selection text
        var selectionGuid = asset.XPathSelectElement("Values/MonumentEvent/SelectionText").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={selectionGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={selectionGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEvent/SelectionText/Description/DE").Add(new XElement("Short", textDE));
        // modify running text
        var runningGuid = asset.XPathSelectElement("Values/MonumentEvent/RunningText").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={runningGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={runningGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEvent/RunningText").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEvent/RunningText").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEvent/RunningText/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEvent/RunningText/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEvent/RunningText/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEvent/RunningText/Description/DE").Add(new XElement("Short", textDE));
        // modify reward text
        var rewardGuid = asset.XPathSelectElement("Values/MonumentEvent/RewardText").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={rewardGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={rewardGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEvent/RewardText").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEvent/RewardText").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEvent/RewardText/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEvent/RewardText/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEvent/RewardText/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEvent/RewardText/Description/DE").Add(new XElement("Short", textDE));
        // modify event text
        var eventGuid = asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={eventGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={eventGuid}]/Text").Value;
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward").Value = String.Empty;
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward").Add(new XElement("Reward", rewardGuid));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward").Add(new XElement("Description"));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/Description/DE").Add(new XElement("Short", textDE));
        var iconFilename = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={eventGuid}]/IconFilename").Value;
        asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward").Add(new XElement("IconFilename", iconFilename));
        // images
        Helper.SetImage(asset.XPathSelectElement("Values/Standard/IconFilename"));
        Helper.SetImage(asset.XPathSelectElement("Values/MonumentEvent/PotentialEventReward/IconFilename"));
        // tresholds
        foreach (var item in asset.XPathSelectElements("Values/MonumentEvent/RewardThresholds/Item")) {
          item.Element("NeededAttractiveness")?.Remove();
        }
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentEvent", assets));
      document.Save(Program.PathRoot + "/Modified/Assets_MonumentEvent.xml");
    }
    private static void MonumentEventReward() {
      var assets = Program.Original.Root.XPathSelectElements("//Asset[Template='MonumentEventReward']").ToArray();
      foreach (var asset in assets) {
        var assetGuid = asset.XPathSelectElement("Values/Standard/GUID").Value;
        var textEN = asset.XPathSelectElement("Values/Text/LocaText/English/Text").Value;
        var textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={assetGuid}]/Text").Value;
        asset.XPathSelectElement("Values/Standard/Name").Remove();
        // modify description
        asset.XPathSelectElement("Values/Standard").AddAfterSelf(new XElement("Description"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("EN"));
        asset.XPathSelectElement("Values/Description").Add(new XElement("DE"));
        asset.XPathSelectElement("Values/Description/EN").Add(new XElement("Short", textEN));
        asset.XPathSelectElement("Values/Description/DE").Add(new XElement("Short", textDE));
        asset.XPathSelectElement("Values/Text").Remove();
        // info
        var infoGuid = asset.XPathSelectElement("Values/Standard/InfoDescription").Value;
        textEN = Program.Original.Root.XPathSelectElement($"//Asset[Template='Text']/Values/Standard[GUID={infoGuid}]/../Text/LocaText/English/Text").Value;
        textDE = Program.TextDE.Root.XPathSelectElement($"Texts/Text[GUID={infoGuid}]/Text").Value;
        asset.XPathSelectElement("Values/Description/EN").Add(new XElement("Long", textEN));
        asset.XPathSelectElement("Values/Description/DE").Add(new XElement("Long", textDE));
        asset.XPathSelectElement("Values/Standard/InfoDescription").Remove();
        // find rewards
        var rewardGuids = asset.XPathSelectElements("Values/Reward/RewardAssets/Item/Reward").Select(s => s.Value).ToArray();
        var rewardNode = asset.XPathSelectElement("Values/Reward");
        rewardNode.Element("RewardAssets").Remove();
        foreach (var rewardGuid in rewardGuids) {
          var reward = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={rewardGuid}]/../..");
          if (reward.XPathSelectElement("Template").Value == "RewardPool") {
            var poolGuids1 = reward.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").Select(s => s.Value).ToArray();
            foreach (var poolGuid1 in poolGuids1) {
              reward = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={poolGuid1}]/../..");
              if (reward.XPathSelectElement("Template").Value == "RewardPool") {
                var poolGuids2 = reward.XPathSelectElements("Values/RewardPool/ItemsPool/Item/ItemLink").Select(s => s.Value).ToArray();
                foreach (var poolGuid2 in poolGuids2) {
                  reward = Program.Original.Root.XPathSelectElement($"//Asset/Values/Standard[GUID={poolGuid2}]/../..");
                  rewardNode.Add(reward);
                }
              } else
                rewardNode.Add(reward);
            }
          } else
            rewardNode.Add(reward);
        }
        // modify rewards
        foreach (var item in asset.XPathSelectElements("Values/Reward/Asset")) {
          switch (item.XPathSelectElement("Template").Value) {
            case "BuildPermitBuilding":
              Helper.TemplateBuildPermitBuilding(item);
              break;
            case "GuildhouseItem":
              Helper.TemplateGuildhouseItem(item);
              break;
            case "CultureItem":
              Helper.TemplateCultureItem(item);
              break;
            case "HarborOfficeItem":
              Helper.TemplateHarborOfficeItem(item);
              break;
            case "VehicleItem":
              Helper.TemplateVehicleItem(item);
              break;
            case "ActiveItem":
              Helper.TemplateActiveItem(item);
              break;
            default:
              throw new NotImplementedException();
          }
        }
        // image
        Helper.SetImage(asset.XPathSelectElement("Values/Standard/IconFilename"));
      }
      var document = new XDocument();
      document.Add(new XElement("MonumentEventReward", assets));
      document.Save(Program.PathRoot + "/Modified/Assets_MonumentEventReward.xml");
    }
    #endregion

  }

}