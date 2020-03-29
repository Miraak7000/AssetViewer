using AssetViewer.Data;
using AssetViewer.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace AssetViewer.Controls {

  public partial class ExpeditionEvents : UserControl, INotifyPropertyChanged {

    #region Public Properties

    public List<ExpeditionEvent> Events { get; } = new List<ExpeditionEvent>();

    #endregion Public Properties

    #region Public Constructors

    public ExpeditionEvents() {
      Loaded += ExpeditionEvents_Loaded;
      Unloaded += ExpeditionEvents_Unloaded;
      InitializeComponent();
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ExpeditionEvents.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          foreach (var item in document.Elements().Select(s => s.FromXElement<ExpeditionEvent>())) {
            Events.Add(item);
          }
        }
      }

      DataContext = this;
    }

    #endregion Public Constructors

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Methods

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion Public Methods

    #region Private Methods

    private void ExpeditionEvents_Unloaded(object sender, RoutedEventArgs e) {
      if (Application.Current.MainWindow is MainWindow main) {
        main.OnLanguage_Changed -= this.ComboBoxLanguage_SelectionChanged;
      }
    }

    private void ExpeditionEvents_Loaded(object sender, RoutedEventArgs e) {
      ((MainWindow)Application.Current.MainWindow).OnLanguage_Changed += this.ComboBoxLanguage_SelectionChanged;
    }

    private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      DataContext = null;
      DataContext = this;
      (this.FindResource("EventSource") as CollectionViewSource)?.View.Refresh();
    }

    #endregion Private Methods
  }
}