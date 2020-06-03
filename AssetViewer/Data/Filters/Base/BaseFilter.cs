﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public abstract class BaseFilter<T> : IFilter<T>, INotifyPropertyChanged {

    #region Public Properties

    public FilterType FilterType { get; set; } = FilterType.Selection;
    public FilterType ComparisonType { get; set; }
    public abstract int DescriptionID { get; }

    public virtual T SelectedValue {
      get => _selectedValue;
      set {
        if (!(_selectedValue?.Equals(value) ?? false) && value != null) {
          _selectedValue = value;
          _selectedComparisonValue = ComparisonValues != null ? ComparisonValues.FirstOrDefault() : default;
          UpdateSavedItems();
          RaisePropertyChanged(nameof(SelectedComparisonValue));
          RaisePropertyChanged();
        }
      }
    }

    public T SelectedComparisonValue {
      get => _selectedComparisonValue;
      set {
        if (!(_selectedComparisonValue?.Equals(value) ?? false) && value != null) {
          _selectedComparisonValue = value;
          UpdateSavedItems();
          RaisePropertyChanged();
        }
      }
    }

    public ValueComparisons Comparison {
      get => comparison;
      set {
        if (comparison != value) {
          comparison = value;
          UpdateSavedItems();
          RaisePropertyChanged(nameof(Comparison));
        }
      }
    }

    public abstract Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc { get; }
    public virtual List<T> CurrentValues { get; set; }
    public ItemsHolder ItemsHolder { get; set; }
    public virtual List<T> ComparisonValues { get; set; }

    public List<TemplateAsset> SavedItems { get; set; }

    public string Description => AssetProvider.Descriptions[DescriptionID];

    object IFilter.SelectedValue => SelectedValue;

    object IFilter.SelectedComparisonValue => SelectedComparisonValue;

    List<T> IFilter<T>.CurrentValues => CurrentValues;

    List<T> IFilter<T>.ComparisonValues => ComparisonValues;

    List<object> IFilter.CurrentValues => CurrentValues.Cast<object>().ToList();

    List<object> IFilter.ComparisonValues => ComparisonValues.Cast<object>().ToList();

    #endregion Public Properties

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Methods

    public void UpdateSavedItems() {
      SavedItems = FilterFunc?.Invoke(ItemsHolder.Base)?.ToList();
    }

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void UpdateUI() {
      var item = SelectedValue;
      var compitem = SelectedComparisonValue;
      SetCurrenValues();
      RaisePropertyChanged(nameof(CurrentValues));
      RaisePropertyChanged(nameof(ComparisonValues));

      if (item != null) {
        if (CurrentValues != null) {
          if (item is Description desc) {
            _selectedValue = CurrentValues.Find(v => (v as Description).Equals(item as Description));
          }
          else {
            _selectedValue = CurrentValues.Find(v => v.Equals(item));
          }
        }
        else {
          _selectedValue = item;
        }
        RaisePropertyChanged(nameof(SelectedValue));
      }

      if (compitem != null) {
        if (ComparisonValues != null) {
          if (compitem is Description desc) {
            _selectedComparisonValue = ComparisonValues.Find(v => (v as Description).Equals(compitem as Description));
          }
          else {
            _selectedComparisonValue = ComparisonValues.Find(v => v.Equals(compitem));
          }
        }
        else {
          _selectedComparisonValue = compitem;
        }
        RaisePropertyChanged(nameof(SelectedComparisonValue));
      }
    }

    public virtual void ResetFilter() {
      SelectedValue = CurrentValues != null ? CurrentValues.FirstOrDefault() : default;
    }

    public virtual void SetCurrenValues() {
    }

    #endregion Public Methods

    #region Protected Constructors

    protected BaseFilter(ItemsHolder itemsHolder) {
      ItemsHolder = itemsHolder;
      SetCurrenValues();
    }

    #endregion Protected Constructors

    #region Private Fields

    private T _selectedValue;
    private T _selectedComparisonValue;
    private ValueComparisons comparison;

    #endregion Private Fields
  }
}