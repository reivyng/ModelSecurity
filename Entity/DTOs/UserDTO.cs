﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class UserDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int PersonId { get; set; }
    }
}
