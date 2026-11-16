namespace MSP.Presentacion;

public enum EstiloBoton { Primario, Secundario, Peligro }

// Paleta y tipografía tomadas del formulario de acceso (FrmLogin) para que toda la aplicación comparta la misma identidad.
internal static class Tema
{
    public static readonly Color Primario = Color.FromArgb(13, 92, 99);
    public static readonly Color PrimarioHover = Color.FromArgb(9, 74, 80);
    public static readonly Color PrimarioPresionado = Color.FromArgb(7, 58, 63);
    public static readonly Color Menta = Color.FromArgb(204, 251, 241);
    public static readonly Color MentaSuave = Color.FromArgb(240, 253, 250);
    public static readonly Color Fondo = Color.FromArgb(241, 245, 249);
    public static readonly Color Borde = Color.FromArgb(203, 213, 225);
    public static readonly Color Divisor = Color.FromArgb(226, 232, 240);
    public static readonly Color Texto = Color.FromArgb(30, 41, 59);
    public static readonly Color TextoSuave = Color.FromArgb(71, 85, 105);
    public static readonly Color TextoDeshabilitado = Color.FromArgb(148, 163, 184);
    public static readonly Color FilaAlterna = Color.FromArgb(248, 250, 252);
    public static readonly Color EncabezadoTabla = Color.FromArgb(231, 243, 244);
    public static readonly Color Exito = Color.FromArgb(21, 128, 61);
    public static readonly Color Aviso = Color.FromArgb(180, 83, 9);
    public static readonly Color Peligro = Color.FromArgb(185, 28, 28);

    public static readonly Font Fuente = new("Segoe UI", 10F);
    public static readonly Font FuenteEtiqueta = new("Segoe UI Semibold", 9.5F);
    public static readonly Font FuenteBoton = new("Segoe UI", 10F, FontStyle.Bold);
    public static readonly Font FuenteTabla = new("Segoe UI", 9.5F);
    public static readonly Font FuenteTablaNegrita = new("Segoe UI Semibold", 9.5F);
    public static readonly Font FuenteTitulo = new("Segoe UI", 15F, FontStyle.Bold);
    public static readonly Font FuenteMarca = new("Segoe UI", 13F, FontStyle.Bold);
    public static readonly Font FuenteUsuario = new("Segoe UI", 11F, FontStyle.Bold);
    public static readonly Font FuenteMenu = new("Segoe UI Semibold", 10F);

    // Factor de escala del sistema (los tamaños en píxeles se expresan a 96 DPI).
    public static readonly float Escala = ObtenerEscala();
    private static float ObtenerEscala()
    {
        using var g = Graphics.FromHwnd(IntPtr.Zero);
        return g.DpiX / 96f;
    }
    public static int Px(int valor) => (int)Math.Round(valor * Escala);
}

internal sealed class BotonMsp : Button
{
    public BotonMsp(EstiloBoton estilo = EstiloBoton.Secundario)
    {
        FlatStyle = FlatStyle.Flat; Cursor = Cursors.Hand; Font = Tema.FuenteBoton; UseVisualStyleBackColor = false;
        AutoSize = true; AutoSizeMode = AutoSizeMode.GrowAndShrink;
        MinimumSize = new Size(Tema.Px(96), Tema.Px(38));
        Padding = new Padding(Tema.Px(14), 0, Tema.Px(14), 0);
        Margin = new Padding(0, 0, Tema.Px(10), Tema.Px(6));
        switch (estilo)
        {
            case EstiloBoton.Primario:
                BackColor = Tema.Primario; ForeColor = Color.White;
                FlatAppearance.BorderSize = 0;
                FlatAppearance.MouseOverBackColor = Tema.PrimarioHover; FlatAppearance.MouseDownBackColor = Tema.PrimarioPresionado;
                break;
            case EstiloBoton.Peligro:
                BackColor = Tema.Peligro; ForeColor = Color.White;
                FlatAppearance.BorderSize = 0;
                FlatAppearance.MouseOverBackColor = Color.FromArgb(153, 27, 27); FlatAppearance.MouseDownBackColor = Color.FromArgb(127, 29, 29);
                break;
            default:
                BackColor = Color.White; ForeColor = Tema.Primario;
                FlatAppearance.BorderSize = 1; FlatAppearance.BorderColor = Tema.Borde;
                FlatAppearance.MouseOverBackColor = Tema.MentaSuave; FlatAppearance.MouseDownBackColor = Tema.Menta;
                break;
        }
    }
}

