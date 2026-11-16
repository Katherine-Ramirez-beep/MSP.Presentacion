using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public class AlertaSeguridadTenant
    {
        public long AlertaId { get; set; }
        public DateTime FechaHora { get; set; }
        public string UsuarioOrigen { get; set; } = string.Empty;
        public int? ProvinciaInvolucradaId { get; set; }
        public string NombreProvincia { get; set; } = "Global";
        public string CategoriaAlerta { get; set; } = string.Empty;
        public string NivelRiesgo { get; set; } = "Informativo";
        public string MensajeTecnico { get; set; } = string.Empty;
        public bool Resuelta { get; set; }
    }
}
