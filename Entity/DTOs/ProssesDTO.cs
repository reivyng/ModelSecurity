﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class ProssesDTO
    {
        public int Id { get; set; }
        public string TypeProcess { get; set; }
        //public DateTime UpdateDate { get; set; }
        //public DateTime CreateDate { get; set; }
        //public DateTime DeleteDate { get; set; }
        //public bool Active { get; set; }
        public DateTime StartAprendiz { get; set; }
        public string Observation { get; set; }
    }
}
