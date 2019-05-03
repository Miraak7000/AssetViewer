using System;

namespace AssetViewer.Library {

  public class Description {

    #region Properties
    public String ShortEN { get; set; }
    public String LongEN { get; set; }
    public String ShortDE { get; set; }
    public String LongDE { get; set; }
    public String Short {
      get { return App.Language == Languages.English ? this.ShortEN : this.ShortDE; }
    }
    public String Long {
      get { return App.Language == Languages.English ? this.LongEN : this.LongDE; }
    }
    #endregion

    #region Constructor
    public Description() {
    }
    public Description(String shortEN, String shortDE) {
      this.ShortEN = shortEN;
      this.ShortDE = shortDE;
    }
    public Description(String shortEN, String longEN, String shortDE, String longDE) {
      this.ShortEN = shortEN;
      this.LongEN = longEN;
      this.ShortDE = shortDE;
      this.LongDE = longDE;
    }
    #endregion

  }

}