using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public class Establecimiento
    {
        public int EstablecimientoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int ProvinciaId { get; set; }
        public int TipoEstablecimientoId { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public int UsuarioCreacionId { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacionId { get; set; }
    }
}
