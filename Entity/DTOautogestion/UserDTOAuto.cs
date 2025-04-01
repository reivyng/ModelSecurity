using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class UserDTOAuto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int PersonId { get; set; }
        //public DateTime DeleteDate { get; set; }
        //public DateTime UpdateDate { get; set; }
        //public DateTime CreateDate { get; set; }
        //public bool Active { get; set; }
    }
}
