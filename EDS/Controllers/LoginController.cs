using EDS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EDS.Controllers
{
    public class LoginController : Controller
    {

        public IConfiguration Configuration { get; }

        public LoginController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Medico medico)
        {
            String CED ="";
            string connectionString = Configuration["ConnectionStrings:DB_Connection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = $"exec sp_buscar_usuario @cedula_p = '{medico.Cedula}', @codigo_p = '{medico.Codigo}', @contrasenna_p = '{medico.Contraseña}'";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    SqlDataReader sqlDataReader = command.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        CED = sqlDataReader["CEDULA"].ToString();
                    }
                    connection.Close();
                }
            }

            if (CED == "ERROR") 
            {
                return View("Login");
            }
            else
            {
                HttpContext.Session.SetString("CedulaMedico", CED);
                
                return RedirectToAction("Index", "Home");
            }
        }
    }
}


