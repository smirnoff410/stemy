
using Android.App;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using System;
using Android.Widget;
using DesignLibrary_Tutorial.Helpers;
using Android.Views;
using DesignLibrary_Tutorial.Models;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DesignLibrary_Tutorial
{
    [Activity(Label = "CheeseDetailActivity", Theme = "@style/Theme.DesignDemo")]
    public class CheeseDetailActivity : AppCompatActivity
    {
        DataBase db;

        public const string EXTRA_NAME = "cheese_name";

        private List<FieldInfo> FieldInfo;

        RecyclerView mRecyclerViewFieldInfo;

        RecyclerView.LayoutManager mLayoutManagerFieldInfo;

        FieldInfoAdapter mAdapterFieldInfo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Activity_Detail);

            db = new DataBase();
            db.CreateDataBase();

            string id = Intent.GetStringExtra("Position");
            LoadData(id);

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.sample_actions, menu);
            return true;
        }

        private void LoadBackDrop()
        {
            ImageView imageView = FindViewById<ImageView>(Resource.Id.backdrop);
            imageView.SetImageResource(Resource.Drawable.field);
        }

        public static HttpClient CreateClient(string token = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        private void OnItemClick(object sender, int e)
        {
            //Intent intent = new Intent(this, typeof(WeatherInfoActivity));
            //intent.PutExtra("IdField", IdField);
            //StartActivity(intent);
        }

        private string WindDirection(double deg)
        {
            if (deg >= 0 && deg < 90)
                return "Направление: СВ";
            else if (deg >= 90 && deg < 180)
                return "Направление: СЗ";
            else if (deg >= 180 && deg < 270)
                return "Направление: ЮЗ";
            else
                return "Направление: ЮВ";
        }

        private DateTime DateTime(int UnixTime)
        {
            DateTime time = DateTimeOffset.FromUnixTimeSeconds(UnixTime).DateTime.AddHours(3);
            return time;
        }

        private async void LoadData(string id)
        {
            using (HttpClient client = CreateClient(db.SelectTablePerson()[0].access_token))
            {
                client.BaseAddress = new Uri("http://whetherdata.azurewebsites.net/api/weather/current/" + id);

                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(content);
                var Weather1 = JsonConvert.DeserializeObject<WeatherModel>(o.ToString());

                //textView.Text = Weather1.name;

                FieldInfo = new List<FieldInfo>
                {
                    new FieldInfo{icon = Resource.Drawable.temperature, text = "Температура: " + Weather1.main.temp + " градусов", descriptions = ""},
                    new FieldInfo{icon = Resource.Drawable.cloud, text = "Облачность: " + Weather1.weather[0].description, descriptions = ""},
                    new FieldInfo{icon = Resource.Drawable.wind, text = "Скорость ветра: " + Weather1.wind.speed + " м/с", descriptions = WindDirection(Weather1.wind.deg)},
                    new FieldInfo{icon = Resource.Drawable.country, text = "Страна: " + Weather1.sys.country, descriptions = ""},
                    new FieldInfo{icon = Resource.Drawable.sunrise, text = "Восход: " + DateTime(Weather1.sys.sunrise).ToString() + " МСК", descriptions = ""},
                    new FieldInfo{icon = Resource.Drawable.sunset, text = "Закат: " + DateTime(Weather1.sys.sunset).ToString() + " МСК", descriptions = ""}
                };

                SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolBar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);

                string cheeseName = Intent.GetStringExtra(EXTRA_NAME);

                CollapsingToolbarLayout collapsingToolBar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
                collapsingToolBar.Title = cheeseName;

                LoadBackDrop();
                
                mRecyclerViewFieldInfo = FindViewById<RecyclerView>(Resource.Id.recyclerFieldInfo);

                mLayoutManagerFieldInfo = new LinearLayoutManager(this);

                mRecyclerViewFieldInfo.SetLayoutManager(mLayoutManagerFieldInfo);

                mAdapterFieldInfo = new FieldInfoAdapter(FieldInfo);

                mAdapterFieldInfo.ItemClick += OnItemClick;

                mRecyclerViewFieldInfo.SetAdapter(mAdapterFieldInfo);
            }
        }
    }

    public class FieldInfoViewHolder : RecyclerView.ViewHolder
    {
        public ImageView fieldInfoIcon { get; private set; }
        public TextView fieldInfoText { get; private set; }
        public TextView fieldInfoDescription { get; private set; }

        // Get references to the views defined in the CardView layout.
        public FieldInfoViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            fieldInfoIcon = itemView.FindViewById<ImageView>(Resource.Id.icon);
            fieldInfoText = itemView.FindViewById<TextView>(Resource.Id.fieldInfoText);
            fieldInfoDescription = itemView.FindViewById<TextView>(Resource.Id.fieldInfoDescription);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    public class FieldInfoAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Underlying data set (a photo album):
        public List<FieldInfo> mFieldInfo;

        // Load the adapter with the data set (photo album) at construction time:
        public FieldInfoAdapter(List<FieldInfo> fieldInfo)
        {
            mFieldInfo = fieldInfo;
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.FieldInfoCardView, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            FieldInfoViewHolder vh = new FieldInfoViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            FieldInfoViewHolder vh = holder as FieldInfoViewHolder;

            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album:
            vh.fieldInfoIcon.SetImageResource(mFieldInfo[position].icon);
            vh.fieldInfoText.Text = mFieldInfo[position].text;
            vh.fieldInfoDescription.Text = mFieldInfo[position].descriptions;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return mFieldInfo.Count; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}