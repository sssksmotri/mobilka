using mobilka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobilka
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
        public partial class OrderPage : ContentPage
        {
        private int userId;
        private List<int> cartItemIds;
        private int tovarId;
        private Dictionary<int, int> cartItemQuantities;

        public OrderPage(int userId, List<int> cartItemIds, Dictionary<int, int> cartItemQuantities)
        {
            InitializeComponent();
            this.userId = userId;
            this.cartItemIds = cartItemIds;
            this.cartItemQuantities = cartItemQuantities; // Сохраняем переданный словарь
            LoadUserInfo();
        }

        private async void PlaceOrderButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var repository = App.Database;
                var address = addressEntry.Text;
                var city = cityEntry.Text;
                var country = countryEntry.Text;
                var name = firstNameEntry.Text;
                var surname = lastNameEntry.Text;
                var phoneNumber = phoneNumberEntry.Text;

                // Validate form fields
                var errorMessage = new StringBuilder();
                if (string.IsNullOrWhiteSpace(address) ||
                    string.IsNullOrWhiteSpace(city) ||
                    string.IsNullOrWhiteSpace(country) ||
                    string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(surname) ||
                    string.IsNullOrWhiteSpace(phoneNumber))
                {
                    errorMessage.AppendLine("Пожалуйста, заполните все обязательные поля (адрес, город, страна, имя, фамилия, номер телефона).");
                }
                if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_]{4,}$"))
                {
                    errorMessage.AppendLine("Пожалуйста, введите корректное имя (только буквы).");
                }
                if (!Regex.IsMatch(surname, @"^[a-zA-Z0-9_]{4,}$"))
                {
                    errorMessage.AppendLine("Пожалуйста, введите корректную фамилию (только буквы).");
                }
                if (!Regex.IsMatch(address, @"^[a-zA-Z0-9_]{4,}$"))
                {
                    errorMessage.AppendLine("Пожалуйста, введите корректный адрес (только буквы).");
                }
                if (!Regex.IsMatch(city, @"^[a-zA-Z0-9_]{4,}$"))
                {
                    errorMessage.AppendLine("Пожалуйста, введите корректный город (только буквы).");
                }
                if (!Regex.IsMatch(country, @"^[a-zA-Z0-9_]{4,}$"))
                {
                    errorMessage.AppendLine("Пожалуйста, введите корректную страну (только буквы).");
                }

                if (!Regex.IsMatch(phoneNumber, @"^\d{11}$"))
                {
                    errorMessage.AppendLine("Пожалуйста, введите корректный номер телефона (например, +12345678901).");
                }

                if (errorMessage.Length > 0)
                {
                    await DisplayAlert("Ошибка", errorMessage.ToString(), "ОК");
                    return;
                }

                foreach (var cartItemId in cartItemIds)
                {
                    var newOrder = new Orders
                    {
                        users_id = userId,
                        OrderDate = DateTime.Now,
                        tovar_Id = cartItemId,
                        address = address,
                        city = city,
                        country = country,
                        Status = "Собирается",
                    };

                    await repository.SaveOrderAsync(newOrder);
                    await repository.RemoveFromCartAsync(userId, cartItemId);
                }

                await DisplayAlert("Заказ успешно собран", "Спасибо! Отправим ваш заказ в течении двух дней", "ОК");
                Main main = new Main(userId);
                await Navigation.PushModalAsync(main);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"При оформлении заказа произошла ошибка: {ex.Message}", "ОК");
            }
        }
        private async void LoadUserInfo()
        {
            var repository = App.Database;
            var user = await repository.GetUserAsync(userId);

            if (user != null)
            {
                firstNameEntry.Text = user.name;
                lastNameEntry.Text = user.surname;
                phoneNumberEntry.Text = user.number;
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
    }
}