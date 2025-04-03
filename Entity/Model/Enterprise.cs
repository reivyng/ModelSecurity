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
        public string NameBoss { get; set; }
        public string NameEnterprise { get; set; }
        public string PhoneEnterprise { get; set; }
        public string Locate { get; set; }
        public string EmailBoss { get; set; }
        public string NitEnterprise { get; set; }
        public string EmailEnterprise { get; set; }
        public bool Active { get; set; }
        public DateTime Create_Date { get; set; }
        public DateTime Delete_Date { get; set; }
        public DateTime Update_Date { get; set; }
    }
}
