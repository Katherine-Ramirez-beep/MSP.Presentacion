using System.Text;
using MSP.Datos;
using MSP.Entidades;

namespace MSP.Negocio
{
    public class AuthService
    {
        private readonly UsuarioRepositorio _usuarioRepo = new();
        private readonly AuditoriaRepositorio _auditoriaRepo = new();

        public bool SistemaRequiereConfiguracionInicial() => !_usuarioRepo.ExisteAlgunUsuario();

        public void RegistrarPrimerAdminNacional(string nombreCompleto, string nombreUsuario, string password)
        {
            if (_usuarioRepo.ExisteAlgunUsuario())
                throw new InvalidOperationException("El sistema ya cuenta con usuarios configurados.");

            if (string.IsNullOrWhiteSpace(nombreCompleto) || string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("El nombre completo y el nombre de usuario son obligatorios.");
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("La contraseña debe tener mínimo 6 caracteres.");

            ValidacionTexto.Longitud(nombreCompleto, 150, "Nombre completo");
            ValidacionTexto.Longitud(nombreUsuario, 50, "Usuario");
            var (hash, salt) = SeguridadCrypto.CrearHash(password);

            _usuarioRepo.Insertar(new Usuario
            {
                ProvinciaId = null,
                NombreCompleto = nombreCompleto,
                NombreUsuario = nombreUsuario,
                PasswordHash = hash,
                PasswordSalt = salt,
                Rol = "AdminNacional",
                Activo = true
            });

            GestorEventosSistema.Instancia.Notificar("Seguridad", "BOOTSTRAP_ADMIN", $"Se creó el Administrador Nacional inicial '{nombreUsuario}'.");
        }

        public Usuario IniciarSesion(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Debe ingresar usuario y contraseña.");

            var usuario = _usuarioRepo.ObtenerPorNombreUsuario(nombreUsuario);
            if (usuario == null)
            {
                _auditoriaRepo.RegistrarAlertaSeguridad(new AlertaSeguridadTenant
                {
                    UsuarioOrigen = nombreUsuario,
                    CategoriaAlerta = "USUARIO_INEXISTENTE",
                    NivelRiesgo = "Advertencia",
                    MensajeTecnico = $"Intento de acceso con usuario no registrado: '{nombreUsuario}'."
                });
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
            }

            if (usuario.Bloqueado)
                throw new UnauthorizedAccessException("Esta cuenta está bloqueada por múltiples intentos fallidos. Contacte al Administrador.");

            if (!usuario.Activo)
                throw new UnauthorizedAccessException("Esta cuenta o su provincia asignada se encuentran desactivadas.");

            if (!SeguridadCrypto.VerificarPassword(password, usuario.PasswordHash, usuario.PasswordSalt))
            {
                int nuevosIntentos = usuario.IntentosFallidos + 1;
                bool bloquear = nuevosIntentos >= 3;
                _usuarioRepo.RegistrarIntentoFallido(usuario.UsuarioId, nuevosIntentos, bloquear);

                _auditoriaRepo.RegistrarAlertaSeguridad(new AlertaSeguridadTenant
                {
                    UsuarioOrigen = usuario.NombreUsuario,
                    ProvinciaInvolucradaId = usuario.ProvinciaId,
                    CategoriaAlerta = bloquear ? "BLOQUEO_CREDENCIALES" : "INTENTO_FALLIDO",
                    NivelRiesgo = bloquear ? "Critico" : "Advertencia",
                    MensajeTecnico = bloquear
                        ? $"Cuenta '{usuario.NombreUsuario}' bloqueada tras 3 intentos fallidos."
                        : $"Contraseña incorrecta para '{usuario.NombreUsuario}' (Intento {nuevosIntentos}/3)."
                });

                throw new UnauthorizedAccessException(bloquear
                    ? "Cuenta bloqueada por alcanzar 3 intentos fallidos."
                    : $"Contraseña incorrecta. Intento {nuevosIntentos} de 3.");
            }

            _usuarioRepo.RegistrarLoginExitoso(usuario.UsuarioId);
            TenantContext.Actual.IniciarSesion(usuario);
            try { GestorEventosSistema.Instancia.Notificar("Autenticación", "INICIO_SESION", $"Inicio de sesión exitoso ({TenantContext.Actual.DescripcionAlcance})."); } catch { TenantContext.Actual.CerrarSesion(); throw; }

            return usuario;
        }

        public void CambiarPasswordPropia(string passwordActual, string passwordNueva)
        {
            var u = TenantContext.Actual.UsuarioActual ?? throw new InvalidOperationException("No hay sesión activa.");
            var uDb = _usuarioRepo.ObtenerPorNombreUsuario(u.NombreUsuario)!;

            if (!SeguridadCrypto.VerificarPassword(passwordActual, uDb.PasswordHash, uDb.PasswordSalt))
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");

            if (passwordNueva.Length < 6)
                throw new ArgumentException("La nueva contraseña debe tener al menos 6 caracteres.");

            var (hash, salt) = SeguridadCrypto.CrearHash(passwordNueva);
            _usuarioRepo.ActualizarPassword(u.UsuarioId, hash, salt);
            GestorEventosSistema.Instancia.Notificar("Seguridad", "CAMBIO_CLAVE", "El usuario actualizó su contraseña.");
        }

        public void CerrarSesion()
        {
            if (TenantContext.Actual.EstaAutenticado)
            {
                try { GestorEventosSistema.Instancia.Notificar("Autenticación", "CIERRE_SESION", "Cierre de sesión del usuario."); } finally { TenantContext.Actual.CerrarSesion(); }
            }
        }
    }

