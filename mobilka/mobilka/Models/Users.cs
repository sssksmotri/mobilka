using System;
using System.Collections.Generic;
using System.Text;
using mobilka.Views;
using SQLite;

namespace mobilka.Models
{
    

    public class Users
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public string name { get; set; }
        public string surname { get; set; }
        public string number { get; set; }
        public DateTime date_of_birthday { get; set; }
        public string gender { get; set; }
        public string login {  get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string image_path { get; set; }
    }
}
