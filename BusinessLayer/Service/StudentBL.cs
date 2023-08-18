using BusinessLayer.Interface;
using CommonLayer.RequestModel;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Service
{
    public class StudentBL: IStudentBL
    {
        private readonly IStudentRL studentRL;
        public StudentBL(IStudentRL studentRL)
        {
            this.studentRL = studentRL;
        }

        public string RegisterStudent(StudentModel model)
        {
            return studentRL.RegisterStudent(model);
        }
        public StudentModel StudentDetails(int? Id)
        {
            return studentRL.StudentDetails(Id);
        }
        public StudentModel UpdateStudentInfo(StudentModel model)
        {
           return studentRL.UpdateStudentInfo(model);
        }
        public string StudentLogin(LoginModel model, HttpContext httpContext)
        {
            return studentRL.StudentLogin(model, httpContext);
        }
        public IEnumerable<StudentModel> GetAllStudents()
        {
            return studentRL.GetAllStudents();
        }
    }
}
