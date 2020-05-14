using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public class FilterHolder : INotifyPropertyChanged {

    #region Public Properties

    public IFilter SelectedFilter {
      get => _selectedFilter;
      set {
        if (_selectedFilter != value) {
          _selectedFilter = value;
          RaisePropertyChanged(nameof(SelectedFilter));
        }
      }
    }

    public Collection<IFilter> Filters { get; } = new Collection<IFilter>();

    #endregion Public Properties

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Constructors

    public FilterHolder() {
    }

    #endregion Public Constructors

    #region Public Methods

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    #endregion Public Methods

    #region Private Fields

    private IFilter _selectedFilter;

    #endregion Private Fields
  }
}