// Caja con borde como la del login: marco gris que se resalta en verde al recibir el foco.
internal sealed class CajaCampo : Panel
{
    public CajaCampo(Control control)
    {
        BackColor = Tema.Borde; Padding = new Padding(1);
        var fondo = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        control.Font = Tema.Fuente;
        Action aplicar = () => { };
        int interior;
        if (control is TextBox texto)
        {
            texto.BorderStyle = BorderStyle.None; texto.Dock = DockStyle.Fill; interior = texto.PreferredHeight;
            aplicar = () =>
            {
                var color = texto.ReadOnly ? Tema.Fondo : Color.White;
                texto.BackColor = color; fondo.BackColor = color;
                texto.ForeColor = texto.ReadOnly ? Tema.TextoSuave : Tema.Texto;
            };
            texto.ReadOnlyChanged += (_, _) => aplicar();
        }
        else if (control is ComboBox combo)
        {
            combo.FlatStyle = FlatStyle.Flat; combo.Dock = DockStyle.Fill; interior = combo.PreferredHeight;
            aplicar = () =>
            {
                var color = combo.Enabled ? Color.White : Tema.Fondo;
                combo.BackColor = color; fondo.BackColor = color; combo.ForeColor = Tema.Texto;
            };
            combo.EnabledChanged += (_, _) => aplicar();
        }
        else { control.Dock = DockStyle.Fill; interior = control.PreferredSize.Height; }

        int alto = Math.Max(Tema.Px(38), interior + Tema.Px(12));
        Height = alto;
        fondo.Padding = new Padding(Tema.Px(10), Math.Max(0, (alto - 2 - interior) / 2), Tema.Px(8), 0);
        fondo.Controls.Add(control); Controls.Add(fondo); aplicar();
        fondo.Click += (_, _) => control.Focus();
        control.Enter += (_, _) => BackColor = Tema.Primario;
        control.Leave += (_, _) => BackColor = Tema.Borde;
    }
}

internal sealed class TablaMsp : DataGridView
{
    public TablaMsp()
    {
        DoubleBuffered = true;
        Dock = DockStyle.Fill; ReadOnly = true; AllowUserToAddRows = false; AllowUserToDeleteRows = false; AllowUserToResizeRows = false;
        AutoGenerateColumns = false; SelectionMode = DataGridViewSelectionMode.FullRowSelect; MultiSelect = false;
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; RowHeadersVisible = false;
        BackgroundColor = Color.White; BorderStyle = BorderStyle.None; GridColor = Tema.Divisor;
        CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        EnableHeadersVisualStyles = false; ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing; ColumnHeadersHeight = Tema.Px(42);
        ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Tema.EncabezadoTabla, ForeColor = Tema.Primario, Font = Tema.FuenteTablaNegrita,
            SelectionBackColor = Tema.EncabezadoTabla, SelectionForeColor = Tema.Primario,
            Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(Tema.Px(8), 0, Tema.Px(8), 0)
        };
        DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.White, ForeColor = Tema.Texto, Font = Tema.FuenteTabla,
            SelectionBackColor = Tema.Menta, SelectionForeColor = Tema.Texto,
            Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(Tema.Px(8), 0, Tema.Px(8), 0)
        };
        AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Tema.FilaAlterna };
        RowTemplate.Height = Tema.Px(36);
    }
}

internal sealed class ColoresMsp : ProfessionalColorTable
{
    public override Color MenuStripGradientBegin => Tema.PrimarioHover;
    public override Color MenuStripGradientEnd => Tema.PrimarioHover;
    public override Color StatusStripGradientBegin => Color.White;
    public override Color StatusStripGradientEnd => Color.White;
    public override Color ToolStripDropDownBackground => Color.White;
    public override Color ImageMarginGradientBegin => Color.White;
    public override Color ImageMarginGradientMiddle => Color.White;
    public override Color ImageMarginGradientEnd => Color.White;
    public override Color MenuBorder => Tema.Borde;
    public override Color SeparatorDark => Tema.Divisor;
    public override Color SeparatorLight => Tema.Divisor;
}

// Pinta el menú con la identidad del login: barra verde oscuro, sección activa subrayada en menta y desplegables claros.
internal sealed class RenderizadorMsp : ToolStripProfessionalRenderer
{
    private readonly Func<ToolStripItem, bool> esActivo;

    public RenderizadorMsp(Func<ToolStripItem, bool>? esActivo = null) : base(new ColoresMsp())
    {
        this.esActivo = esActivo ?? (_ => false);
        RoundedEdges = false;
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        var item = e.Item; var area = new Rectangle(Point.Empty, item.Size);
        if (!item.IsOnDropDown)
        {
            bool activo = esActivo(item);
            Color? fondo = null;
            if (item.Pressed || activo) fondo = Tema.PrimarioPresionado;
            else if (item.Selected) fondo = Tema.Primario;
            if (fondo.HasValue) { using var pincel = new SolidBrush(fondo.Value); e.Graphics.FillRectangle(pincel, area); }
            if (activo) { using var linea = new SolidBrush(Tema.Menta); e.Graphics.FillRectangle(linea, 0, area.Height - Tema.Px(3), area.Width, Tema.Px(3)); }
        }
        else if (item.Selected && item.Enabled)
        {
            using var pincel = new SolidBrush(Tema.Menta);
            e.Graphics.FillRectangle(pincel, new Rectangle(Tema.Px(4), 1, area.Width - Tema.Px(8), area.Height - 2));
        }
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (e.Item is ToolStripMenuItem)
            e.TextColor = !e.Item.Enabled ? Tema.TextoDeshabilitado : e.Item.IsOnDropDown ? Tema.Texto : Color.White;
        base.OnRenderItemText(e);
    }
}
