using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace mobilka.Models
{
    public class Orders
    {
        
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }
            public int users_id { get; set; }
            public DateTime OrderDate { get; set; } = DateTime.Now;
            public int tovar_Id { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public int quantity { get; set; } = 1;
            public string Status { get; set; } // Add status field
             

    }
}
