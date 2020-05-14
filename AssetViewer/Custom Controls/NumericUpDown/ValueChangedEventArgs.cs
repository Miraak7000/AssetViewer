using System.Windows;

namespace AssetViewer {

  public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

  public class ValueChangedEventArgs : RoutedEventArgs {

    #region Public Properties

    public double OldValue { get; private set; }

    public double NewValue { get; private set; }

    #endregion Public Properties

    #region Public Constructors

    public ValueChangedEventArgs(RoutedEvent routedEvent, object source, double oldValue, double newValue)
                : base(routedEvent, source) {
      OldValue = oldValue;
      NewValue = newValue;
    }

    #endregion Public Constructors
  }
}