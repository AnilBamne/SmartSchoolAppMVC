using BusinessLayer.Interface;
using CommonLayer.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace SmartSchoolAppMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentBL studentBL;
        public StudentController(IStudentBL studentBL)
        {
            this.studentBL = studentBL;
        }
        public IActionResult Index()
        {
            return View();
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
        public IActionResult StudentDetails(int? Id)
        {
            Id = HttpContext.Session.GetInt32("studentId");
            
            if(Id == null)
            {
                return NotFound();
            }
            StudentModel studentModel = studentBL.StudentDetails(Id);
            if(studentModel == null)
            {
                return null;
            }

            string email = HttpContext.Session.GetString("email");
            ViewBag.UserEmail = email;

            //int Id = Convert.ToInt32(User.Claims.FirstOrDefault(a => a.Type == "UserId").Value);
            //HttpContext.Session.SetInt32("StudentId", Id);
            return View(studentModel);
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            StudentModel model = studentBL.StudentDetails(Id);
            if(model == null)
            {
                return null;
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(int Id, [Bind]StudentModel model)
        {
            if (Id != model.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                studentBL.UpdateStudentInfo( model);
                return View(model);
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult StudentLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult StudentLogin(LoginModel model)
        {
            try
            {  
                if(model.Email == null || model.RegistrationNumber == null)
                {
                    return NotFound();
                }
                var result=studentBL.StudentLogin(model,HttpContext);
                HttpContext.Session.SetString("token", result);

                

                return RedirectToAction("StudentDetails");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult GetAllStudents()
        {
            try
            {
                var result=studentBL.GetAllStudents();
                return View(result);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
