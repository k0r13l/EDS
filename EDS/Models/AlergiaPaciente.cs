using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDS.Models
{
    public class AlergiaPaciente
    {
        public String CedulaPaciente { get; set; }
        public String CedulaMedico { get; set; }
        public String NombreAlergia { get; set; }
        public String FechaD { get; set; }
        public String Medicamentos { get; set; }
        public int ID { get; set; }
    }
}


