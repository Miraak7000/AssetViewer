using System;

namespace AssetViewer.Data {

  public class DummyDescription : Description, IEquatable<DummyDescription> {

    #region Properties

    public override string CurrentLang => Value;

    public string Value { get; set; }

    public override int ID => GetHashCode();

    #endregion Properties

    #region Constructors

    public DummyDescription(string value) {
      Value = value;
    }

    #endregion Constructors

    #region Methods

    public bool Equals(DummyDescription other) {
      return Value.Equals(other?.Value);
    }

    public override int GetHashCode() {
      return Value.GetHashCode();
    }

    #endregion Methods
  }
}