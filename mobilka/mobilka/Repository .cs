using mobilka.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace mobilka
{
    public class Repository
    {
        readonly SQLiteAsyncConnection _database;

        public Repository(string databasePath)
        {
            _database = new SQLiteAsyncConnection(databasePath);
            _database.CreateTableAsync<Users>().Wait();
            _database.CreateTableAsync<Tovar>().Wait();
            _database.CreateTableAsync<CartItem>().Wait();
            _database.CreateTableAsync<Orders>().Wait();
            _database.CreateTableAsync<FavoriteItem>().Wait();
        }


        public async Task<Users> GetUserAsync(int id)
        {
            return await _database.Table<Users>().Where(u => u.id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserAsync(Users user)
        {
            if (user.id != 0)
            {
                return await _database.UpdateAsync(user);
            }
            else
            {
                return await _database.InsertAsync(user);
            }
        }

        public async Task<int> DeleteUserAsync(Users user)
        {
            return await _database.DeleteAsync(user);
        }

        public async Task<List<Tovar>> GetTovarsAsync()
        {
            return await _database.Table<Tovar>().ToListAsync();
        }

        public async Task<Tovar> GetTovarAsync(int id)
        {
            return await _database.Table<Tovar>().Where(t => t.id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveTovarAsync(Tovar tovar)
        {
            if (tovar.id != 0)
            {
                return await _database.UpdateAsync(tovar);
            }
            else
            {
                return await _database.InsertAsync(tovar);
            }
        }
        public async Task<Users> GetUserByLoginAsync(string login)
        {
            return await _database.Table<Users>().Where(u => u.login == login).FirstOrDefaultAsync();
        }
        public async Task<Users> AuthenticateUserAsync(string login, string password)
        {
            try
            {
                var user = await GetUserByLoginAsync(login);

                if (user != null)
                {
                    string hashedPassword = PasswordHasher.HashPassword(password);
                    if (user.password == hashedPassword)
                    {
                        return user;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
            }

            return null;
        }
        public async Task<int> RemoveFromCartAsync(int userId, int tovarId)
        {
            var cartItem = await _database.Table<CartItem>()
                                           .Where(c => c.users_id == userId && c.tovar_id == tovarId)
                                           .FirstOrDefaultAsync();
            if (cartItem != null)
                return await _database.DeleteAsync(cartItem);

            return 0;
        }
        public async Task<List<Tovar>> GetCartItemsAsync(int userId)
        {
            var cartItems = await _database.Table<CartItem>()
                                            .Where(c => c.users_id == userId)
                                            .ToListAsync();

            var tovarIds = cartItems.Select(c => c.tovar_id).ToList();
            return await _database.Table<Tovar>()
                                   .Where(t => tovarIds.Contains(t.id))
                                   .ToListAsync();
        }
        

        

        
        public async Task<List<Orders>> GetOrdersAsync()
        {
            return await _database.Table<Orders>().ToListAsync();
        }

        public async Task<int> SaveOrderAsync(Orders order)
        {
            if (order.id != 0)
            {
                return await _database.UpdateAsync(order);
            }
            else
            {
                return await _database.InsertAsync(order);
            }
        }
        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            return await _database.Table<CartItem>().ToListAsync();
        }

        public async Task<int> SaveCartItemAsync(CartItem cartItem)
        {
            if (cartItem.Id != 0)
            {
                return await _database.UpdateAsync(cartItem);
            }
            else
            {
                return await _database.InsertAsync(cartItem);
            }
        }

        public async Task<int> DeleteCartItemAsync(CartItem cartItem)
        {
            return await _database.DeleteAsync(cartItem);
        }
        

        

        
        public async Task<List<Tovar>> GetFilteredTovarsAsync(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();

            var repository = App.Database; 
            var allTovars = await repository.GetTovarsAsync();
            var filteredTovars = allTovars
                .Where(t => t.brend.ToLower().Contains(searchTerm) || t.name.ToLower().Contains(searchTerm))
                .ToList();

            return filteredTovars;
        }
        public async Task<int> UpdateCartItemAsync(CartItem cartItem)
        {
            return await _database.UpdateAsync(cartItem);
        }
        public async Task<CartItem> GetCartItemAsync(int userId, int tovarId)
        {
            return await _database.Table<CartItem>()
                                   .Where(c => c.users_id == userId && c.tovar_id == tovarId)
                                   .FirstOrDefaultAsync();
        }
        public async Task<int> UpdateCartItemQuantityAsync(int userId, int tovarId, int newQuantity)
        {
            var cartItem = await _database.Table<CartItem>()
                                           .Where(c => c.users_id == userId && c.tovar_id == tovarId)
                                           .FirstOrDefaultAsync();
            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
                return await _database.UpdateAsync(cartItem);
            }

            return 0;
        }

        public async Task<int> AddCartItemAsync(CartItem cartItem)
        {
            var existingCartItem = await _database.Table<CartItem>()
                                                   .Where(c => c.users_id == cartItem.users_id && c.tovar_id == cartItem.tovar_id)
                                                   .FirstOrDefaultAsync();

            if (existingCartItem != null)
            {
                // Увеличиваем количество товара в корзине
                existingCartItem.Quantity += cartItem.Quantity;
                return await _database.UpdateAsync(existingCartItem);
            }
            else
            {
                // Добавляем новый элемент корзины
                return await _database.InsertAsync(cartItem);
            }
        }
        public async Task<List<Tovar>> GetFavoriteTovarsAsync(int userId)
        {
            var favoriteItems = await _database.Table<FavoriteItem>()
                                               .Where(fi => fi.users_id == userId)
                                               .ToListAsync();

            var favoriteTovarIds = favoriteItems.Select(fi => fi.tovar_id).ToList();

            return await _database.Table<Tovar>()
                                   .Where(t => favoriteTovarIds.Contains(t.id))
                                   .ToListAsync();
        }

        public async Task<bool> AddToFavoriteAsync(int userId, int tovarId)
        {
            var existingFavorite = await _database.Table<FavoriteItem>()
                                                   .Where(fi => fi.users_id == userId && fi.tovar_id == tovarId)
                                                   .FirstOrDefaultAsync();

            if (existingFavorite == null)
            {
                var favoriteItem = new FavoriteItem
                {
                    users_id = userId,
                    tovar_id = tovarId
                };

                await _database.InsertAsync(favoriteItem);
                return true; // Added to favorites
            }

            return false; // Already in favorites
        }

        public async Task<bool> RemoveFromFavoriteAsync(int userId, int tovarId)
        {
            var existingFavorite = await _database.Table<FavoriteItem>()
                                                   .Where(fi => fi.users_id == userId && fi.tovar_id == tovarId)
                                                   .FirstOrDefaultAsync();

            if (existingFavorite != null)
            {
                await _database.DeleteAsync(existingFavorite);
                return true; // Removed from favorites
            }

            return false; // Not found in favorites
        }
        public async Task ClearCartAsync(int userId)
        {
            // Получаем все элементы корзины для указанного пользователя
            var cartItems = await _database.Table<CartItem>()
                                           .Where(c => c.users_id == userId)
                                           .ToListAsync();

            // Удаляем все элементы корзины из базы данных
            foreach (var cartItem in cartItems)
            {
                await _database.DeleteAsync(cartItem);
            }
        }
    }
}
