using AssetViewer.Veras;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace AssetViewer.Controls {
    /// <summary>
    /// Interaktionslogik für ExpeditionEvents.xaml
    /// </summary>
    public partial class ExpeditionEvents : UserControl, INotifyPropertyChanged {
        #region Constructors

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

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public List<ExpeditionEvent> Events { get; } = new List<ExpeditionEvent>();

        #endregion Properties

        #region Methods

        public void RaisePropertyChanged([CallerMemberName]string name = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ExpeditionEvents_Unloaded(object sender, RoutedEventArgs e) {
            ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged -= this.ComboBoxLanguage_SelectionChanged;
        }

        private void ExpeditionEvents_Loaded(object sender, RoutedEventArgs e) {
            ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            DataContext = null;
            DataContext = this;
        }

        #endregion Methods
    }
}