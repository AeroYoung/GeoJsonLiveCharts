# GeoJsonLiveCharts

Live Charts Extension：Convert GeoJson to [LiveCharts](https://github.com/Live-Charts/Live-Charts) [GeoMap](https://lvcharts.net/App/examples/v1/wf/GeoHeatMap) data(*.xaml, MS's [Path Markup Syntax](https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/path-markup-syntax) ) ![GitHub](https://img.shields.io/github/license/AeroYoung/GeoJsonLiveCharts) ![GitHub](https://img.shields.io/badge/language-C#-bule.svg)![Nuget](https://img.shields.io/nuget/v/Newtonsoft.Json?label=Newtonsoft.Json)![GitHub last commit](https://img.shields.io/github/last-commit/AeroYoung/GeoJsonLivecharts)

## Installation & Usage

Visual Studio->Project->Item->Reference->Add->GeoJsonLiveCharts.dll

```csharp
var geoFeature = Converter.GetGeoFeature("map_data.json");
geoFeature.SaveAsXml(fileName, 3);
```

Map data format can be json or geojson. For example, china map data dowload link:[GeoJSON Data Dowload](http://datav.aliyun.com/tools/atlas/#&lat=31.769817845138945&lng=104.29901249999999&zoom=4)

## Net Version

.Net 4.5 or greater





