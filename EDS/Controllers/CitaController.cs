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
    public class CitaController : Controller
    {
        public IConfiguration Configuration { get; }

        public CitaController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Eliminar(int id1)
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
            {
                string sqlQuery = "";
                if (ModelState.IsValid)
                {
                    string connectionString = Configuration["ConnectionStrings:DB_Connection"];
                    var connection = new SqlConnection(connectionString);
                    sqlQuery = $"exec sp_eliminar_cita @IDC={id1}";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        command.ExecuteReader();
                        connection.Close();
                    }
                }
                return RedirectToAction("Listar", "Cita");
            }
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Listar()
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
            {
                List<Cita> citas = new List<Cita>();
                string connectionString = Configuration["ConnectionStrings:DB_Connection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = $"exec sp_listar_citas_ALL";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        SqlDataReader sqlDataReader = command.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            Cita cita = new Cita();
                            cita.Cedula = sqlDataReader["CEDULA_P"].ToString();
                            cita.CentroSalud = sqlDataReader["CENTRO_SALUD"].ToString();
                            cita.FechaH = sqlDataReader["FECHA_H"].ToString();
                            cita.Especialidad = sqlDataReader["ESPECIALIDAD"].ToString();
                            cita.CedulaMedico = sqlDataReader["CEDULA_M"].ToString();
                            cita.Descripcion = sqlDataReader["DESCRIPCION"].ToString();
                            cita.ID = int.Parse(sqlDataReader["ID"].ToString());
                            citas.Add(cita);
                        }
                        connection.Close();
                    }
                }
                return View(citas);
            }
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Editar(int id1)
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
            {
                HttpContext.Session.SetInt32("idCita", id1);
                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public IActionResult Editar(String Descripcion)
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
            {
                string sqlQuery = "";
                if (ModelState.IsValid)
                {
                    string connectionString = Configuration["ConnectionStrings:DB_Connection"];
                    var connection = new SqlConnection(connectionString);
                    int idCita = int.Parse(HttpContext.Session.GetInt32("idCita").ToString());
                    sqlQuery = $"exec sp_actualizar_desc_cita @IDP={idCita}, @DESCRIP='{Descripcion}'";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        command.ExecuteReader();
                        connection.Close();
                    }
                }
                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        public ActionResult Registrar()
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
                return View();
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public ActionResult Registrar(Cita cita)
        {
            if (ModelState.IsValid)
            {
                string connectionString = Configuration["ConnectionStrings:DB_Connection"];
                var connection = new SqlConnection(connectionString);
                string cedM = HttpContext.Session.GetString("CedulaMedico");
                string sqlQuery = $"exec sp_insertar_cita @CEDM='{cedM}', @CEDP='{cita.Cedula}',@CENTRO_S='{cita.CentroSalud}',@FECHA='{cita.FechaH}',@ESP='{cita.Especialidad}'";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    command.ExecuteReader();
                    connection.Close();
                }
            }
            return View();
        }

        
    }
}


