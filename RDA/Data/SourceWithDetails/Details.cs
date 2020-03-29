using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RDA.Data {
  public class Details : IEnumerable<XElement> {
    #region Properties

    public List<String> PreviousIDs { get; set; } = new List<string>();
    public HashSet<XElement> Items { get; set; } = new HashSet<XElement>();

    #endregion Properties

    #region Constructors

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

    #endregion Constructors

    #region Methods

    public void Add(XElement item) {
      Items.Add(item);
    }

    public IEnumerator<XElement> GetEnumerator() {
      return ((IEnumerable<XElement>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable<XElement>)Items).GetEnumerator();
    }

    #endregion Methods
  }
}