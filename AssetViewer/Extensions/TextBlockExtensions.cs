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
          if ((style & Data.DescriptionFontStyle.Regular) != 0)
            textBlock.FontStyle = FontStyles.Normal;
          if ((style & Data.DescriptionFontStyle.Italic) != 0)
            textBlock.FontStyle = FontStyles.Italic;
          if ((style & Data.DescriptionFontStyle.Bold) != 0)
            textBlock.FontWeight = FontWeights.Bold;
          if ((style & Data.DescriptionFontStyle.Underline) != 0)
            textBlock.TextDecorations = TextDecorations.Underline;
          if ((style & Data.DescriptionFontStyle.Strikeout) != 0)
            textBlock.TextDecorations = TextDecorations.Strikethrough;
          if ((style & Data.DescriptionFontStyle.Light) != 0)
            textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA49370"));
        }
      }
    }

    #endregion Methods
  }
}