using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Enterprise
    {
        public int Id { get; set; }
        public string Observation { get; set; }
        public string Name_Boss { get; set; }
        public string Name_Enterprise { get; set; }
        public string Phone_Enterprise { get; set; }
        public string Locate { get; set; }
        public string Email_Boss { get; set; }
        public string Nit_Enterprise { get; set; }
        public string Email_Enterprise { get; set; }
        public bool Active { get; set; }
        public DateTime Create_Date { get; set; }
        public DateTime Delete_Date { get; set; }
        public DateTime Update_Date { get; set; }
    }
}
