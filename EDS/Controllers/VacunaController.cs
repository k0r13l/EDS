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
    public class VacunaController : Controller
    {
        public IConfiguration Configuration { get; }

        public VacunaController(IConfiguration configuration)
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
                    sqlQuery = $"exec sp_eliminar_vacuna_paciente @IDC={id1}";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        command.ExecuteReader();
                        connection.Close();
                    }
                }
                return RedirectToAction("Listar", "Vacuna");
            }
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Listar()
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
            {
                List<VacunaPaciente> vacunas = new List<VacunaPaciente>();
                string connectionString = Configuration["ConnectionStrings:DB_Connection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = $"exec sp_listar_vacuna_paciente";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        SqlDataReader sqlDataReader = command.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            VacunaPaciente vacunaPaciente = new VacunaPaciente();
                            vacunaPaciente.CedulaP = sqlDataReader["CEDULA_P"].ToString();
                            vacunaPaciente.FechaAp = sqlDataReader["FECHA_APLICACION"].ToString();
                            vacunaPaciente.FechaPd = sqlDataReader["FECHA_P_DOSIS"].ToString();
                            vacunaPaciente.CedulaM = sqlDataReader["CEDULA_M"].ToString();
                            vacunaPaciente.NombreV = sqlDataReader["NOMBRE_VACUNA"].ToString();
                            vacunaPaciente.ID = int.Parse(sqlDataReader["ID"].ToString());
                            vacunas.Add(vacunaPaciente);
                        }
                        connection.Close();
                    }
                }
                return View(vacunas);
            }
            return RedirectToAction("Login", "Login");
        }

        public IActionResult RegistrarVacuna()
        {
            if (HttpContext.Session.GetString("CedulaMedico") != null)
            {
                List<Vacuna> vacunas = new List<Vacuna>();
                string connectionString = Configuration["ConnectionStrings:DB_Connection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = $"exec sp_listar_vacunas";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        connection.Open();
                        SqlDataReader sqlDataReader = command.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            Vacuna vacuna = new Vacuna();
                            vacuna.Nombre = sqlDataReader["NOMBRE"].ToString();
                            vacuna.Descripcion = sqlDataReader["DESCRIPCION"].ToString();
                            vacunas.Add(vacuna);
                        }
                    }
                }
                ViewBag.Vacunas = vacunas;
                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public IActionResult RegistrarVacuna(VacunaPaciente vacunaPaciente)
        {
            string connectionString = Configuration["ConnectionStrings:DB_Connection"];
            if (ModelState.IsValid)
            {
                var connection = new SqlConnection(connectionString);
                string cedM = HttpContext.Session.GetString("CedulaMedico");
                string sqlQuery = $"exec sp_insertar_vacuna_paciente @NOMBRE_V='{vacunaPaciente.NombreV}', @CEDULA='{vacunaPaciente.CedulaP}',@CEDULAM='{cedM}',@FECHA_A='{vacunaPaciente.FechaAp}',@FECHA_P='{vacunaPaciente.FechaPd}'";
               using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    command.ExecuteReader();
                    connection.Close();
                }
            }
            List<Vacuna> vacunas = new List<Vacuna>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = $"exec sp_listar_vacunas";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    connection.Open();
                    SqlDataReader sqlDataReader = command.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        Vacuna vacuna = new Vacuna();
                        vacuna.Nombre = sqlDataReader["NOMBRE"].ToString();
                        vacuna.Descripcion = sqlDataReader["DESCRIPCION"].ToString();
                        vacunas.Add(vacuna);
                    }
                }
            }
            ViewBag.Vacunas = vacunas;
            return View();
        }

    }
}


