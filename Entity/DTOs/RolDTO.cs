﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class RolDTO
    {
        public int Id { get; set; }
        public string TypeRol { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
