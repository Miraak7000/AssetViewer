using AssetViewer.Data;
using AssetViewer.Data.Filters;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using VAV;

namespace AssetViewer.Controls {

  public class ItemsBase : UserControl, INotifyPropertyChanged {

    #region Public Properties

    public static RelayCommand<SelectedCountChangedArgs> SelectedCountChangedCommand { get; private set; }

    public TemplateAsset SelectedAsset {
      get {
        return selectedAsset;
      }
      set {
        if (selectedAsset != value) {
          selectedAsset = value;
          RaisePropertyChanged();
        }
      }
    }

    public virtual ItemsHolder ItemsHolder { get; }
    public string ResetButtonText => AssetProvider.Descriptions[-1100];
    public string AdvancedFiltersText => AssetProvider.Descriptions[-1104];

    #endregion Public Properties

    #region Public Constructors

    static ItemsBase() {
      SelectedCountChangedCommand = new RelayCommand<SelectedCountChangedArgs>(ExecuteSelectedCountChanged, CanSelectedCountChange);
    }

    public ItemsBase() : base() {
      Initialize();
    }

    #endregion Public Constructors

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Methods

    public void BtnResetFilters_Click(object sender, RoutedEventArgs e) => ItemsHolder.ResetFilters();

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void BtnAddFilter_Click(object sender, RoutedEventArgs e) {
      ItemsHolder.CustomFilters.Add(ItemsHolder.CreateFilterHolder());
    }

    public void BtnRemoveFilter_Click(object sender, RoutedEventArgs e) {
      var filter = (FilterHolder)(sender as Button)?.DataContext;
      if (ItemsHolder.CustomFilters.Contains(filter)) {
        ItemsHolder.CustomFilters.Remove(filter);
      }
    }

    public void ComboBox_SelectionChanged(object sender, EventArgs e) {
      ItemsHolder.UpdateUI();
      if (!ItemsHolder.IsRefreshingUi && SelectedAsset == null) {
        SelectedAsset = ItemsHolder.Items.FirstOrDefault();
      }
    }

    #endregion Public Methods

    #region Private Fields

    private TemplateAsset selectedAsset;

    #endregion Private Fields

    #region Private Methods

    private static bool CanSelectedCountChange(SelectedCountChangedArgs obj) {
      return obj.Assets?.Any() == true;
    }

    private static void ExecuteSelectedCountChanged(SelectedCountChangedArgs obj) {
      if (obj.Assets != null) {
        foreach (var asset in obj.Assets) {
          asset.Count = obj.Count;
        }
      }
    }

    private void Initialize() {
      ItemsHolder?.SetItems();
      this.Loaded += UserControl_Loaded;
      this.Unloaded += UserControl_Unloaded;
      this.DataContext = this;
    }

    private void UserControl_Loaded(Object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed += this.ComboBoxLanguage_SelectionChanged;
    }

    private void ComboBoxLanguage_SelectionChanged() {
      ItemsHolder.RaiseLanguageChanged();
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed -= this.ComboBoxLanguage_SelectionChanged;
    }

    #endregion Private Methods
  }
}