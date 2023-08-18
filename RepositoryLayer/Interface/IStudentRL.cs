﻿using CommonLayer.RequestModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface IStudentRL
    {
        public string RegisterStudent(StudentModel model);
        public StudentModel StudentDetails(int? Id);
        public StudentModel UpdateStudentInfo(StudentModel model);
        public string StudentLogin(LoginModel model, HttpContext httpContext);
        public IEnumerable<StudentModel> GetAllStudents();
    }
}
