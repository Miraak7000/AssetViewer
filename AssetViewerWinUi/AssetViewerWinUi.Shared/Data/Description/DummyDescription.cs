using System;

namespace AssetViewer.Data {

  public class DummyDescription : Description, IEquatable<DummyDescription> {

    #region Public Properties

    public override string CurrentLang => Value;

    public string Value { get; set; }

    public override int ID => GetHashCode();

    #endregion Public Properties

    #region Public Constructors

    public DummyDescription(string value) {
      Value = value;
    }

    #endregion Public Constructors

    #region Public Methods

    public bool Equals(DummyDescription other) {
      return Value.Equals(other?.Value);
    }

    public override int GetHashCode() {
      return Value.GetHashCode();
    }

    #endregion Public Methods
  }
}