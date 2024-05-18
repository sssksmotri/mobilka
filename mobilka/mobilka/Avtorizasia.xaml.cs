using mobilka.Droid;
using mobilka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.IO;

namespace mobilka
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Avtorizasia : ContentPage
    {
        public Avtorizasia()
        {
            InitializeComponent();

        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            string login = login1Entry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Ошибка", "Введите логин и пароль", "OK");
                return;
            }
            Repository repository = App.Database;
            Users authenticatedUser = await repository.AuthenticateUserAsync(login, password);

            if (authenticatedUser != null)
            {
                // Успешная аутентификация
                await DisplayAlert("Успешно", "Авторизация успешна", "OK");

                // Сохраняем состояние входа пользователя в локальное хранилище
                Application.Current.Properties["IsLoggedIn"] = true; // Используем прямое логическое значение
                Application.Current.Properties["UserId"] = authenticatedUser.id;
                await Application.Current.SavePropertiesAsync();

                // Переходим на главную страницу с передачей userId
                App.Current.MainPage = new NavigationPage(new Main(authenticatedUser.id));
            }
            else
            {
                // Неудачная аутентификация
                await DisplayAlert("Ошибка", "Неправильный логин или пароль", "OK");
            }
        }

        private void Registration_Clicked(object sender, EventArgs e)
        {
            Registrasia registrasia = new Registrasia();
            Navigation.PushModalAsync(registrasia);
        }
    }
}