﻿using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetViewer.Data {

  [Serializable]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot("I")]
  public partial class PoolItem {

    #region Public Properties

    [XmlAttribute]
    public int ID { get; set; }

    [XmlAttribute("W")]
    public int Weight { get; set; }

    public object Item {
      get {
        if (AssetProvider.Items.ContainsKey(ID)) {
          return AssetProvider.Items[ID];
        }
        else if (AssetProvider.Pools.ContainsKey(ID)) {
          return AssetProvider.Pools[ID];
        }
        else {
          return null;
        }
      }
    }

    #endregion Public Properties
  }
}