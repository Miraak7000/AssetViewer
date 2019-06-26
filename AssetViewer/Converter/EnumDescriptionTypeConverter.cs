using System;
using System.ComponentModel;

namespace AssetViewer.Converter {

  public class EnumDescriptionTypeConverter : EnumConverter {

    #region Constructors

    public EnumDescriptionTypeConverter(Type type)
        : base(type) {
    }

    #endregion Constructors

    #region Methods

    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
      if (destinationType == typeof(string)) {
        if (value != null) {
          var fi = value.GetType().GetField(value.ToString());
          if (fi != null) {
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
          }
        }

        return string.Empty;
      }

      return base.ConvertTo(context, culture, value, destinationType);
    }

    #endregion Methods
  }
}