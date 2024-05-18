using mobilka.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobilka
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CatPage : ContentPage
    {
        private int _tovarId;
        private Tovar _tovar;
        private int userId;
        private List<Tovar> cartItems;
        private decimal totalPrice;
        private const string CartKey = "CartItems";
        private Dictionary<int, int> selectedQuantities = new Dictionary<int, int>();
        private int cartItemQuantities;

        public CatPage(int userId, int tovarId)
        {
            InitializeComponent();
            _tovarId = tovarId;
            this.userId = userId;
            LoadTovar();
        }

        private async void LoadTovar()
        {
            var repository = App.Database;
            _tovar = await repository.GetTovarAsync(_tovarId);

            if (_tovar != null)
            {
                var cartItem = await repository.GetCartItemAsync(userId, _tovarId);
                selectedQuantities[_tovarId] = cartItem?.Quantity ?? 1; // Initialize quantity for this item

                cartItems = await repository.GetCartItemsAsync(userId);
                DisplayTovar(cartItems);
            }
        }

        private void DisplayTovar(IEnumerable<Tovar> tovars)
        {
            cartItemsLayout.Children.Clear();
            totalPrice = 0; // Reset total price before recalculating

            foreach (var tovar in tovars)
            {
                var frame = new Frame
                {
                    CornerRadius = 10,
                    HasShadow = true,
                    Margin = new Thickness(5),
                    WidthRequest = 240,
                    HeightRequest = 600
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

                stackLayout.Children.Add(CreateLabel(tovar.name, 16, Color.Black, LayoutOptions.Center, new Thickness(0, 5, 0, 0)));
                stackLayout.Children.Add(CreateLabel($"Бренд: {tovar.brend}", 14, Color.Black, LayoutOptions.Center, new Thickness(0, 5, 0, 0)));
                stackLayout.Children.Add(CreateLabel($"Цена: ${tovar.price:F2}", 14, Color.Black, LayoutOptions.Center, new Thickness(0, 5, 0, 0)));
                stackLayout.Children.Add(CreateLabel($"Цвет: {tovar.color}", 14, Color.Black, LayoutOptions.Center, new Thickness(0, 5, 0, 0)));

                var picker = new Picker
                {
                    Title = "Размер",
                    HorizontalOptions = LayoutOptions.Center
                };
                picker.ItemsSource = new List<string> { tovar.size };
                picker.SelectedIndex = 0;

                stackLayout.Children.Add(picker);
                var quantityPicker = new Picker
                {
                    Title = "Количество",
                    HorizontalOptions = LayoutOptions.Center
                };

                for (int i = 1; i <= 10; i++)
                {
                    quantityPicker.Items.Add(i.ToString());
                }

                int selectedQuantity = selectedQuantities.ContainsKey(tovar.id) ? selectedQuantities[tovar.id] : 1;
                quantityPicker.SelectedIndex = selectedQuantity - 1;

                quantityPicker.SelectedIndexChanged += async (s, e) =>
                {
                    var newQuantity = quantityPicker.SelectedIndex + 1;
                    selectedQuantities[tovar.id] = newQuantity; // Update selected quantity

                    await UpdateCartItemQuantity(tovar.id, newQuantity);
                };

                stackLayout.Children.Add(quantityPicker);

                var deleteFromCartButton = new Button
                {
                    Text = "Удалить из корзины",
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("#C62828"),
                    TextColor = Color.White,
                    Margin = new Thickness(10, 5, 10, 0),
                    WidthRequest = 200,
                    HeightRequest = 70
                };
                deleteFromCartButton.CommandParameter = tovar.id;
                deleteFromCartButton.Clicked += async (s, args) =>
                {
                    var button = (Button)s;
                    var tovarId = (int)button.CommandParameter;
                    await RemoveFromCart(tovarId);
                    await DisplayAlert("Удаление", $"Товар {tovar.name} удален из корзины", "OK");
                };

                stackLayout.Children.Add(deleteFromCartButton);

                frame.Content = stackLayout;
                cartItemsLayout.Children.Add(frame);

                
                totalPrice += tovar.price * selectedQuantity;
            }

            totalPriceLabel.Text = $"Total Price: ${totalPrice:F2}";
        }
        private async Task RemoveFromCart(int tovarId)
        {
            var repository = App.Database;
            var userId = this.userId; // Используем переменную класса

            await repository.RemoveFromCartAsync(userId, tovarId);

            // Обновляем отображение корзины после удаления товара
            cartItems = await repository.GetCartItemsAsync(userId);
            DisplayTovar(cartItems); //
        }

        private Label CreateLabel(string text, int fontSize, Color textColor, LayoutOptions horizontalOptions, Thickness margin)
        {
            return new Label
            {
                Text = text,
                FontSize = fontSize,
                TextColor = textColor,
                HorizontalOptions = horizontalOptions,
                Margin = margin
            };
        }

        private async void PayButton_Clicked(object sender, EventArgs e)
        {
            var repository = App.Database;
            var cartItems = await repository.GetCartItemsAsync(userId);

            if (cartItems == null || !cartItems.Any())
            {
                await DisplayAlert("Оплата", "Корзина пуста. Пожалуйста, добавьте товары перед оплатой.", "OK");
                return;
            }

            try
            {
                // Создаем список cartItemIds и словарь cartItemQuantities
                List<int> cartItemIds = cartItems.Select(item => item.id).ToList();
                Dictionary<int, int> cartItemQuantities = new Dictionary<int, int>();
                foreach (var item in cartItems)
                {
                    cartItemQuantities[item.id] = item.Quantity;
                }

                // Переходим на страницу оформления заказа и передаем необходимые данные
                await Navigation.PushModalAsync(new OrderPage(userId, cartItemIds, cartItemQuantities));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка при оформлении заказа: {ex.Message}", "OK");
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

                    await Navigation.PushModalAsync(new NavigationPage(new CatPage(userId, _tovarId)));
                }
            }
            else
            {
                await Navigation.PushModalAsync(new NavigationPage(new CatPage(userId, _tovarId)));
            }
        }
        private async Task UpdateCartItemQuantity(int tovarId, int newQuantity)
        {
            var repository = App.Database;
            var userId = this.userId;

            await repository.UpdateCartItemQuantityAsync(userId, tovarId, newQuantity);

            // После обновления количества пересчитываем и обновляем корзину
            await RefreshCartItems();
        }

        public async Task RefreshCartItems()
        {
            var repository = App.Database;
            cartItems = await repository.GetCartItemsAsync(userId);
            DisplayTovar(cartItems);

            // После обновления списка товаров в корзине пересчитываем общую цену
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            totalPrice = 0;

            foreach (var tovar in cartItems)
            {
                int selectedQuantity = selectedQuantities.ContainsKey(tovar.id) ? selectedQuantities[tovar.id] : 1;
                totalPrice += tovar.price * selectedQuantity;
            }

            totalPriceLabel.Text = $"Total Price: ${totalPrice:F2}";
        }
        private async void ClearCartButton_Clicked(object sender, EventArgs e)
        {
            var repository = App.Database;
            var userId = this.userId;

            // Clear the cart items for the current user
            await repository.ClearCartAsync(userId);

            // Display alert to confirm cart clearance
            await DisplayAlert("Очистка корзины", "Корзина была очищена", "OK");

            // Refresh the cart display after clearing
            await RefreshCartItems();
        }


    }
}