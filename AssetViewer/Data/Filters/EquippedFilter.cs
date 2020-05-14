﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class EquippedFilter : BaseFilter<Description> {

    #region Public Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue != null && SelectedValue.ID != 0) {
        if (Comparison == ValueComparisons.UnEqual) {
          return result.Where(w => w.Allocation?.Text.Equals(SelectedValue) == false);
        }
        else {
          return result.Where(w => w.Allocation?.Text.Equals(SelectedValue) == true);
        }
      }

      return null;
    };

    public override int DescriptionID => -106;

    #endregion Public Properties

    #region Public Constructors

    public EquippedFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      ComparisonType = FilterType.Selection;
    }

    #endregion Public Constructors

    #region Public Methods

    public override void SetCurrenValues() {
      CurrentValues = ItemsHolder
         .GetResultWithoutFilter(this)
         .Where(s => s.Allocation?.Text != null)
         .Select(s => s.Allocation.Text)
         .Distinct()
         .Concat(new[] { new Description(0) })
         .OrderBy(o => o.CurrentLang)
         .ToList();
    }

    #endregion Public Methods
  }
}