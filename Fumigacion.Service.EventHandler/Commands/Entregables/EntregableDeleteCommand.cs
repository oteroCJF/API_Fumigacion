using System;
using System.Collections.Generic;
using System.Text;

namespace Fumigacion.Service.EventHandler.Commands.Entregables
{
    public class EntregableDeleteCommand
    {
        public string Archivo { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string Folio { get; set; }
    }
}
