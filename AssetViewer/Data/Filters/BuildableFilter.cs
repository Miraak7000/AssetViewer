using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class BuildableFilter : BaseFilter<bool> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue) {
        return result.Where(w => (w.BuildCosts != null ? w.BuildCosts.Count : 0) > 0);
      }

      return null;
    };

    public override int DescriptionID => -1105;

    #endregion Properties

    #region Constructors

    public BuildableFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
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