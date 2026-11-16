using MSP.Entidades;
using MSP.Negocio;
namespace MSP.Presentacion;

public class FrmUsuarios : FrmListado
{
    private readonly AdministracionService servicio = new();
    public FrmUsuarios() : base("Usuarios")
    {
        if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarUsuarios) throw new UnauthorizedAccessException("No tiene permisos para gestionar usuarios.");
        Col("NombreUsuario", "Usuario"); Col("NombreCompleto", "Nombre completo", 160); Col("Rol", "Rol"); Col("NombreProvincia", "Provincia"); Col("Activo", "Activo", 60); Col("Bloqueado", "Bloqueado", 70); Col("IntentosFallidos", "Intentos", 60); Col("UltimoAcceso", "Último acceso", 130);
        Boton("Actualizar", Cargar); Boton("Crear usuario", () => Editar(null), estilo: EstiloBoton.Primario); Boton("Editar / asignar provincia", () => Editar(Seleccion<Usuario>()));
        Boton("Activar / desactivar", () => { var u = Seleccion<Usuario>(); if (Confirmar($"¿{(u.Activo ? "Desactivar" : "Activar")} a {u.NombreUsuario}?")) { servicio.ActualizarEstadoUsuario(u.UsuarioId, !u.Activo, false); Cargar(); } });
        Boton("Desbloquear", () => { var u = Seleccion<Usuario>(); if (Confirmar($"¿Desbloquear a {u.NombreUsuario}?")) { servicio.ActualizarEstadoUsuario(u.UsuarioId, u.Activo, true); Cargar(); } });
        Shown += (_, _) => Ejecutar(Cargar);
    }
    private void Cargar() => Tabla.DataSource = servicio.ListarUsuarios();
    private void Editar(Usuario? u) { using var f = new FrmEditarUsuario(u); if (f.ShowDialog(this) == DialogResult.OK) Cargar(); }
}
public class FrmEditarUsuario : FrmEdicion
{
    public FrmEditarUsuario(Usuario? u = null) : base(u == null ? "Crear usuario" : "Editar usuario")
    {
        var nombre = Texto("Nombre completo", u?.NombreCompleto ?? ""); var usuario = Texto("Usuario", u?.NombreUsuario ?? ""); usuario.ReadOnly = u != null;
        TextBox? password = u == null ? Texto("Contraseña (mín. 6)", clave: true) : null;
        var rol = Ui.Combo(); rol.Items.AddRange(TenantContext.Actual.EsAdminNacional ? new object[] { "AdminNacional", "AdminProvincial", "Consultor" } : new object[] { "AdminProvincial", "Consultor" }); rol.SelectedItem = u?.Rol ?? "Consultor"; Campo("Rol", rol);
        var provincia = Ui.Combo(); Ui.Provincias(provincia); Campo("Provincia", provincia); if (u?.ProvinciaId != null) provincia.SelectedValue = u.ProvinciaId.Value;
        void ActualizarProvincia() => provincia.Enabled = TenantContext.Actual.EsAdminNacional && (string?)rol.SelectedItem != "AdminNacional";
        rol.SelectedIndexChanged += (_, _) => ActualizarProvincia(); ActualizarProvincia();
        AlGuardar(() => { var s = new AdministracionService(); var r = (string?)rol.SelectedItem ?? ""; int? p = r == "AdminNacional" ? null : Ui.Id(provincia); if (u == null) s.CrearUsuario(nombre.Text.Trim(), usuario.Text.Trim(), password!.Text, r, p); else s.EditarUsuario(u.UsuarioId, nombre.Text.Trim(), r, p); });
    }
}
