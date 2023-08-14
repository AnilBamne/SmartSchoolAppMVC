using BusinessLayer.Interface;
using CommonLayer.RequestModel;
using Microsoft.AspNetCore.Mvc;
using System;

namespace SmartSchoolAppMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentBL studentBL;
        public StudentController(IStudentBL studentBL)
        {
            this.studentBL = studentBL;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(StudentModel model)
        {
            try
            {
                studentBL.RegisterStudent(model);
                return View(model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpGet]
        public IActionResult StudentDetails(int Id)
        {
            if(Id == null)
            {
                return NotFound();
            }
            StudentModel studentModel = studentBL.StudentDetails(Id);
            if(studentModel == null)
            {
                return null;
            }
            return View(studentModel);
        }

        //[HttpGet]
        //public IActionResult Update(int? studentId)
        //{
        //    if(studentId == null)
        //    {
        //        return NotFound();
        //    }
        //    StudentModel model = studentBL.UpdateStudentInfo(studentId);
        //}
    }
}
