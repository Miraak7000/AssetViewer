using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using RDA.Data;

namespace RDA {

  internal static class Extensions {

    #region Public Properties

    public static ConcurrentDictionary<string, XElement> Events { get; set; } = new ConcurrentDictionary<string, XElement>();

    #endregion Public Properties

    #region Public Methods

    public static XElement FindParentElement(this string id, string[] ParentTypes, List<string> previousIDs = null) {
      if (Events.ContainsKey(id)) {
        return Events[id];
      }
      XElement result = null;
      previousIDs = previousIDs ?? new List<string>();
      previousIDs.Add(id);

      var cachedLinks = Assets.References.ContainsKey(id) ? Assets.References[id] : new HashSet<XElement>();

      foreach (var asset in cachedLinks)
      {
        foreach (var reference in asset.Descendants())
        {
          if ("GUID".Equals(reference.Name.LocalName) || !id.Equals(reference.Value) || reference.HasElements)
            continue;

          if ("InsertEvent".Equals(reference.Name))
            continue;

          if (asset.Element("Template") == null) {
            continue;
          }

          if (previousIDs.Contains(asset.XPathSelectElement("Values/Standard/GUID").Value)) {
            continue;
          }

          if (ParentTypes.Contains(asset.Element("Template").Value)) {
            return asset;
          }
          result = FindParentElement(asset.XPathSelectElement("Values/Standard/GUID").Value, ParentTypes, previousIDs);
          if (result != null) {
            break;
          }
        }
      }
      if (!Events.ContainsKey(id)) {
        Events.TryAdd(id, result);
      }
      return result;
    }

    public static XElement GetProxyElement(this XElement element, string proxyName) {
      var xRoot = new XElement("Proxy");
      var xTemplate = new XElement("Template") {
        Value = proxyName
      };
      xRoot.Add(xTemplate);
      xRoot.Add(element);
      xRoot.Add(new XElement("Values", element.XPathSelectElement("Values/Standard")));
      return xRoot;
    }

    public static SourceWithDetailsList MergeResults(this ConcurrentBag<SourceWithDetailsList> tempresult, string key, in SourceWithDetailsList mainResult = null) {
      var sourceWithDetailsList = mainResult ?? new SourceWithDetailsList();
      foreach (var result in tempresult) {
        if (result.ElementWeights.TryGetValue(key, out var newWeight)) {
          foreach (var sourceWithDetail in result) {
            foreach (var detail in sourceWithDetail.Details) {
              detail.Weight *= newWeight;
            }
          }
        }
        sourceWithDetailsList.AddSourceAsset(result);
      }

      var events = tempresult.SelectMany(r => r.FollowingEvents);
      if (events.Any()) {
        sourceWithDetailsList.FollowingEvents = new HashSet<XElement>(events);
      }
      return sourceWithDetailsList;
    }

    public static ConcurrentBag<SourceWithDetailsList> SaveSource(this ConcurrentBag<SourceWithDetailsList> tempresult, string key) {
      Asset.SavedSources.TryAdd(key, new ConcurrentBag<SourceWithDetailsList>(tempresult.Select(i => i.Copy())));
      return tempresult;
    }

    public static bool MatchOne(this string value, params string[] matches) {
      foreach (var item in matches) {
        if (value == item) {
          return true;
        }
      }
      return false;
    }

    public static string Round(this double number) {
      var chars = number.ToString();
      var afterdigitNumber = false;
      var pointIndex = chars.IndexOf(',');
      if (pointIndex >= 0) {
        for (var i = pointIndex + 1; i < chars.Length; i++) {
          if (chars[i] != '0') {
            if (afterdigitNumber) {
              return chars.Substring(0, i + 1);
            }
            afterdigitNumber = true;
          }
          else {
            if (afterdigitNumber) {
              return chars.Substring(0, i);
            }
          }
        }
      }
      return chars;
    }

    #endregion Public Methods
  }
}