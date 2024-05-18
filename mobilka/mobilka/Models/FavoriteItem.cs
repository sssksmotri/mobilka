using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobilka.Models
{
    public class FavoriteItem
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public int users_id { get; set; }
        public int tovar_id { get; set; }
    }
}
