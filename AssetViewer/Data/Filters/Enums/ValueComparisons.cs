using AssetViewer.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetViewer {
  [TypeConverter(typeof(EnumDescriptionTypeConverter))]
  public enum ValueComparisons {
    [Description("=")]
    Equals,
    [Description("<")]
    LesserThan,
    [Description(">")]
    GraterThan
  }
}
