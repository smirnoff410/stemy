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
    public class Field
    {
        public int id { get; set; }
        public string name { get; set; }
        public string fieldArea { get; set; }
        public string coordinates { get; set; }
        public int company_id { get; set; }
    }

    public class FieldInfo
    {
        public int icon { get; set; }
        public string text { get; set; }
        public string descriptions { get; set; }
    }
}