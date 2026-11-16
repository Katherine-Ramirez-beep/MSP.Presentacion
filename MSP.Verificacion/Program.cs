using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MSP.Entidades;
using MSP.Negocio;
using MSP.Presentacion;
internal static class Program
{
    static int total;
    static void Check(bool valor, string nombre) { if (!valor) throw new Exception(nombre); total++; Console.WriteLine("OK " + nombre); }
    static void Denegado(Action a, string nombre) { try { a(); } catch (UnauthorizedAccessException) { Check(true, nombre); return; } throw new Exception(nombre); }
    [STAThread] static void Main(string[] args)
    {
        string salida = Path.GetFullPath(args.FirstOrDefault() ?? "resultados"); Directory.CreateDirectory(salida);
        ApplicationConfiguration.Initialize();
        TenantContext.Actual.CerrarSesion(); Denegado(() => FabricaEstrategiaRol.ObtenerEstrategiaActual(), "Sin sesión no obtiene estrategia");
        foreach (string rol in new[] { "AdminNacional", "AdminProvincial", "Consultor" })
        {
            TenantContext.Actual.IniciarSesion(new Usuario { UsuarioId = 1, Rol = rol, ProvinciaId = rol == "AdminNacional" ? null : 7, NombreCompleto = "Usuario de prueba", NombreProvincia = "Provincia de prueba" });
            var p = FabricaEstrategiaRol.ObtenerEstrategiaActual();
            Check(p.ResolverFiltroProvincia(99) == (rol == "AdminNacional" ? 99 : 7), rol + " aplica filtro provincial");
            using var principal = new FrmPrincipal(); var menu = principal.MainMenuStrip!;
            Check(menu.Items.Cast<ToolStripItem>().Single(x => x.Text == "Usuarios").Available == (rol != "Consultor"), rol + " menú Usuarios");
            Check(menu.Items.Cast<ToolStripItem>().Single(x => x.Text == "Provincias").Available == (rol == "AdminNacional"), rol + " menú Provincias");
            Check(menu.Items.Cast<ToolStripItem>().Single(x => x.Text == "Tipos de Centros").Available == (rol == "AdminNacional"), rol + " menú Tipos");
            Check(menu.Items.Cast<ToolStripItem>().Single(x => x.Text == "Mi Sesión").Alignment == ToolStripItemAlignment.Right, "Mi Sesión a la derecha");
            if (rol == "Consultor")
            {
                Denegado(() => new AdministracionService().ListarUsuarios(), "Consultor no consulta cuentas");
                Denegado(() => new AdministracionService().CrearUsuario("A", "a", "abcdef", "Consultor", 7), "Consultor no crea cuentas");
                Denegado(() => new EstablecimientoService().Guardar(new Establecimiento()), "Consultor no guarda centros");
                Denegado(() => new AdministracionService().ResolverAlerta(1), "Consultor no resuelve alertas");
            }
            if (rol != "AdminNacional") Denegado(() => new FrmProvincias(), "Catálogo nacional protegido");
        }
        TenantContext.Actual.IniciarSesion(new Usuario { Rol = "AdminNacional" });
        var registros = Enumerable.Range(1, 251).Select(i => new BitacoraRegistro { BitacoraId = i, NombreUsuario = "prueba", Rol = "Consultor", ProvinciaId = 7, FechaHora = new DateTime(2026, 11, 16), Modulo = "Auditoría", Accion = "PRUEBA", Descripcion = $"Registro {i}: áéíóú ñ & <texto> =SUM(A1) " + new string('x', 250) }).ToList();
        var tipo = typeof(ExportadorBitacoraService);
        var filas = new List<string[]> { new[] { "ID", "Fecha", "Usuario", "Rol", "Provincia", "Módulo", "Acción", "Descripción" } };
        var fila = tipo.GetMethod("Fila", BindingFlags.Static | BindingFlags.NonPublic)!;
        filas.AddRange(registros.Select(r => (string[])fila.Invoke(null, new object[] { r })!));
        foreach (var par in new[] { ("Excel", "xlsx"), ("Word", "docx"), ("Pdf", "pdf") })
        {
            var metodo = tipo.GetMethod(par.Item1, BindingFlags.Static | BindingFlags.NonPublic)!;
            string ruta = Path.Combine(salida, "bitacora." + par.Item2);
            object datos = par.Item1 == "Pdf" ? registros : filas;
            metodo.Invoke(null, new[] { datos, ruta }); metodo.Invoke(null, new[] { datos, ruta });
            Check(new FileInfo(ruta).Length > 0, par.Item1 + " genera y sobrescribe archivo");
            if (par.Item1 != "Pdf")
            {
                using var zip = ZipFile.OpenRead(ruta);
                foreach (var entry in zip.Entries) { using var stream = entry.Open(); XDocument.Load(stream); }
                using var texto = new StreamReader(zip.GetEntry(par.Item1 == "Excel" ? "xl/worksheets/sheet1.xml" : "word/document.xml")!.Open());
                string xml = texto.ReadToEnd(); Check(xml.Contains("Registro 251:"), par.Item1 + " conserva registros posteriores a 200");
                Check(xml.Contains("áéíóú ñ"), par.Item1 + " conserva acentos");
            }
            else { string pdf = Encoding.Latin1.GetString(File.ReadAllBytes(ruta)); Check(pdf.Contains("Registro 251:"), "PDF no recorta registros"); Check(pdf.Contains("Página 2 de"), "PDF multipágina"); }
        }
        using (var bitacora = new VistaBitacora())
        {
            var tabla = (DataGridView)typeof(FrmListado).GetField("Tabla", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(bitacora)!;
            tabla.DataSource = registros; bitacora.Show(); Application.DoEvents();
            using var imagen = new Bitmap(bitacora.Width, bitacora.Height); bitacora.DrawToBitmap(imagen, new Rectangle(Point.Empty, bitacora.Size)); imagen.Save(Path.Combine(salida, "bitacora.png"));
            Check(tabla.Columns.Count == 8, "Bitácora muestra columnas explícitas");
        }
        TenantContext.Actual.CerrarSesion(); Console.WriteLine($"TOTAL: {total} verificaciones correctas. Las pruebas no requieren ni modifican SQL Server.");
    }
}

internal class VistaBitacora : FrmBitacora { protected override void OnShown(EventArgs e) { } }


