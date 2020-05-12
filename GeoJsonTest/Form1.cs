using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoJsonLiveCharts;

namespace GeoJsonTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            geoMap1.Source = @"C:\Users\yao.yang\Desktop\AnalysisStudio\GeoJsonLiveCharts\GeoJsonTest\贵州省.xml";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var geoFeature = Converter.GetGeoFeature(dialog.FileName);

            var fileName = Path.ChangeExtension(dialog.FileName, "xml");
            geoFeature.SaveAsXml(fileName, 3);

            geoMap1.Source = fileName;
            geoMap1.Refresh();
        }

    }
}
