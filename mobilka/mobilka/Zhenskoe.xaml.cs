using mobilka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobilka
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Zhenskoe : ContentPage
    {
        Repository repository;
        private int userId;
        private List<Tovar> mens;
        private bool isHeartFilled = false;
        private int tovarId; List<Tovar> _kofta;
        public Zhenskoe(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            repository = App.Database;
        }
        private void ImageButton_Clicked1(object sender, EventArgs e)
        {
            Pidzhack_zh pidzhack_Zh = new Pidzhack_zh(this.userId);
            Navigation.PushModalAsync(new NavigationPage(pidzhack_Zh));
        }
        private void ImageButton_Clicked2(object sender, EventArgs e)
        {
            Zhenskie_bruki zhenskie_Bruki = new Zhenskie_bruki(this.userId);
            Navigation.PushModalAsync(new NavigationPage(zhenskie_Bruki));
        }
        private void ImageButton_Clicked3(object sender, EventArgs e)
        {
            ZhenskieTshirt zhenskieTshirt = new ZhenskieTshirt(this.userId);
            Navigation.PushModalAsync(new NavigationPage(zhenskieTshirt));
        }
        private void ImageButton_Clicked4(object sender, EventArgs e)
        {
            Platya platya = new Platya(this.userId);
            Navigation.PushModalAsync(new NavigationPage(platya));
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
        private void DisplaySearchResult(IEnumerable<Tovar> tovars)
        {
            searchResultsStack.Children.Clear();
            foreach (var tovar in tovars)
            {
                var frame = new Frame
                {
                    CornerRadius = 10,
                    HasShadow = true,
                    Margin = new Thickness(5),
                    WidthRequest = 240,
                    HeightRequest = 550
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

                var buttonLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center
                };

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

                var favoriteButton = new ImageButton
                {
                    BackgroundColor = Color.Transparent,
                    HeightRequest = 40,
                    WidthRequest = 40
                };
                SetInitialFavoriteButtonImage(favoriteButton, tovar);
                favoriteButton.Clicked += (sender, e) =>
                {
                    ToggleFavoriteButton(favoriteButton, tovar);
                };
                buttonLayout.Children.Add(addToCartButton);
                buttonLayout.Children.Add(favoriteButton);

                stackLayout.Children.Add(buttonLayout);

                frame.Content = stackLayout;
                searchResultsStack.Children.Add(frame);
            }
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
        private void SetInitialFavoriteButtonImage(ImageButton imageButton, Tovar tovar)
        {
            if (imageButton != null && tovar != null)
            {
                int tovarId = tovar.id;
                bool isFavorite = Preferences.Get($"IsFavorite_{userId}_{tovarId}", false);

                if (isFavorite)
                {
                    imageButton.Source = "izbran_filled.png"; // Установка закрашенного сердца
                }
                else
                {
                    imageButton.Source = "izbran.png"; // Установка пустого сердца
                }
            }
        }
        private void ToggleFavoriteButton(ImageButton imageButton, Tovar tovar)
        {
            if (imageButton != null && tovar != null)
            {
                int tovarId = tovar.id; // Получаем идентификатор товара

                // Получаем текущее сохраненное состояние избранного для данного товара
                bool isFavorite = Preferences.Get($"IsFavorite_{userId}_{tovarId}", false);

                if (isFavorite)
                {
                    // Удаляем товар из избранного
                    RemoveFromFavorites(userId, tovarId);
                    // Изменяем иконку на пустое сердце и устанавливаем цвет черный
                    imageButton.Source = "izbran.png";
                    isHeartFilled = false;
                }
                else
                {
                    // Добавляем товар в избранное
                    AddToFavorites(userId, tovarId);
                    // Изменяем иконку на закрашенное сердце и устанавливаем цвет красный
                    imageButton.Source = "izbran_filled.png"; // Предполагаем, что у вас есть изображение "izbran_filled.png" для закрашенного сердца
                    isHeartFilled = true;
                }

                // Сохраняем новое состояние избранного для данного товара
                Preferences.Set($"IsFavorite_{userId}_{tovarId}", !isFavorite);
            }
        }
        private async void AddToFavorites(int userId, int tovarId)
        {
            var repository = App.Database;
            var favoriteItem = new FavoriteItem
            {
                users_id = userId,
                tovar_id = tovarId
            };
            await repository.AddToFavoriteAsync(userId, tovarId); // Передаем оба параметра
            await DisplayAlert("Добавление в избранное", $"Товар  добавлен в избранное", "OK");
        }

        private async void RemoveFromFavorites(int userId, int tovarId)
        {
            var repository = App.Database;
            await repository.RemoveFromFavoriteAsync(userId, tovarId); // Передаем оба параметра
            await DisplayAlert("Удаление из избранного", $"Товар  удален из избранного", "OK");
        }
        private async void GoBack_Clicked(object sender, EventArgs e)
        {
            Mens mens = new Mens(userId);
            await Navigation.PushModalAsync(mens);
        }
        private async void poisk(object sender, EventArgs e)
        {
            string searchTerm = SearchEntry.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                await DisplayAlert("Ошибка", "Введите поисковой запрос", "OK");
                return;
            }

            try
            {
                Console.WriteLine($"Выполняется поиск по запросу: {searchTerm}");

                // Получаем отфильтрованные товары по запросу
                var filteredTovars = await repository.GetFilteredTovarsAsync(searchTerm);

                Console.WriteLine($"Найдено товаров: {filteredTovars.Count}");

                // Отображаем результаты поиска
                if (filteredTovars.Any())
                {
                    DisplaySearchResult(filteredTovars);
                }
                else
                {
                    await DisplayAlert("Ничего не найдено", $"По запросу '{searchTerm}' ничего не найдено", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка при выполнении поиска: {ex.Message}", "OK");
            }
        }
    }
}
    
