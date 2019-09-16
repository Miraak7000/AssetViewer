using AssetViewer.Comparer;
using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AssetViewer.Data.Filters {

  public class OrderFilter : BaseFilter<string> {

    #region Properties

    public override Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc => result => {
      if (!String.IsNullOrEmpty(SelectedValue)) {
        if (SelectedValue == App.Descriptions["-1200"]) {
          result = result.OrderBy(w => w.Text.CurrentLang);
        }
        else if (SelectedValue == App.Descriptions["-1201"]) {
          result = result.OrderBy(w => int.Parse(w.ID));
        }
        else if (SelectedValue == App.Descriptions["-1023"]) {
          result = result.OrderBy(w => w.Rarity.CurrentLang, RarityComparer.Default);
        }
        else if (SelectedValue == App.Descriptions["-1202"]) {
          result = result.OrderBy(w => string.IsNullOrWhiteSpace(w.TradePrice) ? 0 : Int32.Parse(w.TradePrice ?? "0", NumberStyles.Any));
        }
      }

      return result;
    };

    public override IEnumerable<String> CurrentValues => values.Select(d => d.CurrentLang);

    public override string DescriptionID => "-1103";

    #endregion Properties

    #region Constructors

    public OrderFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      SelectedValue = values[0].CurrentLang;
    }

    #endregion Constructors

    #region Fields

    private readonly Description[] values = new[] {
      new Description("-1200"),
      new Description("-1201"),
      new Description("-1023"),
      new Description("-1202")
    };

    #endregion Fields
  }
}