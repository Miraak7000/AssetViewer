using AssetViewer.Data;
using AssetViewer.Extensions;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace AssetViewer.Controls {

  /// <summary>
  /// Interaktionslogik für Tourism.xaml
  /// </summary>
  public partial class Tourism : UserControl {

    #region Properties

    public ObservableCollection<TourismStatus> TourismStati { get; } = new ObservableCollection<TourismStatus>();

    #endregion Properties

    #region Constructors

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

    #endregion Constructors

    #region Methods

    private void Tourism_Unloaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed -= this.ComboBoxLanguage_SelectionChanged;
    }

    private void Tourism_Loaded(object sender, RoutedEventArgs e) {
      AssetProvider.OnLanguage_Changed += this.ComboBoxLanguage_SelectionChanged;
      ListBoxStatis.SelectedIndex = 0;
      lbItemsList.SelectedIndex = 0;
    }

    private void ComboBoxLanguage_SelectionChanged() {
      DataContext = null;
      DataContext = this;
    }

    #endregion Methods
  }
}