using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DesignLibrary_Tutorial.Models
{
    public class Geometry
    {
        public List<List<List<double>>> coordinates { get; set; }
        public string type { get; set; }
    }

    public class Centre
    {
        public string Altitude { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Properties
    {
        public Centre centre { get; set; }
    }

    public class FeaturesItem
    {
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }
        public string type { get; set; }
    }

    public class Coordinates
    {
        public List<FeaturesItem> features { get; set; }
        public string type { get; set; }
    }
}