using AssetViewer.Data.Filters;
using System.Windows.Controls.Primitives;

namespace AssetViewer.Controls {

  public partial class GuildhouseItem : ItemsBase {

    #region Properties

    public override ItemsHolder ItemsHolder { get; } = new ItemsHolderGuildhouse();

    #endregion Properties

    #region Constructors

    public GuildhouseItem() : base() {
      this.InitializeComponent();
    }

        #endregion Constructors

    }
}