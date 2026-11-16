using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public class ConsolidadoProvinciaDto
    {
        public int ProvinciaId { get; set; }
        public string CodigoProvincia { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string RegionSalud { get; set; } = string.Empty;
        public int TotalEstablecimientos { get; set; }
        public int Activos { get; set; }
        public int Inactivos { get; set; }
    }
}

