using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Library {

  internal static class Extensions {

    #region Properties

    public static ConcurrentDictionary<string, XElement> Events { get; set; } = new ConcurrentDictionary<string, XElement>();

    #endregion Properties

    #region Methods

    public static XElement FindParentElement(this String id, string[] ParentTypes, List<String> previousIDs = null) {
      if (Events.ContainsKey(id)) {
        return Events[id];
      }
      XElement result = null;
      previousIDs = previousIDs ?? new List<string>();
      previousIDs.Add(id);
      var links = Assets.Original.Root.XPathSelectElements($"//*[text()={id} and not(self::GUID) and not(self::InsertEvent)]").ToArray();
      if (links.Length > 0) {
        for (var i = 0; i < links.Length; i++) {
          var element = links[i];
          while (element.Name.LocalName != "Asset" || !element.HasElements) {
            element = element.Parent;
          }
          if (element.Element("Template") == null) {
            continue;
          }

          if (previousIDs.Contains(element.XPathSelectElement("Values/Standard/GUID").Value)) {
            continue;
          }

          if (ParentTypes.Contains(element.Element("Template").Value)) {
            return element;
          }
          result = FindParentElement(element.XPathSelectElement("Values/Standard/GUID").Value, ParentTypes, previousIDs);
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
      var xTemplate = new XElement("Template");
      xTemplate.Value = proxyName;
      xRoot.Add(xTemplate);
      xRoot.Add(element);
      xRoot.Add(new XElement("Values", element.XPathSelectElement("Values/Standard")));
      return xRoot;
    }

    #endregion Methods
  }
}