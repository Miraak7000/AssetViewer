// Copyright © 2020 Vera@Versus. All rights reserved.
// Licensed under the MIT License.

namespace RDA.Data {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using System.Xml.Linq;
  using System.Xml.XPath;

  public class GUIDList : List<(string, XElement)> {

    public XElement this[string key, GameTypes gameType] { get => Single(key, gameType); }

    public void Add(string key, XElement value) {
      this.Add((key, value));
    }

    public IEnumerable<XElement> Values { get => this.Select(i => i.Item2); }

    public bool TryGetValue(string key, out XElement value, GameTypes gameType) {
      value = Single(key, gameType);
      return value != default;
    }

    private XElement Single(string key, GameTypes gameType) {
      var list = this.Where(i => i.Item1 == key).Select(i => i.Item2).ToList();
      var rightOne = list.FirstOrDefault(i => i.Attribute("GameType")?.Value == ((int)gameType).ToString());
      if (rightOne != null) {
        return rightOne;
      }
      else {
        return list.FirstOrDefault();
      }
    }
  }
}
