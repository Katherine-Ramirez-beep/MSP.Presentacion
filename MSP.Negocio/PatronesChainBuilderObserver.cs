using MSP.Datos;
using MSP.Entidades;

namespace MSP.Negocio
{
    // ========================================================================
    // PATRÓN OBSERVER: Notifica acciones a la Bitácora y a la Interfaz (UI)
    // ========================================================================
    public interface IObservadorSistema
    {
        void AlOcurrirEvento(string modulo, string accion, string descripcion);
    }

    public sealed class GestorEventosSistema
    {
        private static readonly Lazy<GestorEventosSistema> _instancia = new(() => new GestorEventosSistema());
        public static GestorEventosSistema Instancia => _instancia.Value;

        private readonly List<IObservadorSistema> _observadores = new();

        private GestorEventosSistema()
        {
            Suscribir(new ObservadorBitacoraDB());
        }

        public void Suscribir(IObservadorSistema obs)
        {
            if (!_observadores.Contains(obs)) _observadores.Add(obs);
        }

        public void Desuscribir(IObservadorSistema obs) => _observadores.Remove(obs);

        public void Notificar(string modulo, string accion, string descripcion)
        {
            foreach (var obs in _observadores.ToList())
                obs.AlOcurrirEvento(modulo, accion, descripcion);
        }
    }

    public class ObservadorBitacoraDB : IObservadorSistema
    {
        private readonly AuditoriaRepositorio _repo = new();

        public void AlOcurrirEvento(string modulo, string accion, string descripcion)
        {
            var user = TenantContext.Actual.UsuarioActual;
            _repo.RegistrarBitacora(new BitacoraRegistro
            {
                UsuarioId = user?.UsuarioId,
                NombreUsuario = user?.NombreUsuario ?? "Sistema/Bootstrap",
                Rol = user?.Rol ?? "N/A",
                ProvinciaId = user?.ProvinciaId,
                Modulo = modulo,
                Accion = accion,
                Descripcion = descripcion
            });
        }
    }

    // ========================================================================
    // PATRÓN BUILDER: Construcción fluida de Establecimientos
    // ========================================================================
    public class EstablecimientoBuilder
    {
        private readonly Establecimiento _entidad = new();

        public EstablecimientoBuilder ConId(int id) { _entidad.EstablecimientoId = id; return this; }
        public EstablecimientoBuilder ConCodigo(string codigo) { _entidad.Codigo = codigo; return this; }
        public EstablecimientoBuilder ConNombre(string nombre) { _entidad.Nombre = nombre; return this; }
        public EstablecimientoBuilder EnProvincia(int provinciaId) { _entidad.ProvinciaId = provinciaId; return this; }
        public EstablecimientoBuilder DeTipo(int tipoId) { _entidad.TipoEstablecimientoId = tipoId; return this; }
        public EstablecimientoBuilder ConUbicacion(string direccion, string municipio)
        {
            _entidad.Direccion = direccion;
            _entidad.Municipio = municipio;
            return this;
        }
        public EstablecimientoBuilder ConTelefono(string? telefono) { _entidad.Telefono = telefono; return this; }
        public EstablecimientoBuilder ConEstado(bool activo) { _entidad.Activo = activo; return this; }
        public EstablecimientoBuilder ConAuditoriaUsuario(int usuarioId)
        {
            _entidad.UsuarioCreacionId = usuarioId;
            _entidad.UsuarioModificacionId = usuarioId;
            return this;
        }

        public Establecimiento Construir() => _entidad;
    }

    // ========================================================================
    // PATRÓN CHAIN OF RESPONSIBILITY: Validación de seguridad y negocio
    // ========================================================================
    public abstract class ValidadorEstablecimientoHandler
    {
        protected ValidadorEstablecimientoHandler? _siguiente;

        public ValidadorEstablecimientoHandler EstablecerSiguiente(ValidadorEstablecimientoHandler siguiente)
        {
            _siguiente = siguiente;
            return siguiente;
        }

        public virtual void Validar(Establecimiento est)
        {
            _siguiente?.Validar(est);
        }
    }

    public class ValidadorPermisosTenantHandler : ValidadorEstablecimientoHandler
    {
        private readonly AuditoriaRepositorio _auditoria = new();

        public override void Validar(Establecimiento est)
        {
            var estrategia = FabricaEstrategiaRol.ObtenerEstrategiaActual();
            if (!estrategia.PuedeCrearOEditarEstablecimientos)
                throw new UnauthorizedAccessException("Su rol de Consultor es de solo lectura y no permite crear ni modificar establecimientos.");

            if (TenantContext.Actual.EsAdminProvincial && est.ProvinciaId != TenantContext.Actual.TenantProvinciaId)
            {
                _auditoria.RegistrarAlertaSeguridad(new AlertaSeguridadTenant
                {
                    UsuarioOrigen = TenantContext.Actual.UsuarioActual?.NombreUsuario ?? "Desconocido",
                    ProvinciaInvolucradaId = est.ProvinciaId,
                    CategoriaAlerta = "VIOLACION_AISLAMIENTO",
                    NivelRiesgo = "Critico",
                    MensajeTecnico = $"Intento bloqueado de operar sobre ProvinciaId={est.ProvinciaId} desde TenantId={TenantContext.Actual.TenantProvinciaId}."
                });
                throw new UnauthorizedAccessException("Violación de aislamiento Multi-Tenant: No puede gestionar establecimientos fuera de su provincia.");
            }

            base.Validar(est);
        }
    }

    public class ValidadorCamposObligatoriosHandler : ValidadorEstablecimientoHandler
    {
        public override void Validar(Establecimiento est)
        {
            if (string.IsNullOrWhiteSpace(est.Codigo))
                throw new ArgumentException("El código de habilitación del establecimiento es obligatorio.");
            if (string.IsNullOrWhiteSpace(est.Nombre))
                throw new ArgumentException("El nombre del establecimiento es obligatorio.");
            if (est.ProvinciaId <= 0)
                throw new ArgumentException("Debe seleccionar una provincia válida.");
            if (est.TipoEstablecimientoId <= 0)
                throw new ArgumentException("Debe seleccionar un tipo de establecimiento válido.");
            if (string.IsNullOrWhiteSpace(est.Direccion) || string.IsNullOrWhiteSpace(est.Municipio))
                throw new ArgumentException("La dirección y el municipio son obligatorios.");

            base.Validar(est);
        }
    }

    public class ValidadorUnicidadCodigoHandler : ValidadorEstablecimientoHandler
    {
        private readonly EstablecimientoRepositorio _repo = new();

        public override void Validar(Establecimiento est)
        {
            int? excluirId = est.EstablecimientoId > 0 ? est.EstablecimientoId : null;
            if (est.Activo && _repo.ExisteCodigo(est.Codigo, excluirId))
                throw new InvalidOperationException($"Ya existe un establecimiento activo registrado con el código '{est.Codigo.ToUpper()}'.");

            base.Validar(est);
        }
    }
}

