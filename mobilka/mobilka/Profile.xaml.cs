using mobilka.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobilka
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Profile : ContentPage
    {
        private int userId;
        private int tovarId;
        private Repository repository;
        private Users currentUser;

        public Profile(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            repository = App.Database; 
            LoadUserData(userId);
        }

        private async void LoadUserData(int userId)
        {
            try
            {
                currentUser = await repository.GetUserAsync(userId);

                if (currentUser != null)
                {
                    
                    lastNameEntry.Text = currentUser.surname;
                    firstNameEntry.Text = currentUser.name;
                    phoneNumberEntry.Text = currentUser.number;
                    birthDateDatePicker.Date = currentUser.date_of_birthday;
                    emailEntry.Text = currentUser.email;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        private void Navigate_Clicked1(object sender, EventArgs e)
        {
            Main main = new Main(this.userId);
            Navigation.PushModalAsync(main);

        }
        private void Navigate_Clicked2(object sender, EventArgs e)
        {
            Profile profile = new Profile(this.userId);
            Navigation.PushModalAsync(profile);
        }
         private async void Navigate_Clicked3(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Izbran(userId)));
        }

        private async void Navigate_Clicked4(object sender, EventArgs e)
        {
            var repository = App.Database;
            var cartItems = await repository.GetCartItemsAsync(userId);

            if (cartItems != null && cartItems.Any())
            {
                var firstCartItem = cartItems.FirstOrDefault();
                if (firstCartItem != null)
                {
                    await Navigation.PushModalAsync(new NavigationPage(new CatPage(userId, firstCartItem.id)));
                }
                else
                {

                    await Navigation.PushModalAsync(new NavigationPage(new CatPage(userId, tovarId)));
                }
            }
            else
            {
                await Navigation.PushModalAsync(new NavigationPage(new CatPage(userId, tovarId)));
            }
        }
        private async void SaveChanges_Clicked(object sender, EventArgs e) 
        {
            try
            {
                // Проверка заполнения всех обязательных полей
                if (string.IsNullOrWhiteSpace(lastNameEntry.Text) ||
                    string.IsNullOrWhiteSpace(firstNameEntry.Text) ||
                    string.IsNullOrWhiteSpace(phoneNumberEntry.Text) ||
                    birthDateDatePicker.Date == DateTime.MinValue ||
                    string.IsNullOrWhiteSpace(emailEntry.Text))
                {
                    await DisplayAlert("Ошибка", "Пожалуйста, заполните все обязательные поля.", "OK");
                    return;
                }
                if (!Regex.IsMatch(firstNameEntry.Text, @"^[а-яА-Яa-zA-Z]{4,}$"))
                {
                    await DisplayAlert("Ошибка", "Пожалуйста, введите корректное имя (только буквы).", "OK");
                    return;
                }
                if (!Regex.IsMatch(lastNameEntry.Text, @"^[а-яА-Яa-zA-Z]{4,}$"))
                {
                    await DisplayAlert("Ошибка", "Пожалуйста, введите корректную фамилию (только буквы).", "OK");
                    return;
                }
                // Валидация номера телефона (ровно 11 цифр)
                if (!Regex.IsMatch(phoneNumberEntry.Text, @"^\d{11}$"))
                {
                    await DisplayAlert("Ошибка", "Пожалуйста, введите корректный номер телефона (ровно 11 цифр).", "OK");
                    return;
                }
                if (!Regex.IsMatch(emailEntry.Text, @"^(?=.*[a-zA-Z])[\p{L}0-9!#$%^&*()-_]+@[\p{L}0-9!#$%^&*()-_]+$"))
                {
                    await DisplayAlert("Ошибка", "Пожалуйста, введите корректный email. ", "OK");
                    return;
                }
                // Обновление объекта currentUser новыми значениями из пользовательского интерфейса
                currentUser.surname = lastNameEntry.Text;
                currentUser.name = firstNameEntry.Text;
                currentUser.number = phoneNumberEntry.Text;
                currentUser.date_of_birthday = birthDateDatePicker.Date;
                currentUser.email = emailEntry.Text;



                int rowsUpdated = await repository.SaveUserAsync(currentUser);

                if (rowsUpdated > 0)
                {
                    await DisplayAlert("Успех", "Изменения успешно сохранены!", "OK");
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось сохранить изменения", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }
        private async void HistoryButton_Clicked(object sender, EventArgs e)
        {
            // Переход на страницу истории заказов
            OrderHistory orderHistoryPage = new OrderHistory(userId);
            await Navigation.PushModalAsync(orderHistoryPage);
        }
        private void Handle_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private async void ViewOrderHistory_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new OrderHistory(userId));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }
        private async void vihod_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["IsLoggedIn"] = false;
            Application.Current.Properties.Remove("UserId");
            await Application.Current.SavePropertiesAsync();
            App.Current.MainPage = new NavigationPage(new Avtorizasia());
        }
        

    }
}