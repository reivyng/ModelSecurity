using Entity.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Boss
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailBoss { get; set; }
        public string PhoneNumberBoss { get; set; }
        public ICollection<Enterprise> Enterprise { get; set; }
    }
}
