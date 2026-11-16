using MSP.Entidades;

namespace MSP.Negocio
{
    public interface IEstrategiaAccesoTenant
    {
        int? ResolverFiltroProvincia(int? provinciaSeleccionadaEnUI);
        bool PuedeCrearOEditarEstablecimientos { get; }
        bool PuedeGestionarUsuarios { get; }
        bool PuedeGestionarCatalogosGlobales { get; }
        bool PuedeResolverAlertasSeguridad { get; }
    }

    public class EstrategiaNacional : IEstrategiaAccesoTenant
    {
        public int? ResolverFiltroProvincia(int? provinciaSeleccionadaEnUI) => provinciaSeleccionadaEnUI;
        public bool PuedeCrearOEditarEstablecimientos => true;
        public bool PuedeGestionarUsuarios => true;
        public bool PuedeGestionarCatalogosGlobales => true;
        public bool PuedeResolverAlertasSeguridad => true;
    }

    public class EstrategiaProvincial : IEstrategiaAccesoTenant
    {
        public int? ResolverFiltroProvincia(int? provinciaSeleccionadaEnUI) => TenantContext.Actual.TenantProvinciaId;
        public bool PuedeCrearOEditarEstablecimientos => true;
        public bool PuedeGestionarUsuarios => true;
        public bool PuedeGestionarCatalogosGlobales => false;
        public bool PuedeResolverAlertasSeguridad => false;
    }

    public class EstrategiaConsultor : IEstrategiaAccesoTenant
    {
        public int? ResolverFiltroProvincia(int? provinciaSeleccionadaEnUI) => TenantContext.Actual.TenantProvinciaId;
        public bool PuedeCrearOEditarEstablecimientos => false;
        public bool PuedeGestionarUsuarios => false;
        public bool PuedeGestionarCatalogosGlobales => false;
        public bool PuedeResolverAlertasSeguridad => false;
    }

    public static class FabricaEstrategiaRol
    {
        public static IEstrategiaAccesoTenant ObtenerEstrategiaActual()
        {
            if (TenantContext.Actual.EsAdminNacional) return new EstrategiaNacional();
            if (TenantContext.Actual.EsAdminProvincial) return new EstrategiaProvincial();
            return new EstrategiaConsultor();
        }
    }
}