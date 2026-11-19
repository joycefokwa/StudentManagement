using StudentManagment.Models; 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace StudentManagment.Controllers
{
    public class StudentController : Controller
    {
        // for having list of studnets I should connect to database 
        // connection string with right address 
        private string connectionString = ConfigurationManager.ConnectionStrings["DemoApp1"].ConnectionString;

        // GET: Student
        public ActionResult Index()
        {
            // I can have list of my students-- Read all students in the table 
            // select * 
            // I will receive data from database with associative array format 
            // I have student class each row in my database belongs to one student 
            // one row = one object of student class 
            // if I have bunch of objects I need a list 
            var studentList = new List<Student>();

            SqlConnection conn = new SqlConnection(connectionString);
            // create my command  write query 
            string commandSelect = "SELECT * FROM Students";
            SqlCommand  cmd = new SqlCommand(commandSelect, conn);

            conn.Open();
            // I want to get data from database . I want actual date real data 

            var reader = cmd.ExecuteReader();

            // c# will connect to the database. read table Students put data in reader 
            
            while (reader.Read())
            {
                // go read data to the end do not miss any row 

                Student std = new Student();
                std.Id = Convert.ToInt32(reader["Id"]);
                std.Name = reader["Name"].ToString();
                std.Email = reader["Email"].ToString();
                std.Age = Convert.ToInt32(reader["Age"]);

                studentList.Add(std);
            }

            return View(studentList);
        }

        // GET: Student/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            // this one is used to show the Create Form
            // show the create form when is empty no data 
            // each form that I generate = each student object 
            //
            return View(new Student());
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student model)
        {
            // is used when the user submit the form 
            // completely filled form + all the credentials  and all the data for the user 
            // I want to inser data in students table 
            // we will pass object of student in parameters ( I chhosed model name to be consistence with create page)
            try
            {
               InsertStudent(model);

                // if I create or I inserted new row I want to see in the table 
                // Where is the table ??/ it is in index 
                // please take me to the index 
                return RedirectToAction("Index");

            }
            catch(Exception ex) 
            {
                // if I have an error please show the message in the webpage 
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }
        }

        // GET: Student/Edit/5
        public ActionResult Edit(int id)
        {
            // publish or show the edit page (you need Id)
            // Find the student That I want to edit data ---- With StudentId 
            //send Student Object with the requested studentId
            // I need a function to search get the student with Id and bring the data to the edit page 
            Student stdToEdit = GetStudentByID(id);
            if (stdToEdit == null) return HttpNotFound(); /// true or false 
            return View(stdToEdit); // what I will send to the view 
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //FormCollection collection i will replace it with Student student , I will pass object of student
        public ActionResult Edit(int id, Student student)
        {
            try
            {
                // TODO: Add update logic here
                SqlConnection connection = new SqlConnection(connectionString);
                string stdToUpdate = "UPDATE Students SET Name=@name , Email=@email, Age=@age WHERE Id=@id";

                SqlCommand cmdUpdate = new SqlCommand(stdToUpdate, connection);
                cmdUpdate.Parameters.AddWithValue("@id", student.Id);
                cmdUpdate.Parameters.AddWithValue("@name", student.Name);
                cmdUpdate.Parameters.AddWithValue("@email", student.Email);
                cmdUpdate.Parameters.AddWithValue("@age", student.Age);

                // connected mode 
                connection.Open();
                int affectedRow = cmdUpdate.ExecuteNonQuery();

                if(affectedRow == 0) return HttpNotFound();

                // else I will shoe the edited student info in index 
                // update was successful I can see the result 

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Student/Delete/5 //  confirmation page 
        public ActionResult Delete(int id)
        {
            // I want to delete the student
            // Before deleting the studnet I should find the Id of student then I can Delete 
            Student studToDelete = GetStudentByID(id);
            // we could not find the student 
            if (studToDelete == null) return HttpNotFound();
            return View(studToDelete);
        }

        // POST: Student/Delete/5
        [HttpPost]

        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                SqlConnection conn = new SqlConnection(connectionString);
                string deletQuery = "DELETE FROM Students WHERE Id=@id";

                SqlCommand cmdDelete = new SqlCommand(deletQuery,conn);
                cmdDelete.Parameters.AddWithValue("@Id", id);

                conn.Open();

                int affectedRow = cmdDelete.ExecuteNonQuery();
                if (affectedRow == 0) return HttpNotFound();


                // I will back to the Index page and check the table that data is gone
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public void InsertStudent(Student student)
        {
            // validation 
                // create an object of SqlConnection when job is done 
                // when the student is inserted dismis the connection object 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                // Id is auto increament 
                // Why I am using variable ??? because I will get the data from user in the create page

                // what is the solution to avoid 
                // we have methods to run the query 
                // Method Lambda to create query 
                // create command object 
                connection.Open();
                string insertQuery = "INSERT INTO Students (Name,Email,Age) VALUES" +
                                         "(@Name,@Email,@Age)";
                SqlCommand inserCommand = new SqlCommand(insertQuery, connection);


                // I should set the parameters 
               // inserCommand.Parameters.AddWithValue("@Id", student.Id);
                inserCommand.Parameters.AddWithValue("@Name", student.Name);
                inserCommand.Parameters.AddWithValue("@Email", student.Email);
                inserCommand.Parameters.AddWithValue("@Age", student.Age);

                inserCommand.ExecuteNonQuery();

            }
        }

        private Student GetStudentByID(int id)
        {
            // Search ID 
            // query SELECT * FROM Students WHERE studentId = id 

            SqlConnection connection = new SqlConnection(connectionString);

            string selectById = "SELECT * FROM Students WHERE Id= @id";
            SqlCommand cmdSelectByID = new SqlCommand(selectById, connection);

            cmdSelectByID.Parameters.AddWithValue("@id", id);

            // connected mode 
            connection.Open();

            // I need to read data , I want to retrive data 
            SqlDataReader reader = cmdSelectByID.ExecuteReader();

            // validation 
            // if reader doe not has a data or return null data 
            if (!reader.Read()) return null;// we didnot do anything for null value 
            // we did not handle the null value 

            else
                return new Student
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Age = Convert.ToInt32(reader["Age"])
                };
        }
    }
}
