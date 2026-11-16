using System;
using System.Collections.Generic;
using System.Text;

namespace MSP.Entidades
{
    public sealed class TenantContext
    {
        private static readonly Lazy<TenantContext> _instancia = new(() => new TenantContext());
        public static TenantContext Actual => _instancia.Value;

        private TenantContext() { }

        public Usuario? UsuarioActual { get; private set; }
        public bool EstaAutenticado => UsuarioActual != null;
        public bool EsAdminNacional => UsuarioActual?.Rol == "AdminNacional";
        public bool EsAdminProvincial => UsuarioActual?.Rol == "AdminProvincial";
        public bool EsConsultor => UsuarioActual?.Rol == "Consultor";
        public int? TenantProvinciaId => UsuarioActual?.ProvinciaId;

        public string DescripcionAlcance => EsAdminNacional
            ? "Alcance: Nacional (Todas las provincias)"
            : $"Alcance: {UsuarioActual?.NombreProvincia} ({UsuarioActual?.Rol})";

        public void IniciarSesion(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);
            if (!usuario.EsAdminNacional && !usuario.ProvinciaId.HasValue)
                throw new InvalidOperationException("Un usuario provincial o consultor debe pertenecer a una provincia.");

            UsuarioActual = usuario;
        }

        public void CerrarSesion() => UsuarioActual = null;
    }
}
