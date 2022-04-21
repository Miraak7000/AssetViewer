// Copyright © 2020 Vera@Versus. All rights reserved. Licensed under the MIT License.

namespace AssetViewer.Data.Asset {

  using System.ComponentModel;
  using System.Linq;
  using System.Runtime.CompilerServices;

  public class CountMode : INotifyPropertyChanged {

    #region Public Properties

    public bool IsEnabled {
      get => isEnabled;
      set {
        isEnabled = value;
        RaisePropertyChanged(nameof(IsEnabled));
      }
    }

    public uint Count {
      get => count;
      set {
        if (count != value) {
          count = value;
          RaisePropertyChanged(nameof(Count));
          if (!asset.SetParts?.Any() ?? true) {
            AssetProvider.RaiseAssetCountChanged(asset);
          }
        }
        if (asset.SetParts?.Any() ?? false) {
          try {
            var parts = AssetProvider.GetItemsById(asset.SetParts.Select(p => p));
            foreach (var part in parts) {
              part.CountMode.Count = count;
            }
          }
          catch (System.Exception) {
          }
        }
      }
    }

    #endregion Public Properties

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Constructors

    public CountMode(TemplateAsset asset) {
      this.asset = asset;
    }

    #endregion Public Constructors

    #region Public Methods

    public void RaisePropertyChanged([CallerMemberName] string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion Public Methods

    #region Private Fields

    private readonly TemplateAsset asset;
    private uint count;
    private bool isEnabled;

    #endregion Private Fields
  }
}