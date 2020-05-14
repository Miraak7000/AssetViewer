using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RDA.Data {

  public class Details : IEnumerable<XElement> {

    #region Public Properties

    public List<string> PreviousIDs { get; set; } = new List<string>();
    public HashSet<XElement> Items { get; set; } = new HashSet<XElement>();

    #endregion Public Properties

    #region Public Constructors

    public Details() {
    }

    public Details(Details details, params XElement[] toAdd) : this(details.Items, details.PreviousIDs, toAdd) {
    }

    public Details(HashSet<XElement> items, List<string> previousIDs = null, params XElement[] toAdd) {
      if (items != null) {
        foreach (var item in items) {
          Items.Add(item);
        }
      }
      if (previousIDs != null) {
        foreach (var item in previousIDs) {
          PreviousIDs.Add(item);
        }
      }
      foreach (var item in toAdd) {
        Items.Add(item);
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public void Add(XElement item) {
      Items.Add(item);
    }

    public IEnumerator<XElement> GetEnumerator() {
      return ((IEnumerable<XElement>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable<XElement>)Items).GetEnumerator();
    }

    #endregion Public Methods
  }
}