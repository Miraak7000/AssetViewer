using AssetViewer.Data;
using AssetViewer.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  /// Interaktionslogik für Tourism.xaml
  /// </summary>
  public partial class Tourism : UserControl {
    public Tourism() {
      InitializeComponent();
      Loaded += Tourism_Loaded;
      Unloaded += Tourism_Unloaded;
      using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.Tourism.xml")) {
        using (var reader = new StreamReader(stream)) {
          var document = XDocument.Parse(reader.ReadToEnd()).Root;
          foreach (var item in document.Elements().Select(s => s.FromXElement<TourismStatus>())) {
            TourismStati.Add(item);
          }
        }
      }
      DataContext = this;
    }
    public ObservableCollection<TourismStatus> TourismStati { get; } = new ObservableCollection<TourismStatus>();
    private void Tourism_Unloaded(object sender, RoutedEventArgs e) {
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged -= this.ComboBoxLanguage_SelectionChanged;

    }

    private void Tourism_Loaded(object sender, RoutedEventArgs e) {
      ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;

    }
    private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e) {
     DataContext = null;
     DataContext = this;
    }
  }
}
