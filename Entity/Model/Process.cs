using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Process
    {
        public int Id { get; set; }
        public string Start_Aprendiz { get; set; }
        public string Observation { get; set; }
        public string Type_Procces { get; set; }
        public bool Active { get; set; }
        public DateTime Create_Date { get; set; }
        public DateTime Delete_Date { get; set; }
        public DateTime Update_Date { get; set; }
    }
}
