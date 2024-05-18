using mobilka.Droid;
using mobilka.Services;
using mobilka.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
namespace mobilka
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "magazine.db";
        public static Repository database;
        public static Repository Database
        {
            get
            {
                if (database == null)
                {
                    string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME);
                    database = new Repository(databasePath);
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME);
            App.database = new Repository(databasePath);
            bool isLoggedIn = Application.Current.Properties.ContainsKey("IsLoggedIn") && (bool)Application.Current.Properties["IsLoggedIn"];

            if (isLoggedIn)
            {
                int userId = (int)Application.Current.Properties["UserId"];
                // Пользователь авторизован, загрузить страницу для авторизованных пользователей
                MainPage = new NavigationPage(new Main(userId));
            }
            else
            {
                // Пользователь не авторизован, загрузить страницу входа
                MainPage = new NavigationPage(new Avtorizasia());
            }
        }




    protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
