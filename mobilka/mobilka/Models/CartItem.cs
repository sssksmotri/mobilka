using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobilka.Models
{
    public class CartItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int users_id { get; set; }
        public int tovar_id { get; set; }
        public int Quantity { get; set; } = 1;
        private int price { get; set; }
    }
}
