using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public int? ProvinciaId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public string Rol { get; set; } = "Consultor"; 
        public int IntentosFallidos { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime? UltimoAcceso { get; set; }
        public DateTime FechaCreacion { get; set; }

        public string NombreProvincia { get; set; } = "Nivel Nacional";
        public bool EsAdminNacional => Rol == "AdminNacional";
    }
}
