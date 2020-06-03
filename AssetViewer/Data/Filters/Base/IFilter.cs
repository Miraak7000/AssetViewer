﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public interface IFilter<T> : IFilter {

    #region Public Properties

    new T SelectedValue { get; set; }
    new T SelectedComparisonValue { get; set; }
    new List<T> CurrentValues { get; }
    new List<T> ComparisonValues { get; }

    #endregion Public Properties

    #region Public Methods

    new void RaisePropertyChanged([CallerMemberName] string name = "");

    #endregion Public Methods
  }

  public interface IFilter {

    #region Public Properties

    object SelectedValue { get; }
    Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc { get; }
    List<object> CurrentValues { get; }
    List<object> ComparisonValues { get; }
    string Description { get; }
    FilterType FilterType { get; set; }
    FilterType ComparisonType { get; set; }
    ValueComparisons Comparison { get; set; }
    object SelectedComparisonValue { get; }
    List<TemplateAsset> SavedItems { get; set; }

    #endregion Public Properties

    #region Public Methods

    void RaisePropertyChanged(string name);

    void UpdateUI();

    void ResetFilter();

    void SetCurrenValues();

    #endregion Public Methods
  }
}