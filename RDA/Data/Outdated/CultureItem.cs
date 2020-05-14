namespace RDA.Data {

  public class CultureItem {

    #region Public Properties

    public string ID { get; set; }
    public string Name { get; set; }
    public Description Text { get; set; }
    public Description Info { get; set; }
    public Description Allocation { get; set; }
    public Description Rarity { get; set; }
    public string TradePrice { get; set; }
    public ItemSet ItemSet { get; set; }

    #endregion Public Properties
  }
}