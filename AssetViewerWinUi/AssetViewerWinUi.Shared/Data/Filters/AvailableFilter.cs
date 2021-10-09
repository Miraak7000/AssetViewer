using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class AvailableFilter : BaseFilter<bool> {

    #region Public Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue) {
        return result.Where(w => (w.Sources != null ? w.Sources.Count : 0) > 0);
      }

      return null;
    };

    public override int DescriptionID => -1101;

    #endregion Public Properties

    #region Public Constructors

    public AvailableFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      FilterType = FilterType.Bool;
    }

    #endregion Public Constructors

    #region Public Methods

    public override void ResetFilter() {
      SelectedValue = true;
    }

    #endregion Public Methods
  }
}