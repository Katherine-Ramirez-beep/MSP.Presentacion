using MSP.Entidades;
using MSP.Negocio;

namespace MSP.Presentacion;

public class FrmEstablecimientos : FrmListado
{
    private readonly EstablecimientoService servicio = new();
    private readonly ComboBox provincia = Ui.Combo(), tipo = Ui.Combo(), estado = Ui.Combo();
    private readonly TextBox buscar = new() { Width = 200, MaxLength = 100, PlaceholderText = "Código, nombre o municipio" };
    public FrmEstablecimientos() : base("Establecimientos")
    {
        Col("Codigo", "Código", 65); Col("Nombre", "Nombre", 170); Col("Provincia", "Provincia"); Col("TipoEstablecimiento", "Tipo"); Col("Municipio", "Municipio"); Col("Direccion", "Dirección", 150); Col("Telefono", "Teléfono"); Col("Estado", "Estado", 65);
        estado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" }); estado.SelectedIndex = 0;
        Acciones.Controls.AddRange(new Control[] { Ui.Caja(provincia, 230), Ui.Caja(tipo, 230), Ui.Caja(estado, 140), Ui.Caja(buscar, 270) });
        Boton("Buscar / actualizar", Cargar);
        bool editar = FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeCrearOEditarEstablecimientos;
        Boton("Nuevo", () => Editar(null), editar, EstiloBoton.Primario); Boton("Editar seleccionado", () => Editar(Seleccion<EstablecimientoDto>()), editar);
        Shown += (_, _) => Ejecutar(() => { Ui.Provincias(provincia, true); var tipos = new AdministracionService().ListarTipos(); tipos.Insert(0, new TipoEstablecimiento { Nombre = "Todos los tipos" }); tipo.DisplayMember = "Nombre"; tipo.ValueMember = "TipoEstablecimientoId"; tipo.DataSource = tipos; Cargar(); });
    }
    private void Cargar() => Tabla.DataSource = servicio.Listar(Ui.Id(provincia) == 0 ? null : Ui.Id(provincia), Ui.Id(tipo) == 0 ? null : Ui.Id(tipo), estado.SelectedIndex == 0 ? null : estado.SelectedIndex == 1, buscar.Text);
    private void Editar(EstablecimientoDto? registro) { using var f = new FrmEditarEstablecimiento(registro); if (f.ShowDialog(this) == DialogResult.OK) Cargar(); }
}

public class FrmEditarEstablecimiento : FrmEdicion
{
    public FrmEditarEstablecimiento(EstablecimientoDto? e = null) : base(e == null ? "Nuevo establecimiento" : "Editar establecimiento")
    {
        if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeCrearOEditarEstablecimientos) throw new UnauthorizedAccessException("Su rol es de solo lectura.");
        var codigo = Texto("Código", e?.Codigo ?? ""); var nombre = Texto("Nombre", e?.Nombre ?? "");
        var provincia = Ui.Combo(); Ui.Provincias(provincia); Campo("Provincia", provincia);
        var tipo = Ui.Combo(); tipo.DisplayMember = "Nombre"; tipo.ValueMember = "TipoEstablecimientoId"; tipo.DataSource = new AdministracionService().ListarTipos(); Campo("Tipo de centro", tipo);
        if (e != null) { provincia.SelectedValue = e.ProvinciaId; tipo.SelectedValue = e.TipoEstablecimientoId; }
        var direccion = Texto("Dirección", e?.Direccion ?? ""); var municipio = Texto("Municipio", e?.Municipio ?? ""); var telefono = Texto("Teléfono", e?.Telefono ?? "");
        var activo = new CheckBox { Text = "Activo", Checked = e?.Activo ?? true, Enabled = e != null }; Campo("Estado", activo);
        AlGuardar(() => new EstablecimientoService().Guardar(new EstablecimientoBuilder().ConId(e?.EstablecimientoId ?? 0).ConCodigo(codigo.Text.Trim().ToUpperInvariant()).ConNombre(nombre.Text.Trim()).EnProvincia(Ui.Id(provincia)).DeTipo(Ui.Id(tipo)).ConUbicacion(direccion.Text.Trim(), municipio.Text.Trim()).ConTelefono(telefono.Text.Trim()).ConEstado(activo.Checked).ConAuditoriaUsuario(TenantContext.Actual.UsuarioActual!.UsuarioId).Construir()));
    }
}