    public partial class EstablecimientoService
    {
        private readonly EstablecimientoRepositorio _repo = new();

        public List<EstablecimientoDto> Listar(int? provinciaFiltroUI, int? tipoId, bool? activo, string? texto)
        {
            var estrategia = FabricaEstrategiaRol.ObtenerEstrategiaActual();
            int? provinciaReal = estrategia.ResolverFiltroProvincia(provinciaFiltroUI);
            return _repo.Listar(provinciaReal, tipoId, activo, texto);
        }

        public void Guardar(Establecimiento est)
        {
            ValidarDestinoExistente(est);
            var v1 = new ValidadorPermisosTenantHandler();
            var v2 = new ValidadorCamposObligatoriosHandler();
            var v3 = new ValidadorUnicidadCodigoHandler();
            v1.EstablecerSiguiente(v2).EstablecerSiguiente(v3);
            v1.Validar(est);

            if (est.EstablecimientoId == 0)
            {
                _repo.Insertar(est);
                GestorEventosSistema.Instancia.Notificar("Establecimientos", "CREAR", $"Se registró '{est.Nombre}' ({est.Codigo}).");
            }
            else
            {
                _repo.Actualizar(est);
                GestorEventosSistema.Instancia.Notificar("Establecimientos", "EDITAR", $"Se actualizó '{est.Nombre}' ({est.Codigo}).");
            }
        }

        public List<ConsolidadoProvinciaDto> ObtenerConsolidado()
        {
            var estrategia = FabricaEstrategiaRol.ObtenerEstrategiaActual();
            int? provinciaReal = estrategia.ResolverFiltroProvincia(null);
            return _repo.ObtenerConsolidado(provinciaReal);
        }
    }

    public partial class AdministracionService
    {
        private readonly UsuarioRepositorio _usuarioRepo = new();
        private readonly CatalogoRepositorio _catalogoRepo = new();
        private readonly AuditoriaRepositorio _auditoriaRepo = new();

        public List<Usuario> ListarUsuarios()
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarUsuarios) throw new UnauthorizedAccessException("Sin permisos para consultar usuarios.");
            int? provFiltro = FabricaEstrategiaRol.ObtenerEstrategiaActual().ResolverFiltroProvincia(null);
            return _usuarioRepo.Listar(provFiltro);
        }

