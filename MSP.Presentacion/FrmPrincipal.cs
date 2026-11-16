using MSP.Entidades;
using MSP.Negocio;

namespace MSP.Presentacion;

public class FrmPrincipal : Form
{
    private readonly MenuStrip menu;
    private ToolStripMenuItem? seccionActiva;

    public FrmPrincipal()
    {
        if (!TenantContext.Actual.EstaAutenticado) throw new UnauthorizedAccessException("Inicie sesión.");
        var usuario = TenantContext.Actual.UsuarioActual!;
        Text = "MSP · Plataforma Nacional Multi-Tenant"; IsMdiContainer = true; WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(Tema.Px(1100), Tema.Px(700)); Font = Tema.Fuente; BackColor = Tema.Fondo;
        if (Controls.OfType<MdiClient>().FirstOrDefault() is { } cliente) cliente.BackColor = Tema.Fondo;

        // Banner institucional (misma identidad que el login) con los datos de la sesión a la derecha.
        var margen = Tema.Px(28);
        var banner = new Panel { Dock = DockStyle.Top, Height = Tema.Px(72), BackColor = Tema.Primario };
        var izquierda = new Panel { Dock = DockStyle.Fill };
        izquierda.Controls.Add(new Label { Dock = DockStyle.Fill, Text = "Plataforma Nacional Multi-Tenant de Establecimientos", Font = Tema.Fuente, ForeColor = Tema.Menta, TextAlign = ContentAlignment.TopLeft, Padding = new Padding(margen, 0, 0, 0) });
        izquierda.Controls.Add(new Label { Dock = DockStyle.Top, Height = Tema.Px(42), Text = "MINISTERIO DE SALUD PÚBLICA", Font = Tema.FuenteMarca, ForeColor = Color.White, TextAlign = ContentAlignment.BottomLeft, Padding = new Padding(margen, 0, 0, 0) });
        var derecha = new Panel { Dock = DockStyle.Right, Width = Tema.Px(380) };
        derecha.Controls.Add(new Label { Dock = DockStyle.Fill, Text = TenantContext.Actual.DescripcionAlcance, Font = Tema.FuenteEtiqueta, ForeColor = Tema.Menta, TextAlign = ContentAlignment.TopRight, Padding = new Padding(0, 0, margen, 0), AutoEllipsis = true });
        derecha.Controls.Add(new Label { Dock = DockStyle.Top, Height = Tema.Px(42), Text = usuario.NombreCompleto, Font = Tema.FuenteUsuario, ForeColor = Color.White, TextAlign = ContentAlignment.BottomRight, Padding = new Padding(0, 0, margen, 0), AutoEllipsis = true });
        banner.Controls.Add(izquierda); banner.Controls.Add(derecha);

        menu = new MenuStrip { Dock = DockStyle.Top, GripStyle = ToolStripGripStyle.Hidden, BackColor = Tema.PrimarioHover, Font = Tema.FuenteMenu, Padding = new Padding(Tema.Px(18), 0, Tema.Px(18), 0) };
        menu.Renderer = new RenderizadorMsp(item => ReferenceEquals(item, seccionActiva));
        var permisos = FabricaEstrategiaRol.ObtenerEstrategiaActual();
        var relleno = new Padding(Tema.Px(14), Tema.Px(11), Tema.Px(14), Tema.Px(11));

        ToolStripMenuItem Item(string titulo, Func<Form> crear, bool visible = true)
        {
            var item = new ToolStripMenuItem(titulo) { Visible = visible, Padding = relleno };
            item.Click += (_, _) => Ui.Ejecutar(this, () => Abrir(crear, item));
            menu.Items.Add(item); return item;
        }
        var inicio = Item("Establecimientos", () => new FrmEstablecimientos());
        Item("Usuarios", () => new FrmUsuarios(), permisos.PuedeGestionarUsuarios);
        Item("Provincias", () => new FrmProvincias(), permisos.PuedeGestionarCatalogosGlobales);
        Item("Tipos de Centros", () => new FrmTiposCentros(), permisos.PuedeGestionarCatalogosGlobales);
        Item("Consolidado", () => new FrmConsolidado());
        Item("Bitácora", () => new FrmBitacora());
        Item("Monitor Seguridad Tenant", () => new FrmMonitorSeguridadTenant());

        var sesion = new ToolStripMenuItem("Mi Sesión") { Alignment = ToolStripItemAlignment.Right, Padding = relleno };
        sesion.DropDown.Font = Tema.Fuente;
        if (sesion.DropDown is ToolStripDropDownMenu desplegable) desplegable.ShowImageMargin = false;
        var opcion = new Padding(Tema.Px(6), Tema.Px(6), Tema.Px(6), Tema.Px(6));
        sesion.DropDownItems.Add(new ToolStripMenuItem("Cambiar Contraseña", null, (_, _) => { using var f = new FrmCambiarPassword(); f.ShowDialog(this); }) { Padding = opcion });
        sesion.DropDownItems.Add(new ToolStripSeparator());
        sesion.DropDownItems.Add(new ToolStripMenuItem("Cerrar Sesión", null, (_, _) => Close()) { Padding = opcion });
        menu.Items.Add(sesion);
        MainMenuStrip = menu;

        var estado = new StatusStrip { BackColor = Color.White, SizingGrip = false, Font = Tema.FuenteEtiqueta, Renderer = new RenderizadorMsp() };
        estado.Items.Add(new ToolStripStatusLabel($"Usuario: {usuario.NombreCompleto}") { Spring = true, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Tema.TextoSuave });
        estado.Items.Add(new ToolStripStatusLabel($"Rol: {usuario.Rol}") { ForeColor = Tema.Primario });

        Controls.Add(new Panel { Dock = DockStyle.Top, Height = Tema.Px(3), BackColor = Tema.Menta });
        Controls.Add(menu); Controls.Add(banner); Controls.Add(estado);

        Shown += (_, _) => Ui.Ejecutar(this, () => Abrir(() => new FrmEstablecimientos(), inicio));
        FormClosed += (_, _) => Ui.Ejecutar(this, () => new AuthService().CerrarSesion());
    }

    // Cada sección ocupa toda el área de trabajo; al cambiar de sección se cierra la anterior.
    private void Abrir(Func<Form> crear, ToolStripMenuItem? origen = null)
    {
        var nuevo = crear(); var existente = MdiChildren.FirstOrDefault(f => f.GetType() == nuevo.GetType());
        if (existente != null) { nuevo.Dispose(); existente.Activate(); Resaltar(origen); return; }
        foreach (var anterior in MdiChildren) anterior.Close();
        nuevo.MdiParent = this; nuevo.FormBorderStyle = FormBorderStyle.None; nuevo.ControlBox = false; nuevo.Dock = DockStyle.Fill; nuevo.Show();
        Resaltar(origen);
    }

    private void Resaltar(ToolStripMenuItem? origen) { seccionActiva = origen; menu.Invalidate(); }
}

public class FrmCambiarPassword : FrmEdicion
{
    public FrmCambiarPassword() : base("Cambiar contraseña")
    {
        var actual = Texto("Contraseña actual", clave: true); var nueva = Texto("Nueva (mínimo 6)", clave: true); var confirmar = Texto("Confirmar nueva", clave: true);
        AlGuardar(() => { if (nueva.Text != confirmar.Text) throw new ArgumentException("Las contraseñas no coinciden."); new AuthService().CambiarPasswordPropia(actual.Text, nueva.Text); });
    }
}
