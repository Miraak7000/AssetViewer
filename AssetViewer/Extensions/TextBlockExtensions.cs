using AssetViewer.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AssetViewer {

  public class TextBlockExtensions : DependencyObject {

    #region Fields

    public static readonly DependencyProperty DescFontStyle =
        DependencyProperty.RegisterAttached(nameof(DescFontStyle), typeof(DescriptionFontStyle), typeof(TextBlockExtensions), new FrameworkPropertyMetadata(default(DescriptionFontStyle), OnFontStyleChanged));

    #endregion Fields

    #region Methods

    public static DescriptionFontStyle GetDescFontStyle(DependencyObject obj) {
      return (DescriptionFontStyle)obj.GetValue(DescFontStyle);
    }

    public static void SetDescFontStyle(DependencyObject obj, int value) {
      obj.SetValue(DescFontStyle, value);
    }
    private static void OnFontStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (d is TextBlock textBlock) {
        if (e.NewValue is DescriptionFontStyle style) {
          if (style.HasFlag(Data.DescriptionFontStyle.Regular))
            textBlock.FontStyle = FontStyles.Normal;
          if (style.HasFlag(Data.DescriptionFontStyle.Italic))
            textBlock.FontStyle = FontStyles.Italic;
          if (style.HasFlag(Data.DescriptionFontStyle.Bold))
            textBlock.FontWeight = FontWeights.Bold;
          if (style.HasFlag(Data.DescriptionFontStyle.Underline))
            textBlock.TextDecorations = TextDecorations.Underline;
          if (style.HasFlag(Data.DescriptionFontStyle.Strikeout))
            textBlock.TextDecorations = TextDecorations.Strikethrough;
          if (style.HasFlag(Data.DescriptionFontStyle.Light))
            textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA49370"));
        }
      }
    }

    #endregion Methods
  }
}