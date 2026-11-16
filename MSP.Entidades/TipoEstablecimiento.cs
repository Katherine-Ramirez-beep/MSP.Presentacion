using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public class TipoEstablecimiento
    {
        public int TipoEstablecimientoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }

        public string NombreMostrar => $"[{Codigo}] {Nombre}";
    }
}
