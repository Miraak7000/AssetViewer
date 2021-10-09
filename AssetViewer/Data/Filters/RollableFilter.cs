using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class RollableFilter : BaseFilter<bool> {

    #region Public Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue) {
        return result.Where(w => w.IsRollable);
      }

      return null;
    };

    public override int DescriptionID => -1106;

    #endregion Public Properties

    #region Public Constructors

    public RollableFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
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