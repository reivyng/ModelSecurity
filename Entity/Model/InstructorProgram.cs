using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class InstructorProgram
    {
        public int Id { get; set; }
        public Instructor Instructor { get; set; }
        public Program Program { get; set; }
    }
}
