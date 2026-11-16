using MSP.Entidades;
using MSP.Datos;
namespace MSP.Negocio;

public partial class AdministracionService
{
    private void ValidarRolProvincia(string rol, int? provinciaId)
    {
        if (rol is not ("AdminNacional" or "AdminProvincial" or "Consultor")) throw new ArgumentException("Seleccione un rol válido.");
        if (TenantContext.Actual.EsAdminProvincial && (rol == "AdminNacional" || provinciaId != TenantContext.Actual.TenantProvinciaId))
        {
            _auditoriaRepo.RegistrarAlertaSeguridad(new AlertaSeguridadTenant { UsuarioOrigen = TenantContext.Actual.UsuarioActual!.NombreUsuario, ProvinciaInvolucradaId = TenantContext.Actual.TenantProvinciaId, CategoriaAlerta = "VIOLACION_AISLAMIENTO", NivelRiesgo = "Critico", MensajeTecnico = "Intento de asignar un usuario fuera del alcance permitido." });
            throw new UnauthorizedAccessException("Solo puede asignar roles provinciales dentro de su provincia.");
        }
        if (rol != "AdminNacional" && !_catalogoRepo.ListarProvincias(true).Any(p => p.ProvinciaId == provinciaId)) throw new ArgumentException("Seleccione una provincia activa.");
    }
    private Usuario ValidarUsuarioObjetivo(int id)
    {
        if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarUsuarios) throw new UnauthorizedAccessException("Sin permisos para gestionar usuarios.");
        var u = ListarUsuarios().FirstOrDefault(x => x.UsuarioId == id);
        if (u == null)
        {
            _auditoriaRepo.RegistrarAlertaSeguridad(new AlertaSeguridadTenant { UsuarioOrigen = TenantContext.Actual.UsuarioActual!.NombreUsuario, ProvinciaInvolucradaId = TenantContext.Actual.TenantProvinciaId, CategoriaAlerta = "VIOLACION_AISLAMIENTO", NivelRiesgo = "Critico", MensajeTecnico = $"Intento de modificar usuario ID={id} inexistente o fuera del alcance permitido." });
            throw new UnauthorizedAccessException("Usuario no disponible dentro de su alcance.");
        }
        return u;
    }
    public void EditarUsuario(int usuarioId, string nombreCompleto, string rol, int? provinciaId)
    {
        var u = ValidarUsuarioObjetivo(usuarioId); ValidarRolProvincia(rol, provinciaId);
        if (string.IsNullOrWhiteSpace(nombreCompleto)) throw new ArgumentException("El nombre es obligatorio.");
        if (usuarioId == TenantContext.Actual.UsuarioActual!.UsuarioId && (rol != u.Rol || provinciaId != u.ProvinciaId)) throw new InvalidOperationException("No puede modificar su propio rol o provincia durante la sesión.");
        ValidacionTexto.Longitud(nombreCompleto, 150, "Nombre completo");
        _usuarioRepo.ActualizarPerfil(usuarioId, nombreCompleto, rol, rol == "AdminNacional" ? null : provinciaId);
        GestorEventosSistema.Instancia.Notificar("Usuarios", "EDITAR", $"Se actualizó el perfil del usuario ID={usuarioId}.");
    }
}
public partial class EstablecimientoService
{
    private void ValidarDestinoExistente(Establecimiento e)
    {
        ValidacionTexto.Longitud(e.Codigo, 25, "Código"); ValidacionTexto.Longitud(e.Nombre, 150, "Nombre"); ValidacionTexto.Longitud(e.Direccion, 250, "Dirección"); ValidacionTexto.Longitud(e.Municipio, 100, "Municipio"); ValidacionTexto.Longitud(e.Telefono, 25, "Teléfono");
        var permisos = FabricaEstrategiaRol.ObtenerEstrategiaActual();
        if (!permisos.PuedeCrearOEditarEstablecimientos) throw new UnauthorizedAccessException("Su rol es de solo lectura.");
        EstablecimientoDto? anterior = null;
        if (e.EstablecimientoId != 0)
        {
            anterior = _repo.Listar(permisos.ResolverFiltroProvincia(null), null, null, null).FirstOrDefault(x => x.EstablecimientoId == e.EstablecimientoId);
            if (anterior == null)
            {
                new AuditoriaRepositorio().RegistrarAlertaSeguridad(new AlertaSeguridadTenant { UsuarioOrigen = TenantContext.Actual.UsuarioActual!.NombreUsuario, ProvinciaInvolucradaId = TenantContext.Actual.TenantProvinciaId, CategoriaAlerta = "VIOLACION_AISLAMIENTO", NivelRiesgo = "Critico", MensajeTecnico = $"Intento de editar establecimiento ID={e.EstablecimientoId} inexistente o ajeno." });
                throw new UnauthorizedAccessException("El establecimiento no existe dentro de su alcance.");
            }
        }
        var catalogo = new CatalogoRepositorio();
        if (!catalogo.ListarProvincias(true).Any(p => p.ProvinciaId == e.ProvinciaId) && anterior?.ProvinciaId != e.ProvinciaId) throw new ArgumentException("Seleccione una provincia activa.");
        if (!catalogo.ListarTipos(true).Any(t => t.TipoEstablecimientoId == e.TipoEstablecimientoId) && anterior?.TipoEstablecimientoId != e.TipoEstablecimientoId) throw new ArgumentException("Seleccione un tipo activo.");
    }
}

