using AssetViewer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace AssetViewer {

  [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  public partial class App : Application {

    #region Public Methods

    [STAThread]
    public static void Main() {
      AssetProvider.CountMode = true;
      AssetProvider.OnAssetCountChanged += AssetProvider_OnAssetCountChanged;
      var app = new App();
      app.InitializeComponent();
      app.Run();
    }

    #endregion Public Methods

    #region Private Methods

    private static void AssetProvider_OnAssetCountChanged(IEnumerable<TemplateAsset> obj) {
    }

    #endregion Private Methods
  }
}