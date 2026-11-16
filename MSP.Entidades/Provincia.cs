using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public class Provincia
    {
        public int ProvinciaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string RegionSalud { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }

        public string NombreMostrar => $"{Codigo} - {Nombre}";
    }
}
