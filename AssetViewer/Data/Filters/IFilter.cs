using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public interface IFilter {

    #region Properties

    string SelectedValue { get; set; }
    Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc { get; }
    IEnumerable<String> CurrentValues { get; }
    Description Description { get; }
    void RaisePropertyChanged(string name);
    #endregion Properties
  }
}