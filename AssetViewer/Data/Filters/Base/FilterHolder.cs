using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public class FilterHolder : INotifyPropertyChanged {

    #region Properties

    public IFilter SelectedFilter {
      get {
        return _selectedFilter;
      }
      set {
        if (_selectedFilter != value) {
          _selectedFilter = value;
          RaisePropertyChanged(nameof(SelectedFilter));
        }
      }
    }

    public Collection<IFilter> Filters { get; } = new Collection<IFilter>();

    #endregion Properties

    #region Constructors

    public FilterHolder() {
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion Methods

    #region Fields

    private IFilter _selectedFilter;

    #endregion Fields
  }
}