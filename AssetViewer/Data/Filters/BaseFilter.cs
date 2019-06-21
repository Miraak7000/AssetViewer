using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AssetViewer.Data.Filters {

  public abstract class BaseFilter : IFilter, INotifyPropertyChanged {
    private string _selectedValue;

    #region Properties

    public abstract int DescriptionID { get; }

    public virtual string SelectedValue {
      get { return _selectedValue; }
      set {
        if (_selectedValue != value) {
          _selectedValue = value;
          RaisePropertyChanged();
        }
      }
    }

    public abstract Func<IQueryable<TemplateAsset>, IQueryable<TemplateAsset>> FilterFunc { get; }

    public abstract IEnumerable<string> CurrentValues { get; }

    public ItemsHolder ItemsHolder { get; set; }

    public bool IsChecked { get; set; }

    public Description Description => new Description(App.Descriptions[DescriptionID].ShortEN, App.Descriptions[DescriptionID].ShortDE);

    #endregion Properties

    #region Constructors

    public BaseFilter(ItemsHolder itemsHolder) {
      ItemsHolder = itemsHolder;
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
  }
}