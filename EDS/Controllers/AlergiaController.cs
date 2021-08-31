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
    public class AlergiaController : Controller
    {
        public IConfiguration Configuration { get; }

        public AlergiaController(IConfiguration configuration)
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
                    sqlQuery = $"exec sp_eliminar_alergia_paciente @IDC={id1}";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        command.ExecuteReader();
                        connection.Close();
                    }
                }
                return RedirectToAction("Listar", "Alergia");
            }
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Listar()
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null) 
            {
                List<AlergiaPaciente> alergias = new List<AlergiaPaciente>();
                string connectionString = Configuration["ConnectionStrings:DB_Connection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = $"exec sp_listar_alergia_paciente";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        SqlDataReader sqlDataReader = command.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            AlergiaPaciente alergia = new AlergiaPaciente();
                            alergia.CedulaPaciente = sqlDataReader["CEDULA_P"].ToString();
                            alergia.CedulaMedico = sqlDataReader["CEDULA_M"].ToString();
                            alergia.NombreAlergia = sqlDataReader["NOMBRE_ALERGIA"].ToString();
                            alergia.FechaD = sqlDataReader["FECHA_DIAGNOSTICO"].ToString();
                            alergia.Medicamentos = sqlDataReader["MEDICAMENTOS"].ToString();
                            alergia.ID = int.Parse(sqlDataReader["ID"].ToString());
                            alergias.Add(alergia);
                        }
                        connection.Close();
                    }
                }
                return View(alergias);
            }
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Registrar()
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
            {
                ViewBag.Alergias = GetAlergias();
                return View();
            }
            return RedirectToAction("Login", "Login");
        }       

        [HttpPost]
        public IActionResult Registrar(AlergiaPaciente alergiaPaciente)
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
            {
                string connectionString = "";
                if (ModelState.IsValid)
                {
                    connectionString = Configuration["ConnectionStrings:DB_Connection"];
                    var connection = new SqlConnection(connectionString);
                    string cedM = HttpContext.Session.GetString("CedulaMedico");
                    string sqlQuery = $"exec sp_insertar_alergia_paciente @CEDULA_M='{cedM}', @CEDULA_P='{alergiaPaciente.CedulaPaciente}',@NOMBRE_ALERGIA='{alergiaPaciente.NombreAlergia}',@FECHA_DIAGNOSTICO='{alergiaPaciente.FechaD}',@MEDICAMENTOS='{alergiaPaciente.Medicamentos}'";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        command.ExecuteReader();
                        connection.Close();
                    }
                    ViewBag.Q = sqlQuery;
                }
                ViewBag.Alergias = GetAlergias();
                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        private List<Alergia> GetAlergias()
        {
            List<Alergia> alergias = new List<Alergia>();
            String connectionString = Configuration["ConnectionStrings:DB_Connection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = $"exec sp_listar_alergias";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    SqlDataReader sqlDataReader = command.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        Alergia alergia = new Alergia();
                        alergia.Nombre = sqlDataReader["NOMBRE"].ToString();
                        alergia.Descripcion = sqlDataReader["DESCRIPCION"].ToString();
                        alergias.Add(alergia);
                    }
                    connection.Close();
                }
            }
            return alergias;
        }

    }
}

