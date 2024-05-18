using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLitePCL;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using mobilka.Models;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
namespace mobilka.Droid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registrasia : ContentPage
    {
        public Registrasia()
        {
            InitializeComponent();
        }
        private void Handle_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ваш код обработки события SelectedIndexChanged здесь
        }
        
        private async void Registration_Clicked(object sender, EventArgs e)
        {
            // Создание нового пользователя на основе введенных данных
            Users newUser = new Users
            {
                login = LOGINEntry.Text,
                email = EmailEntry.Text,
                password = PasswordEntry.Text,
                name = FirstNameEntry.Text,
                surname = LastNameEntry.Text,
                number = PhoneNumberEntry.Text,
                date_of_birthday = BirthDatePicker.Date,
                gender = GenderPicker.SelectedItem?.ToString()
            };

            if (string.IsNullOrWhiteSpace(newUser.login) ||
                string.IsNullOrWhiteSpace(newUser.email) ||
                string.IsNullOrWhiteSpace(newUser.password) ||
                string.IsNullOrWhiteSpace(newUser.name) ||
                string.IsNullOrWhiteSpace(newUser.surname) ||
                string.IsNullOrWhiteSpace(newUser.number) ||
                newUser.date_of_birthday == DateTime.MinValue ||
                string.IsNullOrWhiteSpace(newUser.gender))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, заполните все поля.", "OK");
                return;
            }

            // Проверка введенных данных
            bool hasError = false;
            StringBuilder errorMessage = new StringBuilder();

            // Проверка имени
            if (!Regex.IsMatch(newUser.name, @"^[а-яА-Яa-zA-Z]{4,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректное имя (только буквы).");
                hasError = true;
            }

            if (newUser.gender == null)
            {
                errorMessage.AppendLine("Пожалуйста, выберите пол (мужской или женский).");
                hasError = true;
            }

            // Проверка фамилии
            if (!Regex.IsMatch(newUser.surname, @"^[а-яА-Яa-zA-Z]{4,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректную фамилию (только буквы).");
                hasError = true;
            }

            // Проверка номера телефона (ровно 11 цифр)
            if (!Regex.IsMatch(newUser.number, @"^\d{11}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный номер телефона (ровно 11 цифр).");
                hasError = true;
            }
            if (!Regex.IsMatch(newUser.email, @"^(?=.*[a-zA-Z])[\p{L}0-9!#$%^&*()-_]+@[\p{L}0-9!#$%^&*()-_]+$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный email. ");
                hasError = true;
            }
            if (!Regex.IsMatch(newUser.login, @"^[a-zA-Z0-9_]{4,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный email. ");
                hasError = true;
            }if (!Regex.IsMatch(newUser.password, @"^(?=.*[a-zA-Z])(?=.*[0-9!()-_]).{8,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный email. ");
                hasError = true;
            }

            // Проверка даты рождения
            DateTime minDate = new DateTime(1950, 1, 1);
            DateTime maxDate = new DateTime(2009, 12, 31);

            if (newUser.date_of_birthday == DateTime.MinValue ||
                newUser.date_of_birthday < minDate ||
                newUser.date_of_birthday > maxDate)
            {
                errorMessage.AppendLine("Пожалуйста, введите корректную дату рождения от 01.01.1950 до 31.12.2009.");
                hasError = true;
            }

            // Проверка логина в базе данных
            try
            {
                var existingUser = await App.Database.GetUserByLoginAsync(newUser.login);

                if (existingUser != null)
                {
                    // Пользователь с таким логином уже существует
                    await DisplayAlert("Ошибка", "Этот логин уже занят. Пожалуйста, выберите другой.", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке логина в базе данных: {ex.Message}");
                await DisplayAlert("Ошибка", "Не удалось выполнить проверку логина. Пожалуйста, попробуйте еще раз.", "OK");
                return;
            }

            if (hasError)
            {
                await DisplayAlert("Ошибка", errorMessage.ToString(), "OK");
                return;
            }

            try
            {
                newUser.password = PasswordHasher.HashPassword(newUser.password);
                int rowsAffected = await App.Database.SaveUserAsync(newUser);

                if (rowsAffected > 0)
                {
                    // Уведомление об успешной регистрации
                    await DisplayAlert("Успешно", "Регистрация прошла успешно", "OK");

                    // Сохраняем состояние входа пользователя в локальное хранилище
                    Application.Current.Properties["IsLoggedIn"] = true;
                    Application.Current.Properties["UserId"] = newUser.id;
                    await Application.Current.SavePropertiesAsync();

                    
                    App.Current.MainPage = new NavigationPage(new Main(newUser.id));
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось сохранить пользователя", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка регистрации: {ex.Message}", "OK");
            }
        }
       
        private void Voiti_Clicked(object sender, EventArgs e)
        {
            Avtorizasia avtorizasia = new Avtorizasia();


            Navigation.PushModalAsync(avtorizasia);
        }
    }
}