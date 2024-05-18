using SQLite;
using System;
using System.Collections.Generic;
using System.Text;


namespace mobilka.Models
{
    public class Tovar
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public string name { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public string gender { get; set; }
        public string brend { get; set; }
        public string image_tovar { get; set; }
        public decimal price { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
