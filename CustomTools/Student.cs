using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTools
{
    internal class Student
    {
        public string StudentId { get; set; }
        public string EnrollmentYear { get; set; }
        public string Grade { get; set; }
        public string Class { get; set; }
        public string Sex { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Student(string StudentId, string EnrollmentYear, string Grade, string Class, string Sex, string Name, string Password)
        {
            this.StudentId = StudentId;
            this.EnrollmentYear = EnrollmentYear;
            this.Grade = Grade;
            this.Class = Class;
            this.Sex = Sex;
            this.Name = Name;
            this.Password = Password;
        }
    }
}
