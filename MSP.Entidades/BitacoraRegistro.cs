using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public class BitacoraRegistro
    {
        public long BitacoraId { get; set; }
        public DateTime FechaHora { get; set; }
        public int? UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string? Rol { get; set; }
        public int? ProvinciaId { get; set; }
        public string Modulo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
