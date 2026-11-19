using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentManagment.Models
{
    public class Student
    {
        private int id;
        private string name;
        private string email;
        private int age;

        public Student()
        {
        }

        public Student(int id, string name, string email, int age)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.Age = age;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public int Age { get => age; set => age = value; }
    }
}