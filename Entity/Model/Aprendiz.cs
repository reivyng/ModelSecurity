﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Aprendiz
    {
        public int Id { get; set; }
        public string PreviuosProgram { get; set; }
        public bool Active { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int AprendizProgramId { get; set; }
        public ICollection<AprendizProgram> AprendizProgram { get; set; }
        public ICollection<AprendizProcessInstructor> AprendizProcessInstructor { get; set; }
        public int AprendizProcessInstructorId { get; set; }
    }
}
