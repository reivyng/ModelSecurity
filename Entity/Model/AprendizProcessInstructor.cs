using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class AprendizProcessInstructor
    {
        public int Id { get; set; }
        public TypeModality TypeModality { get; set; }
        public RegisterySofia RegisterySofia { get; set; }
        public Concept Concept { get; set; }
        public Enterprise Enterprise { get; set; }
        public Process Process { get; set; }
        public Aprendiz Aprendiz { get; set; }
        public Instructor Instructor { get; set; }
        public State State { get; set; }
        public Verification Verification { get; set; }  
    }
}
