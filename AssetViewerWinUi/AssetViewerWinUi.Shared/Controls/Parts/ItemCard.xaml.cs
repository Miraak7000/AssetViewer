using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AssetViewer.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace AssetViewer.Controls {

  public partial class ItemCard : UserControl, INotifyPropertyChanged {

    #region Public Properties

    public bool CanSwap {
      get => (bool)GetValue(CanSwapProperty);
      set => SetValue(CanSwapProperty, value);
    }

    public TemplateAsset SelectedAsset {
      get => (TemplateAsset)GetValue(SelectedAssetProperty);
      set => SetValue(SelectedAssetProperty, value);
    }

    public LinearGradientBrush RarityBrush {
      get {
        var selection = SelectedAsset?.RarityType ?? "Common";
        switch (selection) {
          case "Uncommon":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(65, 89, 41), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Rare":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(50, 60, 83), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Epic":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(90, 65, 89), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Legendary":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(98, 66, 46), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          default:
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(126, 128, 125), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
        }
      }
    }

    public string AllocationText => AssetProvider.Descriptions[-106];

    public string ExpeditionText => AssetProvider.Descriptions[-1220];

    public string TradeText => AssetProvider.Descriptions[12725];

    public string HiringFeeText => AssetProvider.Descriptions[21731];

    public string ItemSetText => AssetProvider.Descriptions[-1221];

    public string ItemTrasmutable => AssetProvider.Descriptions[113817];

    public string ProductionText => AssetProvider.Descriptions[100006];

    public string ConsumptionText => AssetProvider.Descriptions[100007];

    public string BuildCostsText => AssetProvider.Descriptions[100008];

    public string MaintenanceText => AssetProvider.Descriptions[100409];

    public string UpgradeCostsText => AssetProvider.Descriptions[2001775];

    #endregion Public Properties

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Constructors

    public ItemCard() {
      InitializeComponent();
      Loaded += ItemCard_Loaded;
      Unloaded += ItemCard_Unloaded;
    }

    #endregion Public Constructors

    #region Public Methods

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion Public Methods

    #region Public Fields

    public static readonly DependencyProperty CanSwapProperty =
                                                                DependencyProperty.Register(nameof(CanSwap), typeof(bool), typeof(ItemCard), new PropertyMetadata(false));

    public static readonly DependencyProperty SelectedAssetProperty =
        DependencyProperty.Register(nameof(SelectedAsset), typeof(TemplateAsset), typeof(ItemCard), new PropertyMetadata(null, OnSelectedAssetChanged));

    #endregion Public Fields

    #region Private Methods

    private static void OnSelectedAssetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (d is ItemCard card) {
        card.RaisePropertyChanged(nameof(RarityBrush));
      }
    }

    private void ItemCard_Unloaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed -= ComboBoxLanguage_SelectionChanged;
    }

    private void ItemCard_Loaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed += ComboBoxLanguage_SelectionChanged;
    }

    private void ComboBoxLanguage_SelectionChanged() {
      mainGrid.DataContext = null;
      mainGrid.DataContext = this;
    }

    private void ButtonSwitch_Click(object sender, RoutedEventArgs e) {
      if (ItemFront.Visibility == Visibility.Visible) {
        ItemFront.Visibility = Visibility.Collapsed;
        ItemBack.Visibility = Visibility.Visible;
      }
      else {
        ItemBack.Visibility = Visibility.Collapsed;
        ItemFront.Visibility = Visibility.Visible;
      }
    }

    #endregion Private Methods
  }
}