using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GeoJsonLiveCharts
{
    public class Properties
    {
        [JsonProperty("adcode")]
        public string AddressCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("center")]
        public double[] Center { get; set; }

        [JsonProperty("centroid")]
        public double[] Centroid { get; set; }

        [JsonProperty("childrenNum")]
        public int ChildrenNum { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("subFeatureIndex")]
        public int SubFeatureIndex { get; set; }

        [JsonProperty("parent")]
        public Properties Parent { get; set; }

        public string Id => $"{AddressCode}";
    }

    public class GeoFeature
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("features")]
        public List<GeoFeature> ChildrenFeatures { get; set; }

        public List<Coordinate> AllPoints
        {
            get
            {
                var points = new List<Coordinate>();

                if (Geometry != null)
                {
                    points.AddRange(Geometry.AllPoints);
                }

                if (ChildrenFeatures == null) return points;
                foreach (var feature in ChildrenFeatures)
                {
                    points.AddRange(feature.AllPoints);
                }

                return points;
            }
        }

        public override string ToString()
        {
            return $"{Properties?.Name} {Properties?.Level}";
        }
    }

    /// <summary>
    /// 几何对象
    /// </summary>
    public class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public double[][][][] Coordinates {
            get => polygons;
            set
            {
                polygons = value;

                Polygons = new List<GeoPolygon>();

                if (polygons == null || polygons.Length < 1)
                    return;

                foreach (var polygon in from d in value where d != null && d.Length >= 1 select new GeoPolygon(d))
                {
                    Polygons.Add(polygon);
                }
            }
        }

        public double[][][][] polygons;

        public List<GeoPolygon> Polygons { get; private set; }

        public List<Coordinate> AllPoints
        {
            get
            {
                var points = new List<Coordinate>();

                foreach (var polygon in Polygons)
                {
                    points.AddRange(polygon.AllPoints);
                }

                return points;
            }
        }
    }

    public class GeoPolygon
    {
        public List<GeoLine> Lines;

        public GeoPolygon(IReadOnlyCollection<double[][]> values)
        {
            Lines = new List<GeoLine>();

            if (values == null || values.Count < 1)
                return;

            foreach (var line in from value in values where value != null && value.Length >= 1 select new GeoLine(value))
            {
                Lines.Add(line);
            }
        }

        public List<Coordinate> AllPoints
        {
            get
            {
                var points = new List<Coordinate>();

                foreach (var line in Lines)
                {
                    points.AddRange(line.Points);
                }

                return points;
            }
        }
    }

    public class GeoLine
    {
        public List<Coordinate> Points;

        public GeoLine(IReadOnlyCollection<double[]> values)
        {
            Points = new List<Coordinate>();

            if (values == null || values.Count < 1)
                return;

            foreach (var coordinate in values.Select(value => new Coordinate(value)))
            {
                Points.Add(coordinate);
            }
        }
    }

    public class Coordinate
    {
        public double Longitude;

        public double Latitude;

        public Coordinate(IReadOnlyList<double> values)
        {
            if(values==null || values.Count<2)
                return;
            Longitude = values[0];
            Latitude = values[1];
        }
    }
}