        public void CrearUsuario(string nombreCompleto, string nombreUsuario, string password, string rol, int? provinciaId)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarUsuarios)
                throw new UnauthorizedAccessException("No tiene permisos para crear usuarios.");

            ValidarRolProvincia(rol, provinciaId);
            if (TenantContext.Actual.EsAdminProvincial)
            {
                if (rol == "AdminNacional")
                    throw new UnauthorizedAccessException("Un Administrador Provincial no puede crear Administradores Nacionales.");
                provinciaId = TenantContext.Actual.TenantProvinciaId;
            }

            if (rol == "AdminNacional") provinciaId = null;
            else if (!provinciaId.HasValue)
                throw new ArgumentException("Debe asignar una provincia para los roles Provincial o Consultor.");

            if (string.IsNullOrWhiteSpace(nombreCompleto) || string.IsNullOrWhiteSpace(nombreUsuario) || password.Length < 6)
                throw new ArgumentException("Complete todos los campos y use una contraseña de al menos 6 caracteres.");

            ValidacionTexto.Longitud(nombreCompleto, 150, "Nombre completo");
            ValidacionTexto.Longitud(nombreUsuario, 50, "Usuario");
            var (hash, salt) = SeguridadCrypto.CrearHash(password);
            _usuarioRepo.Insertar(new Usuario
            {
                ProvinciaId = provinciaId,
                NombreCompleto = nombreCompleto,
                NombreUsuario = nombreUsuario,
                PasswordHash = hash,
                PasswordSalt = salt,
                Rol = rol
            });

            GestorEventosSistema.Instancia.Notificar("Usuarios", "CREAR", $"Se creó el usuario '{nombreUsuario}' con rol '{rol}'.");
        }

        public void ActualizarEstadoUsuario(int usuarioId, bool activo, bool desbloquear)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarUsuarios)
                throw new UnauthorizedAccessException("Sin permisos para gestionar usuarios.");

            ValidarUsuarioObjetivo(usuarioId);
            if (usuarioId == TenantContext.Actual.UsuarioActual!.UsuarioId && !activo) throw new InvalidOperationException("No puede desactivar su propia cuenta.");
            _usuarioRepo.CambiarEstadoBloqueoOActivo(usuarioId, activo, desbloquear);
            GestorEventosSistema.Instancia.Notificar("Usuarios", "ESTADO", $"Se actualizó estado/bloqueo del usuario ID={usuarioId}.");
        }

        public List<Provincia> ListarProvincias(bool soloActivas = false) => _catalogoRepo.ListarProvincias(soloActivas);

        public void CrearProvincia(string codigo, string nombre, string regionSalud)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarCatalogosGlobales)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede registrar provincias.");

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(regionSalud))
                throw new ArgumentException("Todos los campos de la provincia son obligatorios.");

            ValidacionTexto.Longitud(codigo, 10, "Código"); ValidacionTexto.Longitud(nombre, 100, "Provincia"); ValidacionTexto.Longitud(regionSalud, 80, "Región de salud");
            _catalogoRepo.InsertarProvincia(new Provincia { Codigo = codigo, Nombre = nombre, RegionSalud = regionSalud });
            GestorEventosSistema.Instancia.Notificar("Provincias", "CREAR", $"Se registró la provincia '{nombre}' ({codigo}).");
        }

        public void CambiarEstadoProvincia(int provinciaId, bool activo)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarCatalogosGlobales)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede activar o desactivar provincias.");

            _catalogoRepo.CambiarEstadoProvincia(provinciaId, activo);
            GestorEventosSistema.Instancia.Notificar("Provincias", activo ? "ACTIVAR" : "DESACTIVAR", $"Cambio de estado en Provincia ID={provinciaId}.");
        }

        public List<TipoEstablecimiento> ListarTipos(bool soloActivos = false) => _catalogoRepo.ListarTipos(soloActivos);

        public void CrearTipo(string codigo, string nombre, string descripcion)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarCatalogosGlobales)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede crear tipos de establecimientos.");

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Código y Nombre son obligatorios.");

            ValidacionTexto.Longitud(codigo, 15, "Código"); ValidacionTexto.Longitud(nombre, 100, "Tipo"); ValidacionTexto.Longitud(descripcion, 250, "Descripción");
            _catalogoRepo.InsertarTipo(new TipoEstablecimiento { Codigo = codigo, Nombre = nombre, Descripcion = descripcion });
            GestorEventosSistema.Instancia.Notificar("TiposEstablecimientos", "CREAR", $"Se creó el tipo '{nombre}'.");
        }

        public void CambiarEstadoTipo(int tipoId, bool activo)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarCatalogosGlobales)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede modificar tipos.");

            _catalogoRepo.CambiarEstadoTipo(tipoId, activo);
            GestorEventosSistema.Instancia.Notificar("TiposEstablecimientos", "ESTADO", $"Cambio de estado en Tipo ID={tipoId}.");
        }

        public List<BitacoraRegistro> ListarBitacora()
        {
            int? provFiltro = FabricaEstrategiaRol.ObtenerEstrategiaActual().ResolverFiltroProvincia(null);
            return _auditoriaRepo.ListarBitacora(provFiltro);
        }

        public List<AlertaSeguridadTenant> ListarAlertas()
        {
            int? provFiltro = FabricaEstrategiaRol.ObtenerEstrategiaActual().ResolverFiltroProvincia(null);
            return _auditoriaRepo.ListarAlertas(provFiltro);
        }

        public void ResolverAlerta(long alertaId)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeResolverAlertasSeguridad)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede marcar alertas como resueltas.");

            _auditoriaRepo.MarcarAlertaResuelta(alertaId);
            GestorEventosSistema.Instancia.Notificar("SeguridadTenant", "RESOLVER_ALERTA", $"Alerta de seguridad ID={alertaId} marcada como resuelta.");
        }
    }

}


