using System;

namespace RDA.Data {

  public class CultureItem {

    #region Public Properties

    public String ID { get; set; }
    public String Name { get; set; }
    public Description Text { get; set; }
    public Description Info { get; set; }
    public Description Allocation { get; set; }
    public Description Rarity { get; set; }
    public String TradePrice { get; set; }
    public ItemSet ItemSet { get; set; }

    #endregion Public Properties
  }
}