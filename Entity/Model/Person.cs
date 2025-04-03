using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string First_Name { get; set; }
        public string Second_Name { get; set; }
        public string First_Last_Name { get; set; }
        public string Second_Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Email { get; set; }
        public string Type_Identification { get; set; }
        public int Number_Identification { get; set; }
        public bool Signig { get; set; }
    }
}
