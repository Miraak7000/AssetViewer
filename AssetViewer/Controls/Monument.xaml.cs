using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml.XPath;
using AssetViewer.Library;
using AssetViewer.Templates;
using AssetViewer.Veras;

namespace AssetViewer.Controls {
    [SuppressMessage("ReSharper", "PossibleNullReferenceException"), SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public partial class Monument : UserControl, INotifyPropertyChanged {
        #region Properties
        public IEnumerable<TemplateAsset> Categories {
            get { return this.AssetCategory.AsEnumerable(); }
        }

        public IEnumerable<TemplateAsset> Events {
            get {
                if (!(this.ComboBoxCategories.SelectedItem is TemplateAsset monumentCategory))
                    return new TemplateAsset[0];
                return this.AssetEvent.Where(w => monumentCategory.MonumentEvents.Contains(w.ID));
            }
        }

        public IEnumerable<TemplateAsset> Thresholds {
            get {
                if (!(this.ComboBoxEvents.SelectedItem is TemplateAsset monumentEvent))
                    return new TemplateAsset[0];
                return this.AssetThreshold.Where(w => monumentEvent.MonumentThresholds.Contains(w.ID));
            }
        }

        public IEnumerable<TemplateAsset> Rewards {
            get {
                if (!(this.ComboBoxThresholds.SelectedItem is TemplateAsset monumentThreshold))
                    return new TemplateAsset[0];
                return monumentThreshold.MonumentRewards.GetItemsById();
            }
        }

        public TemplateAsset SelectedAsset { get; set; }

        public LinearGradientBrush RarityBrush {
            get {
                var selection = this.SelectedAsset?.Rarity?.EN ?? "Common";
                switch (selection) {
                    case "Uncommon":
                        return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(65, 89, 41), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
                    case "Rare":
                        return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(50, 60, 83), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
                    case "Epic":
                        return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(90, 65, 89), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
                    case "Legendary":
                        return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(98, 66, 46), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
                    default:
                        return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(126, 128, 125), 0),
              new GradientStop(Color.FromRgb(42, 44, 39), 0.2),
              new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
                }
            }
        }

        public Boolean HasResult {
            get { return this.Rewards.Any(); }
        }

        public String AllocationText {
            get {
                switch (App.Language) {
                    case Languages.German:
                        return "Hier ausgerüstet";
                    default:
                        return "Equipped in";
                }
            }
        }

        public String ExpeditionText {
            get {
                switch (App.Language) {
                    case Languages.German:
                        return "Expeditions-Bonus";
                    default:
                        return "Expedition Bonus";
                }
            }
        }

        public String TradeText {
            get {
                switch (App.Language) {
                    case Languages.German:
                        return "Verkaufspreis";
                    default:
                        return "Selling Price";
                }
            }
        }

        public String ItemSetText {
            get {
                switch (App.Language) {
                    case Languages.German:
                        return "Teil eines Sets";
                    default:
                        return "Part of set";
                }
            }
        }
        #endregion

        #region Fields
        private readonly List<TemplateAsset> AssetCategory;
        private readonly List<TemplateAsset> AssetEvent;
        private readonly List<TemplateAsset> AssetThreshold;
        //private readonly List<TemplateAsset> AssetReward;
        #endregion

        #region Constructor
        public Monument() {
            this.InitializeComponent();
            ((MainWindow)Application.Current.MainWindow).ComboBoxLanguage.SelectionChanged += this.ComboBoxLanguage_SelectionChanged;
            this.AssetCategory = new List<TemplateAsset>();
            this.AssetEvent = new List<TemplateAsset>();
            this.AssetThreshold = new List<TemplateAsset>();
            //this.AssetReward = new List<TemplateAsset>();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentCategory.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    this.AssetCategory.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentEvent.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    this.AssetEvent.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentThreshold.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    this.AssetThreshold.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
                }
            }
            //using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.MonumentReward.xml")) {
            //    using (var reader = new StreamReader(stream)) {
            //        var document = XDocument.Parse(reader.ReadToEnd()).Root;
            //        this.AssetReward.AddRange(document.Elements().Select(s => new TemplateAsset(s)));
            //    }
            //}
            this.DataContext = this;
        }
        #endregion

        #region Private Methods
        private void Monument_OnLoaded(Object sender, RoutedEventArgs e) {
            this.ComboBoxCategories.SelectedIndex = 0;
        }

        private void ComboBoxLanguage_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
            this.ComboBoxCategories.SelectedItem = null;
            this.ComboBoxEvents.SelectedItem = null;
            this.ComboBoxThresholds.SelectedItem = null;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Categories"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Events"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Thresholds"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rewards"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllocationText"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpeditionText"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TradeText"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedAsset"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RarityBrush"));
            this.ComboBoxCategories.SelectedIndex = 0;
        }

        private void ComboBoxCategories_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Events"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
            this.ComboBoxEvents.SelectedIndex = 0;
        }

        private void ComboBoxEvents_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Thresholds"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
            this.ComboBoxThresholds.SelectedIndex = 0;
        }

        private void ComboBoxThresholds_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rewards"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasResult"));
            this.ListBoxItems.SelectedIndex = 0;
        }

        private void ListBoxItems_OnSelectionChanged(Object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count == 0)
                this.ListBoxItems.SelectedIndex = 0;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedAsset"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RarityBrush"));
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}