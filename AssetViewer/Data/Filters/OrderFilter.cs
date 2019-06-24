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
      if (!String.IsNullOrEmpty(SelectedValue))
        switch (values.FirstOrDefault(v => v.DE == SelectedValue || v.EN == SelectedValue).EN) {
          case "Name":
            result = result.OrderBy(w => w.Text.CurrentLang);
            break;

          case "ID":
            result = result.OrderBy(w => int.Parse(w.ID));
            break;

          case "Rarity":
            result = result.OrderBy(w => w.Rarity.CurrentLang, RarityComparer.Default);
            break;

          case "Selling Price":
            result = result.OrderBy(w => string.IsNullOrWhiteSpace(w.TradePrice) ? 0 : Int32.Parse(w.TradePrice ?? "0", NumberStyles.Any));
            break;
        }

      return result;
    };

    public override IEnumerable<String> CurrentValues => values.Select(d => d.CurrentLang);

    public override int DescriptionID => 1103;

    #endregion Properties

    #region Constructors

    public OrderFilter(ItemsHolder itemsHolder) : base(itemsHolder) {
      SelectedValue = values[0].CurrentLang;
    }

    #endregion Constructors

    #region Fields

    private Description[] values = new[] {
      new Description("Name", "Name"),
      new Description("ID", "ID"),
      new Description("Rarity", "Seltenheit"),
      new Description("Selling Price", "Verkaufspreis")
    };

    #endregion Fields
  }
}