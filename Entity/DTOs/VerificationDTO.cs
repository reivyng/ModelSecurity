﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class VerificationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Observation { get; set; }
        public bool Active { get; set; }
    }
}
