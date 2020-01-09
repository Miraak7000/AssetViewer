using AssetViewer.Data.Filters;
using AssetViewer.Templates;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AssetViewer.Controls {

  public class ItemsBase : UserControl, INotifyPropertyChanged {

    #region Properties

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
    public string ResetButtonText => App.Descriptions[-1100];
    public string AdvancedFiltersText => App.Descriptions[-1104];

    #endregion Properties

    #region Constructors

    public ItemsBase() : base() {
      Initialize();
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

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

    #endregion Methods

    #region Fields

    private TemplateAsset selectedAsset;

    #endregion Fields

    private void Initialize() {
      ItemsHolder.SetItems();
      this.Loaded += UserControl_Loaded;
      this.Unloaded += UserControl_Unloaded;
      this.DataContext = this;
    }

    private void UserControl_Loaded(Object sender, RoutedEventArgs e) {
      if (Application.Current.MainWindow is MainWindow main) {
        main.OnLanguage_Changed += this.ComboBoxLanguage_SelectionChanged;
      }
    }

    private void ComboBoxLanguage_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
      ItemsHolder.RaiseLanguageChanged();
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e) {
      if (Application.Current.MainWindow is MainWindow main) {
        main.OnLanguage_Changed -= this.ComboBoxLanguage_SelectionChanged;
      }
    }
  }
}