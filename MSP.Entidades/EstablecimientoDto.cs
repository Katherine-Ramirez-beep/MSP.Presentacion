using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public class EstablecimientoDto
    {
        public int EstablecimientoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int ProvinciaId { get; set; }
        public string Provincia { get; set; } = string.Empty;
        public string RegionSalud { get; set; } = string.Empty;
        public int TipoEstablecimientoId { get; set; }
        public string TipoEstablecimiento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string Estado => Activo ? "Activo" : "Inactivo";
    }

}
