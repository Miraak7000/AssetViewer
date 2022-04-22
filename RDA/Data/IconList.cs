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

  public class IconList : List<(string, GameTypes, string)> {

    public string this[string key, GameTypes gameType] { get => Single(key, gameType); }

    public void Add(string key, GameTypes gameType, string value ) {
      this.Add((key, gameType, value));
    }

    public bool TryGetValue(string key, out string value, GameTypes gameType) {
      value = Single(key, gameType);
      return value != default;
    }

    private string Single(string key, GameTypes gameType) {
      var list = this.Where(i => i.Item1 == key).ToList();
      var rightOne = list.FirstOrDefault(i => i.Item2 == gameType);
      if (rightOne != default) {
        return rightOne.Item3;
      }
      else {
        return list.FirstOrDefault().Item3;
      }
    }
  }
}
