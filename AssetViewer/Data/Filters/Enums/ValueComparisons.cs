using AssetViewer.Converter;
using System.ComponentModel;

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