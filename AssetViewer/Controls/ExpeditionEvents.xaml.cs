using AssetViewer.Templates;
using AssetViewer.Veras;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace AssetViewer.Controls {
    /// <summary>
    /// Interaktionslogik für ExpeditionEvents.xaml
    /// </summary>
    public partial class ExpeditionEvents : UserControl , INotifyPropertyChanged{

public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string name = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ExpeditionEvents() {
            Loaded += ExpeditionEvents_Loaded;
            Unloaded += ExpeditionEvents_Unloaded;
            InitializeComponent();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ExpeditionEvents.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s =>s.FromXElement<ExpeditionEvent>())) {
                        Events.Add(item);
                    }

                }
            }

            DataContext = this;
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

        public List<ExpeditionEvent> Events { get; } = new List<ExpeditionEvent>();
    }
}
