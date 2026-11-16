using MSP.Entidades;
using MSP.Negocio;
namespace MSP.Presentacion;

public class FrmConsolidado : FrmListado
{
    public FrmConsolidado() : base("Consolidado de establecimientos")
    {
        Col("CodigoProvincia", "Código"); Col("Provincia", "Provincia", 160); Col("RegionSalud", "Región de salud", 150); Col("TotalEstablecimientos", "Total"); Col("Activos", "Activos"); Col("Inactivos", "Inactivos");
        Boton("Actualizar", Cargar); Shown += (_, _) => Ejecutar(Cargar);
    }
    private void Cargar()
    { var datos = new EstablecimientoService().ObtenerConsolidado(); Tabla.DataSource = datos; Estado.Text = $"Total de centros: {datos.Sum(x => x.TotalEstablecimientos)}   |   Activos: {datos.Sum(x => x.Activos)}   |   Inactivos: {datos.Sum(x => x.Inactivos)}"; }
}
public class FrmBitacora : FrmListado
{
    private List<BitacoraRegistro> registros = new();
    public FrmBitacora() : base("Bitácora de auditoría")
    {
        Col("BitacoraId", "ID", 45); Col("FechaHora", "Fecha y hora", 130); Col("NombreUsuario", "Usuario"); Col("Rol", "Rol"); Col("ProvinciaId", "Provincia ID", 60); Col("Modulo", "Módulo"); Col("Accion", "Acción"); Col("Descripcion", "Descripción", 270);
        Boton("Actualizar", Cargar);
        var formatos = new ContextMenuStrip { Font = Tema.Fuente, ShowImageMargin = false, Renderer = new RenderizadorMsp() };
        foreach (var formato in new[] { "Excel", "Word", "PDF", "TXT" }) { string f = formato; formatos.Items.Add(new ToolStripMenuItem(f, null, (_, _) => Ejecutar(() => Exportar(f))) { Padding = new Padding(Tema.Px(6)) }); }
        Button descargar = null!;
        descargar = Boton("Descargar…", () => formatos.Show(descargar, new Point(0, descargar.Height)), estilo: EstiloBoton.Primario);
        Shown += (_, _) => Ejecutar(Cargar);
    }
    private void Cargar() { registros = new AdministracionService().ListarBitacora(); Tabla.DataSource = registros; }
    private void Exportar(string formato)
    {
        string extension = formato switch { "Excel" => "xlsx", "Word" => "docx", "PDF" => "pdf", _ => "txt" };
        using var dialogo = new SaveFileDialog { Filter = $"{formato} (*.{extension})|*.{extension}", DefaultExt = extension, AddExtension = true, FileName = $"Bitacora_MSP_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}" };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        ExportadorBitacoraService.Exportar(registros, dialogo.FileName, formato); MessageBox.Show(this, "Archivo exportado correctamente.", "Bitácora", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
public class FrmMonitorSeguridadTenant : FrmListado
{
    private readonly AdministracionService servicio = new();
    public FrmMonitorSeguridadTenant() : base("Monitor Seguridad Tenant")
    {
        Col("FechaHora", "Fecha y hora", 130); Col("UsuarioOrigen", "Usuario"); Col("NombreProvincia", "Provincia"); Col("CategoriaAlerta", "Categoría", 140); Col("NivelRiesgo", "Riesgo", 70); Col("MensajeTecnico", "Detalle", 250); Col("Resuelta", "Resuelta", 65);
        Boton("Actualizar", Cargar); Boton("Marcar resuelta", () => { var a = Seleccion<AlertaSeguridadTenant>(); if (a.Resuelta) return; if (Confirmar("¿Marcar la alerta seleccionada como resuelta?")) { servicio.ResolverAlerta(a.AlertaId); Cargar(); } }, FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeResolverAlertasSeguridad, EstiloBoton.Primario);
        Shown += (_, _) => Ejecutar(Cargar);
    }
    private void Cargar() { var datos = servicio.ListarAlertas(); Tabla.DataSource = datos; Estado.Text = $"Alertas: {datos.Count}   |   Pendientes: {datos.Count(x => !x.Resuelta)}   |   Críticas pendientes: {datos.Count(x => !x.Resuelta && x.NivelRiesgo == "Critico")}"; }
}


