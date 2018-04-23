using DevExpress.Xpf.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DXMapShapeFile1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void vLayer_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            GeoPoint point = (GeoPoint)mapControl.ScreenPointToCoordPoint(e.GetPosition(mapControl));
            var hi = mapControl.CalcHitInfo(e.GetPosition(mapControl));            
            foreach (var item in vectorLayer.Data.DisplayItems) {
                if (IsPointInsideItem(item, point))
                    this.Title = item.Attributes["NAME"].Value.ToString();
            }
        }

        private bool IsPointInsideItem(MapItem item, GeoPoint point) {
            MapPolygon polygon = item as MapPolygon;
            if (polygon != null) {
                return IsPointInside(polygon.Points.Cast<GeoPoint>().ToArray(), point);
            }
            MapPath path = item as MapPath;
            if (path != null) {
                var segments = ((MapPathGeometry)path.Data).Figures.SelectMany(f => f.Segments).Cast<MapPolyLineSegment>();
                return segments.Any(segment => IsPointInside(segment.Points.Cast<GeoPoint>().ToArray(), point));

            }
            return false;
        }

        private bool IsPointInside(GeoPoint[] polygon, GeoPoint testPoint) {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++) {
                if (polygon[i].Latitude < testPoint.Latitude && polygon[j].Latitude >= testPoint.Latitude || polygon[j].Latitude < testPoint.Latitude && polygon[i].Latitude >= testPoint.Latitude) {
                    if (polygon[i].Longitude + (testPoint.Latitude - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) * (polygon[j].Longitude - polygon[i].Longitude) < testPoint.Longitude) {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }



    }
}
