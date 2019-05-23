using AssetViewer.Templates;
using AssetViewer.Veras;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public partial class ExpeditionEvents : UserControl {
        public ExpeditionEvents() {
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

        public List<ExpeditionEvent> Events { get; } = new List<ExpeditionEvent>();
    }
}
