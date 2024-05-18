using mobilka.Models;
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
    public partial class Izbran : ContentPage
    {
        private int userId;
        private List<Tovar> favoriteTovars;
        private int tovarId;

        public Izbran(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadFavoriteTovars();
        }

        private async void LoadFavoriteTovars()
        {
            var repository = App.Database;
            favoriteTovars = await repository.GetFavoriteTovarsAsync(userId);

            DisplayFavoriteTovars(favoriteTovars);
        }

        private void DisplayFavoriteTovars(IEnumerable<Tovar> tovars)
        {
            favoriteItemsLayout.Children.Clear();

            foreach (var tovar in tovars)
            {
                var frame = new Frame
                {
                    CornerRadius = 10,
                    HasShadow = true,
                    Margin = new Thickness(5),
                    WidthRequest = 240,
                    HeightRequest = 560
                };

                var stackLayout = new StackLayout();

                var image = new Image
                {
                    Source = tovar.image_tovar,
                    Aspect = Aspect.AspectFill,
                    HeightRequest = 280,
                    WidthRequest = frame.WidthRequest
                };
                stackLayout.Children.Add(image);

                stackLayout.Children.Add(new Label { Text = tovar.name, FontSize = 16, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center });
                stackLayout.Children.Add(new Label { Text = $"Бренд: {tovar.brend}", FontSize = 14, HorizontalOptions = LayoutOptions.Center });
                stackLayout.Children.Add(new Label { Text = $"Цена: ${tovar.price:F2}", FontSize = 14, HorizontalOptions = LayoutOptions.Center });

                var picker = new Picker
                {
                    Title = "Размер",
                    HorizontalOptions = LayoutOptions.Center
                };
                picker.ItemsSource = new List<string> { tovar.size };
                picker.SelectedIndex = 0;

                stackLayout.Children.Add(picker);

                var addToCartButton = new Button
                {
                    Text = "Добавить в корзину",
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("#512DA8"),
                    TextColor = Color.White,
                    Margin = new Thickness(10, 5, 10, 0),
                    WidthRequest = 200,
                    HeightRequest = 70
                };
                addToCartButton.CommandParameter = tovar.id;
                addToCartButton.Clicked += async (s, args) =>
                {
                    var button = (Button)s;
                    var tovarId = (int)button.CommandParameter;

                    var repository = App.Database;
                    var existingCartItem = await repository.GetCartItemAsync(userId, tovarId);

                    if (existingCartItem != null && existingCartItem.Quantity >= 10)
                    {
                        // Maximum quantity reached, inform the user
                        await DisplayAlert("Ошибка", $"Достигнуто максимальное количество ({existingCartItem.Quantity}) этого товара в корзине.", "OK");
                        return;
                    }

                    if (existingCartItem != null)
                    {
                        // Check if adding one more item exceeds the limit
                        if (existingCartItem.Quantity + 1 > 10)
                        {
                            await DisplayAlert("Ошибка", $"Невозможно добавить больше товаров данного типа. Достигнуто максимальное количество (10) в корзине.", "OK");
                            return;
                        }

                        // Увеличиваем количество товара в корзине
                        existingCartItem.Quantity++;
                        await repository.UpdateCartItemAsync(existingCartItem);
                        await DisplayAlert("Добавление", $"Товар {tovar.name} добавлен в корзину", "OK");
                    }
                    else
                    {
                        // Create new element in the cart with quantity 1
                        var cartItem = new CartItem
                        {
                            users_id = userId,
                            tovar_id = tovarId,
                            Quantity = 1
                        };



                        await repository.AddCartItemAsync(cartItem);
                        await DisplayAlert("Добавление", $"Товар {tovar.name} добавлен в корзину", "OK");
                    }

                };

                var removeFromFavoriteButton = new Button
                {
                    Text = "Удалить из избранного",
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("#C62828"),
                    TextColor = Color.White,
                    Margin = new Thickness(10, 5, 10, 0),
                    WidthRequest = 200,
                    HeightRequest = 70
                };
                removeFromFavoriteButton.CommandParameter = tovar.id;
                removeFromFavoriteButton.Clicked += async (s, args) =>
                {
                    var button = (Button)s;
                    var tovarId = (int)button.CommandParameter;
                    await RemoveFromFavorite(tovarId);
                    await DisplayAlert("Удаление", $"Товар {tovar.name} удален из избранного", "OK");
                };

                stackLayout.Children.Add(addToCartButton);
                stackLayout.Children.Add(removeFromFavoriteButton);

                frame.Content = stackLayout;
                favoriteItemsLayout.Children.Add(frame);
            }
        }

        private async Task RemoveFromFavorite(int tovarId)
        {
            var repository = App.Database;
            await repository.RemoveFromFavoriteAsync(userId, tovarId);

            // Refresh favorite items after removal
            favoriteTovars = await repository.GetFavoriteTovarsAsync(userId);
            DisplayFavoriteTovars(favoriteTovars);
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