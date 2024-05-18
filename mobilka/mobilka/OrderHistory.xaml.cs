using mobilka.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobilka
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderHistory : ContentPage
    {
        private readonly int userId;
        private readonly Repository repository;
        private int tovarId;

        public OrderHistory(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            repository = App.Database; // Получаем экземпляр репозитория базы данных

            // Загрузка и отображение данных заказов
            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                var orders = await repository.GetOrdersAsync();
                if (orders != null && orders.Any())
                {
                    foreach (var order in orders)
                    {
                        var tovar = await repository.GetTovarAsync(order.tovar_Id);

                        // Создаем Grid для структурированного размещения данных заказа и товара
                        var grid = new Grid
                        {
                            Margin = new Thickness(0, 0, 0, 20), // Отступ снизу между сетками
                            BackgroundColor = Color.White, // Цвет заднего фона сетки
                            Padding = new Thickness(10),
                            ColumnDefinitions =
                            {
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                            }
                        };

                        // Добавляем данные заказа в Grid
                        grid.Children.Add(new Label { Text = $"Order Date:", FontAttributes = FontAttributes.Bold }, 0, 0);
                        grid.Children.Add(new Label { Text = $"{order.OrderDate}", Margin = new Thickness(5, 0, 0, 0) }, 1, 0);

                        grid.Children.Add(new Label { Text = $"Address:", FontAttributes = FontAttributes.Bold }, 0, 1);
                        grid.Children.Add(new Label { Text = $"{order.address}", Margin = new Thickness(5, 0, 0, 0) }, 1, 1);

                        grid.Children.Add(new Label { Text = $"City:", FontAttributes = FontAttributes.Bold }, 0, 2);
                        grid.Children.Add(new Label { Text = $"{order.city}", Margin = new Thickness(5, 0, 0, 0) }, 1, 2);

                        grid.Children.Add(new Label { Text = $"Country:", FontAttributes = FontAttributes.Bold }, 0, 3);
                        grid.Children.Add(new Label { Text = $"{order.country}", Margin = new Thickness(5, 0, 0, 0) }, 1, 3);

                        grid.Children.Add(new Label { Text = $"Status:", FontAttributes = FontAttributes.Bold }, 0, 4);
                        grid.Children.Add(new Label { Text = $"{order.Status}", Margin = new Thickness(5, 0, 0, 0) }, 1, 4);

                        // Добавляем данные товара в Grid
                        if (tovar != null)
                        {
                            grid.Children.Add(new Label { Text = $"Tovar Name:", FontAttributes = FontAttributes.Bold }, 0, 5);
                            grid.Children.Add(new Label { Text = $"{tovar.name}", Margin = new Thickness(5, 0, 0, 0) }, 1, 5);
                            grid.Children.Add(new Label { Text = $"Tovar Price:", FontAttributes = FontAttributes.Bold }, 0, 6);
                            grid.Children.Add(new Label { Text = $"{tovar.price}", Margin = new Thickness(5, 0, 0, 0) }, 1, 6);
                            grid.Children.Add(new Label { Text = $"Quantity:", FontAttributes = FontAttributes.Bold }, 0, 7);
                            grid.Children.Add(new Label { Text = $"{tovar.Quantity}", Margin = new Thickness(5, 0, 0, 0) }, 1, 7);
                            grid.Children.Add(new Label { Text = $"Size:", FontAttributes = FontAttributes.Bold }, 0, 8);
                            grid.Children.Add(new Label { Text = $"{tovar.size}", Margin = new Thickness(5, 0, 0, 0) }, 1, 8);
                            grid.Children.Add(new Label { Text = $"Color:", FontAttributes = FontAttributes.Bold }, 0, 9);
                            grid.Children.Add(new Label { Text = $"{tovar.color}", Margin = new Thickness(5, 0, 0, 0) }, 1, 9);
                            grid.Children.Add(new Label { Text = $"Brand:", FontAttributes = FontAttributes.Bold }, 0, 10);
                            grid.Children.Add(new Label { Text = $"{tovar.size}", Margin = new Thickness(5, 0, 0, 0) }, 1, 10);

                        }

                        // Добавляем Grid с данными заказа в mainLayout
                        mainLayout.Children.Add(grid);
                    }
                }
                else
                {
                    mainLayout.Children.Add(new Label { Text = "No orders found.", HorizontalOptions = LayoutOptions.CenterAndExpand });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
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