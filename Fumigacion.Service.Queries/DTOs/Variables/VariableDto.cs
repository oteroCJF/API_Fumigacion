using System;
using System.Collections.Generic;
using System.Text;

namespace Fumigacion.Service.Queries.DTOs.Variables
{
    public class VariableDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Abreviacion { get; set; }
        public string Valor { get; set; }
    }
}
