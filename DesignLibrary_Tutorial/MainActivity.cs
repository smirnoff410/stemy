using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.App;
using System.Collections.Generic;
using Java.Lang;
using DesignLibrary_Tutorial.Fragments;
using DesignLibrary_Tutorial.Helpers;
using DesignLibrary_Tutorial.Models;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net;

namespace DesignLibrary_Tutorial
{
    [Activity(Label = "Stemy", Theme = "@style/Theme.DesignDemo")]
    public class MainActivity : AppCompatActivity
    {
        DataBase db;
        List<Person> person;

        private List<Field> Fields;

        private DrawerLayout mDrawerLayout;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            db = new DataBase();
            db.CreateDataBase();

            person = db.SelectTablePerson();

            GetFieldJson();

           
        }

        private async void GetFieldJson()
        {
            using (var client = new HttpClient())
            {
                FieldPost token = new FieldPost { token = db.SelectTablePerson()[0].access_token };
                string json = JsonConvert.SerializeObject(token);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://whetherdata.azurewebsites.net/api/fields", content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var asd = await response.Content.ReadAsStringAsync();
                    Fields = JsonConvert.DeserializeObject<List<Field>>(asd);

                    List<Field> fieldsDelete = db.SelectTableField();
                    for (int i = 0; i < fieldsDelete.Count; i++)
                    {
                        db.DeleteTableField(fieldsDelete[i]);
                    }
                    fieldsDelete = db.SelectTableField();

                    for (int k = 0; k < Fields.Count; k++)
                    {
                        db.InsertIntoTableField(Fields[k]);
                        Coordinates coord = JsonConvert.DeserializeObject<Coordinates>(Fields[k].coordinates);
                    }
                }
                else
                {
                    Toast.MakeText(this, "Ошибка загрузки данных, пожалуйста, повторите позже", ToastLength.Long).Show();
                }

                SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
                SetSupportActionBar(toolBar);

                SupportActionBar ab = SupportActionBar;
                ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
                ab.SetDisplayHomeAsUpEnabled(true);

                mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

                NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                if (navigationView != null)
                {
                    SetUpDrawerContent(navigationView);
                }

                TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabs);

                ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);

                SetUpViewPager(viewPager);

                tabs.SetupWithViewPager(viewPager);

                FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

                fab.Click += (o, e) =>
                {
                    View anchor = o as View;

                    Snackbar.Make(anchor, "Yay Snackbar!!", Snackbar.LengthLong)
                            .SetAction("Action", v =>
                            {
                            //Do something here
                            Intent intent = new Intent(fab.Context, typeof(BottomSheetActivity));
                                StartActivity(intent);
                            })
                            .Show();
                };
            }
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            adapter.AddFragment(new Fragment1(), "Fragment 1");
            adapter.AddFragment(new Fragment2(), "Fragment 2");

            viewPager.Adapter = adapter;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);                    
            }
        }

        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                e.MenuItem.SetChecked(true);
                switch(e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        Toast.MakeText(this, "Home", ToastLength.Short).Show();
                        break;
                    case Resource.Id.nav_messages:
                        Toast.MakeText(this, "Message", ToastLength.Short).Show();
                        break;
                    case Resource.Id.nav_friends:
                        Toast.MakeText(this, "Friends", ToastLength.Short).Show();
                        break;
                    case Resource.Id.nav_discussion:
                        Toast.MakeText(this, "Discussion", ToastLength.Short).Show();
                        db.DeleteTablePerson(person[0]);
                        Intent intent = new Intent(this, typeof(StartActivity));
                        StartActivity(intent);
                        break;
                }
                mDrawerLayout.CloseDrawers();
            };
        }

        public class TabAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentNames { get; set; }

            public TabAdapter (SupportFragmentManager sfm) : base (sfm)
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }

            public void AddFragment(SupportFragment fragment, string name)
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }

            public override int Count
            {
                get
                {
                    return Fragments.Count;
                }
            }

            public override SupportFragment GetItem(int position)
            {
                return Fragments[position];
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(FragmentNames[position]);
            }
        }
    }
}

