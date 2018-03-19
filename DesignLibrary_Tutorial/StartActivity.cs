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
using Android.Support.V7.App;
using DesignLibrary_Tutorial.Models;
using System.Net.Http;
using Newtonsoft.Json;
using DesignLibrary_Tutorial.Helpers;
using Android.Views.InputMethods;

namespace DesignLibrary_Tutorial
{
    [Activity(Theme = "@style/Theme.DesignDemo", NoHistory = true)]
    public class StartActivity : AppCompatActivity
    {
        DataBase db;
        List<Person> listPerson = new List<Person>();
        Person person = new Person();

        EditText userName;
        EditText userPassword;
        LinearLayout container;
        ProgressBar progressBar;
        TextView errorText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StartActivity);

            db = new DataBase();
            db.CreateDataBase();

            LoadData();

            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            userName = FindViewById<EditText>(Resource.Id.userName);
            userPassword = FindViewById<EditText>(Resource.Id.userPassword);
            container = FindViewById<LinearLayout>(Resource.Id.container);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            errorText = FindViewById<TextView>(Resource.Id.errorText);
            container.RemoveView(errorText);
            container.RemoveView(progressBar);
            btnLogin.Click += (o, e) =>
            {
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(userPassword.WindowToken, 0);
                container.AddView(progressBar);
                container.RemoveView(errorText);
                GetTokenJson();
            };
        }

        private void LoadData()
        {
            listPerson = db.SelectTablePerson();
            if (listPerson.Count > 0)
            {
                Intent act = new Intent(this, typeof(MainActivity));
                StartActivity(act);
            }
        }

        private async void GetTokenJson()
        {
            using (var client = new HttpClient())
            {
                var Person = new PostPerson { email = userName.Text, password = userPassword.Text };
                string json = JsonConvert.SerializeObject(Person);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://whetherdata.azurewebsites.net/api/token/get", content);

                var asd = await response.Content.ReadAsStringAsync();
                if (asd.Length < 400)
                {
                    container.RemoveView(progressBar);
                    container.AddView(errorText);
                }
                else
                {
                    var otvet = JsonConvert.DeserializeObject<Person>(asd);

                    Person person = otvet;
                    db.InsertIntoTablePerson(person);

                    Intent act = new Intent(this, typeof(MainActivity));
                    StartActivity(act);
                }
            }
        }
    }
}