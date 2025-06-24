using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace ASSNlearningManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly string connectionString = "server=localhost;user=root;password=root;database=lmsdb";

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT role_id FROM user WHERE username = @username AND password = @password";
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", user.Password);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            int roleId = Convert.ToInt32(result);

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

        // Optional default welcome page
        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }
    }
}