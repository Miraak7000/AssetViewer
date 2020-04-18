using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssetViewer {
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
    public class ValueChangedEventArgs : RoutedEventArgs
    {
        public ValueChangedEventArgs(RoutedEvent routedEvent, object source, double oldValue, double newValue)
            : base(routedEvent, source)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        public double OldValue { get; private set; }
        public double NewValue { get; private set; }
    }
}
