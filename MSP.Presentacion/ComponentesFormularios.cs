using MSP.Entidades;
using MSP.Negocio;

namespace MSP.Presentacion;

// Componentes compartidos de presentación; el acceso a datos permanece en los servicios.
public class FrmListado : Form
{
    protected readonly DataGridView Tabla = new TablaMsp();
    protected readonly FlowLayoutPanel Acciones = new()
    {
        Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = true, BackColor = Color.White,
        Padding = new Padding(Tema.Px(28), Tema.Px(16), Tema.Px(18), Tema.Px(6))
    };
    protected readonly Label Estado = new()
    {
        Dock = DockStyle.Fill, AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, Font = Tema.FuenteEtiqueta, ForeColor = Tema.TextoSuave,
        Padding = new Padding(Tema.Px(28), 0, Tema.Px(28), 0)
    };

    public FrmListado(string titulo)
    {
        Text = titulo; Font = Tema.Fuente; BackColor = Tema.Fondo;
        Size = new Size(Tema.Px(1100), Tema.Px(650)); MinimumSize = new Size(Tema.Px(800), Tema.Px(480)); StartPosition = FormStartPosition.CenterParent;

        var encabezado = Ui.Encabezado(titulo, TenantContext.Actual.DescripcionAlcance, Tema.Px(84));
        var tarjeta = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Borde, Padding = new Padding(1) };
        tarjeta.Controls.Add(Tabla);
        var contenido = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Fondo, Padding = new Padding(Tema.Px(28), Tema.Px(22), Tema.Px(28), Tema.Px(22)) };
        contenido.Controls.Add(tarjeta);
        var lineaAcciones = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Tema.Borde };
        var pie = new Panel { Dock = DockStyle.Bottom, Height = Tema.Px(40), BackColor = Color.White };
        pie.Controls.Add(Estado); pie.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Tema.Borde });

        Controls.Add(contenido); Controls.Add(lineaAcciones); Controls.Add(Acciones); Controls.Add(encabezado); Controls.Add(pie);

        Tabla.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0 && e.ColumnIndex >= 0) MessageBox.Show(this, Convert.ToString(Tabla[e.ColumnIndex, e.RowIndex].Value), Tabla.Columns[e.ColumnIndex].HeaderText, MessageBoxButtons.OK, MessageBoxIcon.Information); };
        Tabla.DataBindingComplete += (_, _) => Estado.Text = $"{Tabla.Rows.Count} registros";
        Tabla.CellFormatting += FormatearCelda;
    }

    // Valores lógicos como Sí/No con color, y estados/riesgos con un color que facilita el escaneo visual.
    private void FormatearCelda(object? emisor, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.CellStyle == null) return;
        string propiedad = Tabla.Columns[e.ColumnIndex].DataPropertyName;
        if (e.Value is bool valor)
        {
            e.Value = valor ? "Sí" : "No"; e.FormattingApplied = true;
            e.CellStyle.Font = Tema.FuenteTablaNegrita;
            e.CellStyle.ForeColor = propiedad switch
            {
                "Bloqueado" => valor ? Tema.Peligro : Tema.TextoSuave,
                "Resuelta" => valor ? Tema.Exito : Tema.Aviso,
                _ => valor ? Tema.Exito : Tema.Peligro
            };
        }
        else if (e.Value is string texto)
        {
            Color? color = texto switch
            {
                "Activo" => Tema.Exito,
                "Inactivo" or "Critico" => Tema.Peligro,
                "Advertencia" => Tema.Aviso,
                "Informativo" => Tema.Primario,
                _ => null
            };
            if (color.HasValue) { e.CellStyle.ForeColor = color.Value; e.CellStyle.Font = Tema.FuenteTablaNegrita; }
        }
    }

    protected void Col(string propiedad, string titulo, int peso = 100)
    {
        var columna = new DataGridViewTextBoxColumn { DataPropertyName = propiedad, HeaderText = titulo, FillWeight = peso, MinimumWidth = Tema.Px(75) };
        if (propiedad is "FechaHora" or "UltimoAcceso") { columna.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss"; columna.MinimumWidth = Tema.Px(165); }
        if (propiedad is "TotalEstablecimientos" or "Activos" or "Inactivos" or "IntentosFallidos" or "BitacoraId" or "ProvinciaId")
        { columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; columna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter; }
        Tabla.Columns.Add(columna);
    }
    protected Button Boton(string texto, Action accion, bool visible = true, EstiloBoton estilo = EstiloBoton.Secundario)
    {
        var b = new BotonMsp(estilo) { Text = texto, Visible = visible };
        b.Click += (_, _) => Ejecutar(accion); Acciones.Controls.Add(b); return b;
    }
    protected T Seleccion<T>() where T : class => Tabla.CurrentRow?.DataBoundItem as T ?? throw new InvalidOperationException("Seleccione un registro.");
    protected void Ejecutar(Action accion) => Ui.Ejecutar(this, accion);
    protected bool Confirmar(string texto) => MessageBox.Show(this, texto, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
}

internal static class Ui
{
    internal static void Ejecutar(IWin32Window owner, Action accion)
    {
        try { accion(); }
        catch (Exception ex) { MessageBox.Show(owner, ex.Message, "MSP · Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
    internal static ComboBox Combo() => new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 240 };
    internal static void Provincias(ComboBox combo, bool todas = false)
    {
        var datos = new AdministracionService().ListarProvincias();
        if (!TenantContext.Actual.EsAdminNacional) datos = datos.Where(p => p.ProvinciaId == TenantContext.Actual.TenantProvinciaId).ToList();
        if (todas && TenantContext.Actual.EsAdminNacional) datos.Insert(0, new Provincia { Nombre = "Todas las provincias" });
        combo.DisplayMember = "Nombre"; combo.ValueMember = "ProvinciaId"; combo.DataSource = datos;
        combo.Enabled = TenantContext.Actual.EsAdminNacional;
    }
    internal static int Id(ComboBox c) => c.SelectedValue is int id ? id : 0;

    // Envuelve un TextBox o ComboBox con el borde del login (para barras de filtros).
    internal static Control Caja(Control control, int ancho) =>
        new CajaCampo(control) { Width = Tema.Px(ancho), Margin = new Padding(0, 0, Tema.Px(10), Tema.Px(6)) };

    // Encabezado verde con línea de acento menta, igual al del login.
    internal static Panel Encabezado(string titulo, string subtitulo, int alto)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = alto, BackColor = Tema.Primario };
        var margen = new Padding(Tema.Px(28), 0, Tema.Px(28), 0);
        var acento = new Panel { Dock = DockStyle.Bottom, Height = Tema.Px(4), BackColor = Tema.Menta };
        var detalle = new Label { Dock = DockStyle.Fill, Text = subtitulo, Font = Tema.Fuente, ForeColor = Tema.Menta, TextAlign = ContentAlignment.TopLeft, Padding = margen, AutoEllipsis = true };
        var principal = new Label { Dock = DockStyle.Top, Height = alto - Tema.Px(4) - Tema.Px(30), Text = titulo, Font = Tema.FuenteTitulo, ForeColor = Color.White, TextAlign = ContentAlignment.BottomLeft, Padding = margen, AutoEllipsis = true };
        panel.Controls.Add(detalle); panel.Controls.Add(principal); panel.Controls.Add(acento);
        return panel;
    }
}

public class FrmEdicion : Form
{
    private readonly TableLayoutPanel campos = new()
    {
        Dock = DockStyle.Fill, ColumnCount = 1, AutoScroll = true, BackColor = Color.White,
        Padding = new Padding(Tema.Px(28), Tema.Px(22), Tema.Px(28), Tema.Px(8))
    };
    private readonly Panel cabecera;
    private readonly Panel pie = new() { Dock = DockStyle.Bottom, Height = Tema.Px(68), BackColor = Tema.Fondo };
    protected readonly Button Guardar = new BotonMsp(EstiloBoton.Primario) { Text = "Guardar" };

    public FrmEdicion(string titulo)
    {
        Text = "MSP · " + titulo; Font = Tema.Fuente; BackColor = Tema.Fondo;
        FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; ShowInTaskbar = false;
        ClientSize = new Size(Tema.Px(540), Tema.Px(420)); StartPosition = FormStartPosition.CenterParent;
        campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        cabecera = Ui.Encabezado(titulo, "Complete los datos y presione Guardar.", Tema.Px(80));
        var interior = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        interior.Controls.Add(campos); interior.Controls.Add(new Panel { Dock = DockStyle.Top, Height = Tema.Px(4), BackColor = Tema.Primario });
        var tarjeta = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Borde, Padding = new Padding(1) };
        tarjeta.Controls.Add(interior);
        var cuerpo = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Fondo, Padding = new Padding(Tema.Px(24), Tema.Px(24), Tema.Px(24), 0) };
        cuerpo.Controls.Add(tarjeta);

        var cancelar = new BotonMsp { Text = "Cancelar", DialogResult = DialogResult.Cancel };
        var botones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(Tema.Px(24), Tema.Px(14), Tema.Px(14), 0) };
        botones.Controls.Add(Guardar); botones.Controls.Add(cancelar); pie.Controls.Add(botones);

        Controls.Add(cuerpo); Controls.Add(pie); Controls.Add(cabecera);
        AcceptButton = Guardar; CancelButton = cancelar;
    }

    // Ajusta el alto del diálogo a la cantidad de campos antes de centrarlo.
    protected override void OnLoad(EventArgs e)
    {
        int ancho = ClientSize.Width - Tema.Px(24) * 2 - 2;
        int altoCampos = campos.GetPreferredSize(new Size(ancho, 0)).Height;
        int total = cabecera.Height + pie.Height + Tema.Px(24) + 2 + Tema.Px(4) + altoCampos;
        int maximo = Screen.FromControl(this).WorkingArea.Height - Tema.Px(60);
        ClientSize = new Size(ClientSize.Width, Math.Max(Tema.Px(300), Math.Min(total, maximo)));
        base.OnLoad(e);
    }

    protected void Campo(string titulo, Control control)
    {
        int fila = campos.RowCount; campos.RowCount += 2;
        campos.RowStyles.Add(new RowStyle(SizeType.AutoSize)); campos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        campos.Controls.Add(new Label { Text = titulo, AutoSize = true, Font = Tema.FuenteEtiqueta, ForeColor = Tema.TextoSuave, Anchor = AnchorStyles.Left, Margin = new Padding(0, 0, 0, Tema.Px(5)) }, 0, fila);
        if (control is CheckBox casilla)
        {
            casilla.AutoSize = true; casilla.Font = Tema.Fuente; casilla.ForeColor = Tema.Texto; casilla.Anchor = AnchorStyles.Left;
            casilla.Margin = new Padding(0, Tema.Px(2), 0, Tema.Px(16)); campos.Controls.Add(casilla, 0, fila + 1);
        }
        else
        {
            var caja = new CajaCampo(control) { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, Tema.Px(16)) };
            campos.Controls.Add(caja, 0, fila + 1);
        }
    }
    protected TextBox Texto(string titulo, string valor = "", bool clave = false)
    { var t = new TextBox { Text = valor, UseSystemPasswordChar = clave }; Campo(titulo, t); return t; }
    protected void AlGuardar(Action accion) => Guardar.Click += (_, _) => Ui.Ejecutar(this, () => { accion(); DialogResult = DialogResult.OK; Close(); });
}
