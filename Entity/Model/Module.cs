﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Module
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Create_Date { get; set; }
        public DateTime Delete_Date { get; set; }
        public DateTime Update_Date { get; set; }
    }
}
