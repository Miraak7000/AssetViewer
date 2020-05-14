using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using AssetViewer.Data;

namespace AssetViewer {

  [TemplatePart(Name = nameof(PART_TextBox), Type = typeof(TextBox))]
  [TemplatePart(Name = "PART_ButtonUp", Type = typeof(ButtonBase))]
  [TemplatePart(Name = "PART_ButtonDown", Type = typeof(ButtonBase))]
  public class NumericUpDown : Control {

    #region Public Properties

    public uint MaxValue {
      get => (uint)GetValue(MaxValueProperty);
      set => SetValue(MaxValueProperty, value);
    }

    public uint MinValue {
      get => (uint)GetValue(MinValueProperty);
      set => SetValue(MinValueProperty, value);
    }

    public uint Increment {
      get => (uint)GetValue(IncrementProperty);
      set => SetValue(IncrementProperty, value);
    }

    public uint Value {
      get => (uint)GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    public ICommand Command {
      get => (ICommand)GetValue(CommandProperty);
      set => SetValue(CommandProperty, value);
    }

    public object CommandParameter {
      get => GetValue(CommandParameterProperty);
      set => SetValue(CommandParameterProperty, value);
    }

    #endregion Public Properties

    #region Public Events

    public event ValueChangedEventHandler ValueChanged {
      add { base.AddHandler(NumericUpDown.ValueChangedEvent, value); }
      remove { base.RemoveHandler(NumericUpDown.ValueChangedEvent, value); }
    }

    #endregion Public Events

    #region Public Constructors

    static NumericUpDown() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
    }

    #endregion Public Constructors

    #region Public Methods

    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      if (GetTemplateChild(nameof(PART_TextBox)) is TextBox textBox) {
        PART_TextBox = textBox;
        PART_TextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
        PART_TextBox.TextChanged += TextBox_TextChanged;
        PART_TextBox.LostFocus += TextBox_LostFocus;
        PART_TextBox.Text = Value.ToString();
      }
      if (GetTemplateChild("PART_ButtonUp") is ButtonBase PART_ButtonUp) {
        PART_ButtonUp.Click += ButtonUp_Click;
      }
      if (GetTemplateChild("PART_ButtonDown") is ButtonBase PART_ButtonDown) {
        PART_ButtonDown.Click += ButtonDown_Click;
      }
    }

    #endregion Public Methods

    #region Public Fields

    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
                                    nameof(ValueChanged), RoutingStrategy.Direct,
        typeof(ValueChangedEventHandler), typeof(NumericUpDown));

    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register(nameof(MaxValue), typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(uint.MaxValue, MaxValueChangedCallback, CoerceMaxValueCallback));

    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register(nameof(MinValue), typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(uint.MinValue, MinValueChangedCallback, CoerceMinValueCallback));

    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register(nameof(Increment), typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(1U, null, CoerceIncrementCallback));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(0U, ValueChangedCallback, CoerceValueCallback), ValidateValueCallback);

    public static readonly DependencyProperty CommandProperty =
      DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(NumericUpDown), new FrameworkPropertyMetadata((ICommand)null, new PropertyChangedCallback(OnCommandChanged)));

    public static readonly DependencyProperty CommandParameterProperty =
      DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(NumericUpDown), new FrameworkPropertyMetadata((object)null));

    #endregion Public Fields

    #region Private Methods

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    }

    private static object CoerceMaxValueCallback(DependencyObject d, object value) {
      var minValue = ((NumericUpDown)d).MinValue;
      if ((uint)value < minValue)
        return minValue;

      return value;
    }

    private static void MaxValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var numericUpDown = ((NumericUpDown)d);
      numericUpDown.CoerceValue(MinValueProperty);
      numericUpDown.CoerceValue(ValueProperty);
    }

    private static object CoerceMinValueCallback(DependencyObject d, object value) {
      var maxValue = ((NumericUpDown)d).MaxValue;
      if ((uint)value > maxValue)
        return maxValue;

      return value;
    }

    private static void MinValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var numericUpDown = ((NumericUpDown)d);
      numericUpDown.CoerceValue(NumericUpDown.MaxValueProperty);
      numericUpDown.CoerceValue(NumericUpDown.ValueProperty);
    }

    private static object CoerceIncrementCallback(DependencyObject d, object value) {
      var numericUpDown = ((NumericUpDown)d);
      var i = numericUpDown.MaxValue - numericUpDown.MinValue;
      if ((uint)value > i)
        return i;

      return value;
    }

    private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var numericUpDown = (NumericUpDown)d;
      var control = d as NumericUpDown;

      var ea = new ValueChangedEventArgs(NumericUpDown.ValueChangedEvent, d, (uint)e.OldValue, (uint)e.NewValue);
      numericUpDown.RaiseEvent(ea);
      //if (ea.Handled) numericUpDown.Value = (uint)e.OldValue;
      //else
      numericUpDown.PART_TextBox.Text = e.NewValue.ToString();
      var command = control?.Command;
      var args = new SelectedCountChangedArgs { Count = (uint)e.NewValue, Assets = (control?.CommandParameter as IList)?.OfType<TemplateAsset>() };
      if (command?.CanExecute(args) == true)
        command.Execute(args);
    }

    private static bool ValidateValueCallback(object value) {
      var val = (uint)value;
      return val >= uint.MinValue && val <= uint.MaxValue;
    }

    private static object CoerceValueCallback(DependencyObject d, object value) {
      var val = (uint)value;
      var minValue = ((NumericUpDown)d).MinValue;
      var maxValue = ((NumericUpDown)d).MaxValue;
      uint result;
      if (val < minValue)
        result = minValue;
      else if (val > maxValue)
        result = maxValue;
      else
        result = (uint)value;

      return result;
    }

    private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
      if (e.Key == Key.Space)
        e.Handled = true;
    }

    private void ButtonUp_Click(object sender, RoutedEventArgs e) {
      Value += Increment;
    }

    private void ButtonDown_Click(object sender, RoutedEventArgs e) {
      Value -= Increment;
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
      var index = PART_TextBox.CaretIndex;
      if (!uint.TryParse(PART_TextBox.Text, out var result)) {
        var changes = e.Changes.FirstOrDefault();
        PART_TextBox.Text = PART_TextBox.Text.Remove(changes.Offset, changes.AddedLength);
        PART_TextBox.CaretIndex = index > 0 ? index - changes.AddedLength : 0;
      }
      else if (result > MaxValue) {
        Value = MaxValue;
        PART_TextBox.Text = Value.ToString();
        PART_TextBox.CaretIndex = PART_TextBox.Text.Length;
      }
      else if (result < MinValue) {
        Value = MinValue;
        PART_TextBox.Text = Value.ToString();
        PART_TextBox.CaretIndex = PART_TextBox.Text.Length;
      }
      else if (result <= MaxValue && result >= MinValue) {
        Value = result;
      }
      else {
        PART_TextBox.Text = Value.ToString();
        PART_TextBox.CaretIndex = index > 0 ? index - 1 : 0;
      }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
      if (string.IsNullOrEmpty(PART_TextBox.Text)) {
        PART_TextBox.Text = "0";
      }
    }

    #endregion Private Methods

    #region Private Fields

    private TextBox PART_TextBox = new TextBox();

    #endregion Private Fields
  }
}