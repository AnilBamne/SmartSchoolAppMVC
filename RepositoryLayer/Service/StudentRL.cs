using CommonLayer;
using CommonLayer.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace RepositoryLayer.Service
{
    public class StudentRL: IStudentRL
    {
        private readonly IConfiguration configuration;
        private readonly string ConnectionString;
        private readonly SqlConnection connection = new SqlConnection();
        public StudentRL(IConfiguration configuration)
        {
            this.configuration = configuration;
            ConnectionString = this.configuration.GetConnectionString("DBConnect");
            connection.ConnectionString= ConnectionString;
        }

        public string RegisterStudent(StudentModel model)
        {
            try
            {
                using (connection)
                {
                    string regNo="";
                    SqlCommand cmd = new SqlCommand("spRegisterStudent", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("LastName", model.LastName);
                    cmd.Parameters.AddWithValue("Email", model.Email);
                    connection.Open();
                    // int count=cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            regNo = reader["RegisterNumber"].ToString();
                           // return regNo;
                        }
                        return regNo;
                    }
                    else
                    {
                        return "Student Registration Failed";
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
        public StudentModel StudentDetails(int? Id)
        {
            StudentModel studentModel = new StudentModel();
            try
            {
                using (connection)
                {
                    string query = "Select * from StudentTable where Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@Id", Id);
                    
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            studentModel.Id = reader.IsDBNull(0)?0:reader.GetInt32(0);
                            studentModel.FirstName = reader.IsDBNull(1)?string.Empty:reader.GetString(1);
                            studentModel.LastName = reader.IsDBNull(2)?String.Empty:reader.GetString(2);
                            studentModel.Class = reader.IsDBNull(3) ? String.Empty : reader.GetString(3);
                            //studentModel.DOB = reader.IsDBNull(4) ? DateTime.Now.ToString("dd/MM/YYYY", CultureInfo.InvariantCulture) : reader.GetDateTime(4);
                            studentModel.DOB = reader.IsDBNull(4) ?DateTime.Now :reader.GetDateTime(4);
                            studentModel.RegistrationNumber = reader.IsDBNull(5) ? String.Empty : reader.GetString(5);
                            studentModel.Address = reader.IsDBNull(6)?string.Empty:reader.GetString(6);
                            studentModel.PhoneNumber = reader.IsDBNull(7)?0:reader.GetInt64(7);
                            studentModel.Email = reader.IsDBNull(8) ? string.Empty:reader.GetString(8);
                            studentModel.Gender = reader.IsDBNull(12) ? string.Empty:reader.GetString(12);
                        }
                        return studentModel;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
        public StudentModel UpdateStudentInfo(StudentModel model)
        {
            try
            {
                using (connection)
                {
                    SqlCommand cmd = new SqlCommand("spUpdateStudentInfo", connection);
                    cmd.CommandType=CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("Id", model.Id);
                    cmd.Parameters.AddWithValue("FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("LastName", model.LastName);
                    cmd.Parameters.AddWithValue("Class", model.Class);
                    cmd.Parameters.AddWithValue("DOB", model.DOB);
                    cmd.Parameters.AddWithValue("RegistrationNumber", model.RegistrationNumber);
                    cmd.Parameters.AddWithValue("Address", model.Address);
                    cmd.Parameters.AddWithValue("PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("Email", model.Email);
                    cmd.Parameters.AddWithValue("Gender", model.Gender);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        StudentModel studentModel = new StudentModel();
                        while (reader.Read())
                        {
                            
                            studentModel.Id = reader.GetInt32(0);
                            studentModel.FirstName = reader.GetString(1);
                            studentModel.LastName = reader.GetString(2);
                            studentModel.Class = reader.GetString(3);
                            studentModel.DOB = reader.GetDateTime(4);
                            studentModel.RegistrationNumber = reader.GetString(5);
                            studentModel.Address = reader.GetString(6);
                            studentModel.PhoneNumber = reader.GetInt64(7);
                            studentModel.Email = reader.GetString(8);
                            studentModel.Gender = reader.GetString(12);
                        }
                        return studentModel;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public string StudentLogin(LoginModel model, HttpContext httpContext)
        {
            try
            {
                using (connection)
                {
                    SqlCommand cmd = new SqlCommand("spStudentLogin",connection);
                    cmd.CommandType=CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@email",model.Email);
                    cmd.Parameters.AddWithValue("@registrationNumber", model.RegistrationNumber);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int studentId = Convert.ToInt32(reader["Id"]);
                            httpContext.Session.SetInt32("studentId", studentId);
                            httpContext.Session.SetString("email", model.Email);
                            string token = GenerateToken(model.Email, studentId);
                            return token;
                        }
                    }
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public string GenerateToken(string emailId,long userId)
        {
            var securityKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var crediantials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Email",emailId),
                new Claim("UserId",userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials:crediantials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        } 

        public IEnumerable<StudentModel> GetAllStudents()
        {
            try
            {
                using (connection)
                {
                    string query = "Select * from StudentTable where IsTrash=0";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        List<StudentModel> studentlist = new List<StudentModel>();
                        while (reader.Read())
                        {
                            StudentModel studentModel = new StudentModel();
                            studentModel.Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            studentModel.FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            studentModel.LastName = reader.IsDBNull(2) ? String.Empty : reader.GetString(2);
                            studentModel.Class = reader.IsDBNull(3) ? String.Empty : reader.GetString(3);
                            //studentModel.DOB = reader.IsDBNull(4) ? DateTime.Now.ToString("dd/MM/YYYY", CultureInfo.InvariantCulture) : reader.GetDateTime(4);
                            studentModel.DOB = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4);
                            studentModel.RegistrationNumber = reader.IsDBNull(5) ? String.Empty : reader.GetString(5);
                            studentModel.Address = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                            studentModel.PhoneNumber = reader.IsDBNull(7) ? 0 : reader.GetInt64(7);
                            studentModel.Email = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);
                            studentModel.Gender = reader.IsDBNull(12) ? string.Empty : reader.GetString(12);
                            studentlist.Add(studentModel);
                        }
                        return studentlist;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                connection.Close();
            }
        }

        public StudentTicketModel CreateTicketForPassword(string email,string token)
        {
            StudentTicketModel model= new StudentTicketModel();
            return model;
        }
    }
}
