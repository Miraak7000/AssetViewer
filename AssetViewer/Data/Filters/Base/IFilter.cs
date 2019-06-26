using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public interface IFilter<T> : IFilter {

    #region Properties

    new T SelectedValue { get; set; }
    new T SelectedComparisonValue { get; set; }
    new IEnumerable<T> CurrentValues { get; }
    new IEnumerable<T> ComparisonValues { get; }

    #endregion Properties
  }

  public interface IFilter {

    #region Properties

    object SelectedValue { get; }
    Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc { get; }
    IEnumerable<object> CurrentValues { get; }
    IEnumerable<object> ComparisonValues { get; }
    Description Description { get; }
    FilterType FilterType { get; set; }
    FilterType ComparisonType { get; set; }
    ValueComparisons Comparison { get; set; }
    object SelectedComparisonValue { get; }

    #endregion Properties

    #region Methods

    void RaisePropertyChanged(string name);
    void UpdateUI();
    void ResetFilter();

    #endregion Methods
  }
}