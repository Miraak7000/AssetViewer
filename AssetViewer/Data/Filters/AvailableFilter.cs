using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class AvailableFilter : BaseFilter<bool> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue) {
        return result.Where(w => (w.Sources != null ? w.Sources.Count : 0) > 0);
      }

      return null;
    };

    public override int DescriptionID => -1101;

    #endregion Properties

    #region Constructors

    public AvailableFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      FilterType = FilterType.Bool;
    }

    #endregion Constructors

    #region Methods

    public override void ResetFilter() {
      SelectedValue = true;
    }

    #endregion Methods
  }
}