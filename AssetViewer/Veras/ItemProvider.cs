using AssetViewer.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AssetViewer.Veras {
    public static class ItemProvider {
        public static Dictionary<string, TemplateAsset> Items { get; } = new Dictionary<string, TemplateAsset>();
        public static Dictionary<string, TemplateAsset> Products { get; } = new Dictionary<string, TemplateAsset>();
        public static Dictionary<string, Pool> Pools { get; } = new Dictionary<string, Pool>();

        public static IEnumerable<TemplateAsset> GetItemsById(string id) {
            foreach (var item in SearchItems(id)) {
                yield return item;
            }

            IEnumerable<TemplateAsset> SearchItems(string searchid) {
                if (Items.ContainsKey(searchid)) {
                    yield return Items[searchid];
                }
                else if (Products.ContainsKey(searchid)) {
                    yield return Products[searchid];
                }
                else if (Pools.ContainsKey(searchid)) {
                    foreach (var item in Pools[searchid].Items) {
                        foreach (var item2 in SearchItems(item.ID)) {
                            yield return item2;
                        }
                    }
                }
                else {
                    Console.WriteLine(searchid);
                }


            }
        }
        static ItemProvider() {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.GuildhouseItem.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
                        Items.Add(item.ID, item);
                    }

                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.HarborOfficeItem.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
                        Items.Add(item.ID, item);
                    }
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.TownhallItem.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
                        Items.Add(item.ID, item);
                    }
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.VehicleItem.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
                        Items.Add(item.ID, item);
                    }
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.ShipSpecialist.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
                        Items.Add(item.ID, item);
                    }
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.CultureItem.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
                        Items.Add(item.ID, item);
                    }
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.Product.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s => new TemplateAsset(s))) {
                        Products.Add(item.ID, item);
                    }
                }
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssetViewer.Resources.Assets.RewardPools.xml")) {
                using (var reader = new StreamReader(stream)) {
                    var document = XDocument.Parse(reader.ReadToEnd()).Root;
                    foreach (var item in document.Elements().Select(s => s.FromXElement<Pool>())) {
                        Pools.Add(item.ID, item);
                    }
                }
            }

        }
    }
}
