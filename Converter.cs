using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GeoJsonLiveCharts
{
    public static class Converter
    {
        public static GeoFeature GetGeoFeature(string filePath)
        {
            using (var fs = new StreamReader(filePath, Encoding.UTF8, true))
            {
                var str = fs.ReadToEnd();
                var geoFeature = JsonConvert.DeserializeObject<GeoFeature>(str);
                return geoFeature;
            }
        }

        public static List<GeoFeature> GetGeoFeatures(string filePath)
        {
            using (var fs = new StreamReader(filePath, Encoding.UTF8, true))
            {
                var str = fs.ReadToEnd();
                var geoFeature = JsonConvert.DeserializeObject<GeoFeature>(str);
                return geoFeature.ChildrenFeatures;
            }
        }

        public static void GetMaxMinNum(GeoFeature geoFeature,
            out double maxLongitude, out double minLongitude,
            out double maxLatitude, out double minLatitude,
            out double width,out double height)
        {
            maxLongitude = 0;
            minLongitude = 0;
            maxLatitude = 0;
            minLatitude = 0;

            width = Math.Abs(maxLongitude - minLongitude);
            height = Math.Abs(maxLatitude - minLatitude);

            if (geoFeature == null)
                return;

            var points = geoFeature.AllPoints;

            maxLongitude = points.Max(p=>p.Longitude);
            minLongitude = points.Min(p => p.Longitude);
            maxLatitude = points.Max(p => p.Latitude);
            minLatitude = points.Min(p => p.Latitude);

            width = Math.Abs(maxLongitude - minLongitude);
            height = Math.Abs(maxLatitude - minLatitude);
        }

        public static void SaveAsXml(this GeoFeature geoFeature, string fileName, int accuracy = 3)
        {
            var results = geoFeature.ConvertToXml();
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var sw = new StreamWriter(fs))
                {
                    foreach (var result in results)
                    {
                        sw.WriteLine(result);
                    }
                }
            }
        }

        public static List<string> ConvertToXml(this GeoFeature geoFeature, int accuracy = 3)
        {
            var k = Math.Pow(10, accuracy);
            var results = new List<string>();

            GetMaxMinNum(geoFeature,
                out _, out var minLongitude,
                out var maxLatitude, out _,
                out var width, out var height);

            width *= k;
            height *= k;

            results.AddRange(new[]
            {
                "<LiveChartsMap xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">",
                $"<Height>{(int) height}</Height>",
                $"<Width>{(int) width}</Width>"
            });

            results.Add("<Shapes>");

            results.AddRange(geoFeature.ConvertToMapSharpe(minLongitude, maxLatitude, accuracy));
            if (geoFeature.ChildrenFeatures != null)
            {
                foreach (var feature in geoFeature.ChildrenFeatures)
                {
                    results.AddRange(feature.ConvertToMapSharpe(minLongitude, maxLatitude, accuracy));
                }
            }
                

            results.Add("</Shapes>");
            results.Add("</LiveChartsMap>");
            return results;
        }

        public static List<string> ConvertToMapSharpe(this GeoFeature geoFeature,
            double minLongitude, double maxLatitude, 
            int accuracy = 3)
        {
            var results = new List<string>();

            if (geoFeature.Geometry == null)
                return results;
            else if (geoFeature.Geometry.Polygons == null || geoFeature.Geometry.Polygons.Count == 0)
                return results;

            var i = 0;
            foreach (var polygon in geoFeature.Geometry.Polygons)
            {
                results.AddRange(polygon.ConvertToMapSharpe(geoFeature.Properties.Id, i, geoFeature.Properties.Name,
                    minLongitude, maxLatitude, accuracy));
                i++;
            }

            return results;
        }

        public static List<string> ConvertToMapSharpe(this GeoPolygon polygon, string parentId, int index, string name,
            double minLongitude, double maxLatitude,
            int accuracy = 3)
        {
            var results = new List<string>();

            if (polygon == null || polygon.AllPoints.Count == 0)
                return results;

            results.Add("<MapShape>");
            results.Add($"<Id>{parentId}{index}</Id>");
            results.Add($"<Name>{name}</Name>");
            results.Add("<Path>");

            var points = polygon.AllPoints;
            var count = points.Count;
            var path = $"M {points[0].ConvertToPathMarkUp(minLongitude, maxLatitude, accuracy)} ";
            for (var i = 1; i < count; i++)
            {
                path += $"L {points[i].ConvertToPathMarkUp(minLongitude, maxLatitude, accuracy)} ";
            }

            results.Add(path);
            results.Add(" Z</Path>");
            /*
             * <MapShape>
             * <Id>2003</Id>
             * <Name>Badghis</Name>
             * <Path>M425,429 L427,428 L428,428L433,427L437,426L438,427L438,428 Z</Path>
             * </MapShape>
             */
            results.Add("</MapShape>");
            return results;
        }

        public static string ConvertToPathMarkUp(this Coordinate point, 
            double minLongitude, double maxLatitude, int accuracy = 3)
        {
            var k = Math.Pow(10, accuracy);
            var x = Math.Abs((int) (point.Longitude * k - minLongitude * k));
            var y = Math.Abs((int)(point.Latitude * k - maxLatitude * k));
            return $"{x},{y}";
        }
    }
}
