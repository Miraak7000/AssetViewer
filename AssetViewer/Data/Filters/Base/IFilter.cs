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
    object SelectedValue { get; }
    Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc { get; }
    IEnumerable<object> CurrentValues { get; }
    IEnumerable<object> ComparisonValues { get; }
    string Description { get; }
    FilterType FilterType { get; set; }
    FilterType ComparisonType { get; set; }
    ValueComparisons Comparison { get; set; }
    object SelectedComparisonValue { get; }

    void RaisePropertyChanged(string name);
    void UpdateUI();
    void ResetFilter();
  }
}