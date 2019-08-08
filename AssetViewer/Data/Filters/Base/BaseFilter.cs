using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public abstract class BaseFilter<T> : IFilter<T>, INotifyPropertyChanged {

    #region Properties

    public FilterType FilterType { get; set; } = FilterType.Selection;
    public FilterType ComparisonType { get; set; }
    public abstract string DescriptionID { get; }

    public virtual T SelectedValue {
      get {
        return _selectedValue;
      }
      set {
        if (!(_selectedValue?.Equals(value) ?? false)) {
          _selectedValue = value;
          RaisePropertyChanged();
          //ItemsHolder.UpdateUI(this);
          RaisePropertyChanged(nameof(ComparisonValues));
        }
      }
    }

    public abstract Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc { get; }
    public virtual IEnumerable<T> CurrentValues { get; }
    public ItemsHolder ItemsHolder { get; set; }

    public ValueComparisons Comparison { get; set; }
    public virtual IEnumerable<T> ComparisonValues { get; }
    public T SelectedComparisonValue { get; set; }
    public string Description => App.Descriptions[DescriptionID];
    object IFilter.SelectedValue => SelectedValue;
    object IFilter.SelectedComparisonValue => SelectedComparisonValue;
    IEnumerable<T> IFilter<T>.CurrentValues => CurrentValues;
    IEnumerable<T> IFilter<T>.ComparisonValues => ComparisonValues;
    IEnumerable<object> IFilter.CurrentValues => CurrentValues.Cast<object>();
    IEnumerable<object> IFilter.ComparisonValues => ComparisonValues.Cast<object>();

    #endregion Properties

    #region Constructors

    public BaseFilter(ItemsHolder itemsHolder) {
      ItemsHolder = itemsHolder;
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public void UpdateUI() {
      var item = SelectedValue;
      RaisePropertyChanged(nameof(CurrentValues));
      _selectedValue = item;
      RaisePropertyChanged(nameof(SelectedValue));
    }
    public virtual void ResetFilter() {
      SelectedValue = default;
      SelectedComparisonValue = default;
    }

    #endregion Methods

    #region Fields

    private T _selectedValue;

    #endregion Fields
  }
}