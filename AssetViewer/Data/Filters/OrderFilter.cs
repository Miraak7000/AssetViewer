using AssetViewer.Comparer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class OrderFilter : BaseFilter<Description> {

    #region Properties

    public override Func<IEnumerable<TemplateAsset>, IEnumerable<TemplateAsset>> FilterFunc => result => {
      if (SelectedValue != null && SelectedValue.ID != 0) {
        if (SelectedValue.ID == -1200) {
          result = result.OrderBy(w => w.Text.CurrentLang);
        }
        else if (SelectedValue.ID == -1201) {
          result = result.OrderBy(w => w.ID);
        }
        else if (SelectedValue.ID == -1023) {
          result = result.OrderBy(w => w.Rarity.ID, RarityComparer.Default);
        }
        else if (SelectedValue.ID == 12725) {
          result = result.OrderBy(w => string.IsNullOrWhiteSpace(w.TradePrice) ? 0 : Int32.Parse(w.TradePrice ?? "0", NumberStyles.Any));
        }    
        else if (SelectedValue.ID == 22440) {
          result = result.OrderByDescending(w => w.Count);
        }
      }

      return result;
    };

    public override int DescriptionID => -1103;

    public override List<Description> CurrentValues {
      get {
        return currentValues;
      }
      set {
        if (currentValues != value) {
          currentValues = value;
          RaisePropertyChanged();
        }
      }
    }

    public override Description SelectedValue {
      get {
        return _selectedValue;
      }
      set {
        if (!(_selectedValue?.Equals(value) ?? false)) {
          _selectedValue = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion Properties

    #region Constructors

    public OrderFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      if (AssetProvider.CountMode) {
        values.Add(new Description(22440));
      }
      CurrentValues = values;
      SelectedValue = values.FirstOrDefault();
    }

    #endregion Constructors

    #region Methods

    public override void ResetFilter() {
      CurrentValues = null;
      CurrentValues = values;
      SelectedValue = null;
      base.ResetFilter();
    }

    #endregion Methods

    #region Fields

    private readonly List<Description> values = new List<Description>{
      new Description(-1200),
      new Description(-1201),
      new Description(-1023),
      new Description(12725)
    };
    private Description _selectedValue;
    private List<Description> currentValues;

    #endregion Fields
  }
}