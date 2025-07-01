using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace ASSNlearningManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly string connectionString = "server=localhost;user=root;password=root;database=lmsdb";

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT role_id, user_id FROM user WHERE username = @username AND password = @password";
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", user.Password);

                        using var reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int roleId = Convert.ToInt32(reader["role_id"]);
                            int userId = Convert.ToInt32(reader["user_id"]);

                            HttpContext.Session.SetInt32("UserId", userId);

                            switch (roleId)
                            {
                                case 1:
                                    return RedirectToAction("Dashboard", "Admin");
                                case 2:
                                    return RedirectToAction("Dashboard", "Trainer");
                                case 3:
                                    return RedirectToAction("Dashboard", "Learner");
                                default:
                                    ViewBag.ErrorMessage = "User role unknown.";
                                    return View();
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Invalid username or password.";
                            return View();
                        }
                    }
                }
            }

            return View();
        }


        public IActionResult Welcome() => View();
        public IActionResult Index() => RedirectToAction("Login");
    }
}
