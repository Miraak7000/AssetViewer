using System.ComponentModel;
using AssetViewer.Converter;

namespace AssetViewer {

  [TypeConverter(typeof(EnumDescriptionTypeConverter))]
  public enum ValueComparisons {

    [Description("=")]
    Equals,

    [Description("<")]
    LesserThan,

    [Description(">")]
    GraterThan,

    [Description("≠")]
    UnEqual,
  }
}