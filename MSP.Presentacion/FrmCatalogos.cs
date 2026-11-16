using MSP.Entidades;
using MSP.Negocio;
namespace MSP.Presentacion;

public class FrmProvincias : FrmListado
{
    private readonly AdministracionService servicio = new();
    public FrmProvincias() : base("Provincias (Tenants)")
    {
        if (!TenantContext.Actual.EsAdminNacional) throw new UnauthorizedAccessException("Solo el Administrador Nacional puede gestionar provincias.");
        Col("Codigo", "Código"); Col("Nombre", "Provincia", 180); Col("RegionSalud", "Región de salud", 180); Col("Activo", "Activa");
        Boton("Actualizar", Cargar); Boton("Nueva provincia", () => { using var f = new FrmCrearCatalogo(true); if (f.ShowDialog(this) == DialogResult.OK) Cargar(); }, estilo: EstiloBoton.Primario);
        Boton("Activar / desactivar", () => { var p = Seleccion<Provincia>(); if (Confirmar($"¿{(p.Activo ? "Desactivar" : "Activar")} la provincia {p.Nombre}? Esto afecta el acceso de sus usuarios.")) { servicio.CambiarEstadoProvincia(p.ProvinciaId, !p.Activo); Cargar(); } });
        Shown += (_, _) => Ejecutar(Cargar);
    }
    private void Cargar() => Tabla.DataSource = servicio.ListarProvincias();
}
public class FrmTiposCentros : FrmListado
{
    private readonly AdministracionService servicio = new();
    public FrmTiposCentros() : base("Tipos de Centros")
    {
        if (!TenantContext.Actual.EsAdminNacional) throw new UnauthorizedAccessException("Solo el Administrador Nacional puede gestionar tipos.");
        Col("Codigo", "Código"); Col("Nombre", "Tipo de centro", 180); Col("Descripcion", "Descripción", 230); Col("Activo", "Activo");
        Boton("Actualizar", Cargar); Boton("Nuevo tipo", () => { using var f = new FrmCrearCatalogo(false); if (f.ShowDialog(this) == DialogResult.OK) Cargar(); }, estilo: EstiloBoton.Primario);
        Boton("Activar / desactivar", () => { var t = Seleccion<TipoEstablecimiento>(); if (Confirmar($"¿Cambiar el estado de {t.Nombre}?")) { servicio.CambiarEstadoTipo(t.TipoEstablecimientoId, !t.Activo); Cargar(); } });
        Shown += (_, _) => Ejecutar(Cargar);
    }
    private void Cargar() => Tabla.DataSource = servicio.ListarTipos();
}
public class FrmCrearCatalogo : FrmEdicion
{
    public FrmCrearCatalogo(bool provincia) : base(provincia ? "Nueva provincia" : "Nuevo tipo de centro")
    {
        var codigo = Texto("Código"); var nombre = Texto("Nombre"); var detalle = Texto(provincia ? "Región de salud" : "Descripción");
        AlGuardar(() => { var s = new AdministracionService(); if (provincia) s.CrearProvincia(codigo.Text, nombre.Text, detalle.Text); else s.CrearTipo(codigo.Text, nombre.Text, detalle.Text); });
    }
}
