using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using MSP.Entidades;
namespace MSP.Negocio;

public static class ExportadorBitacoraService
{
    private static readonly string[] Cabeceras = { "ID", "Fecha y hora", "Usuario", "Rol", "Provincia ID", "Módulo", "Acción", "Descripción" };
    private static string[] Fila(BitacoraRegistro r) => new[] { r.BitacoraId.ToString(), r.FechaHora.ToString("yyyy-MM-dd HH:mm:ss"), r.NombreUsuario, r.Rol ?? "", r.ProvinciaId?.ToString() ?? "Nacional", r.Modulo, r.Accion, r.Descripcion };
    public static void Exportar(List<BitacoraRegistro> registros, string rutaArchivo, string formato)
    {
        FabricaEstrategiaRol.ObtenerEstrategiaActual();
        if (!TenantContext.Actual.EsAdminNacional && registros.Any(r => r.ProvinciaId != TenantContext.Actual.TenantProvinciaId)) throw new UnauthorizedAccessException("La exportación contiene registros de otra provincia.");
        var filas = new[] { Cabeceras }.Concat(registros.Select(Fila)).ToList();
        switch (formato.ToUpperInvariant())
        {
            case "EXCEL": Excel(filas, rutaArchivo); break;
            case "WORD": Word(filas, rutaArchivo); break;
            case "PDF": Pdf(registros, rutaArchivo); break;
            case "TXT": File.WriteAllLines(rutaArchivo, new[] { "MINISTERIO DE SALUD PÚBLICA · BITÁCORA", TenantContext.Actual.DescripcionAlcance }.Concat(filas.Select(f => string.Join(" | ", f))), new UTF8Encoding(true)); break;
            default: throw new ArgumentException("Formato no compatible.");
        }
        GestorEventosSistema.Instancia.Notificar("Bitácora", "EXPORTAR", $"Exportación de {registros.Count} registros en formato {formato}.");
    }
    private static void Entrada(ZipArchive zip, string ruta, XDocument contenido)
    { using var s = zip.CreateEntry(ruta).Open(); contenido.Save(s); }
    private static XDocument Tipos(string principal, string tipo, bool excel)
    {
        XNamespace n = "http://schemas.openxmlformats.org/package/2006/content-types";
        var raiz = new XElement(n + "Types", new XElement(n + "Default", new XAttribute("Extension", "rels"), new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")), new XElement(n + "Default", new XAttribute("Extension", "xml"), new XAttribute("ContentType", "application/xml")), new XElement(n + "Override", new XAttribute("PartName", principal), new XAttribute("ContentType", tipo)));
        if (excel) raiz.Add(new XElement(n + "Override", new XAttribute("PartName", "/xl/worksheets/sheet1.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml")));
        return new XDocument(raiz);
    }
    private static XDocument Relacion(string destino, string tipo)
    { XNamespace n = "http://schemas.openxmlformats.org/package/2006/relationships"; return new XDocument(new XElement(n + "Relationships", new XElement(n + "Relationship", new XAttribute("Id", "rId1"), new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/" + tipo), new XAttribute("Target", destino)))); }
    private static void Excel(List<string[]> filas, string ruta)
    {
        if (filas.Count > 1048576) throw new InvalidOperationException("La cantidad de registros excede el límite de una hoja de Excel. Use TXT, Word o PDF.");
        using var archivo = new FileStream(ruta, FileMode.Create, FileAccess.Write); using var zip = new ZipArchive(archivo, ZipArchiveMode.Create);
        XNamespace n = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", r = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        Entrada(zip, "[Content_Types].xml", Tipos("/xl/workbook.xml", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", true));
        Entrada(zip, "_rels/.rels", Relacion("xl/workbook.xml", "officeDocument"));
        Entrada(zip, "xl/workbook.xml", new XDocument(new XElement(n + "workbook", new XAttribute(XNamespace.Xmlns + "r", r), new XElement(n + "sheets", new XElement(n + "sheet", new XAttribute("name", "Bitácora"), new XAttribute("sheetId", "1"), new XAttribute(r + "id", "rId1"))))));
        Entrada(zip, "xl/_rels/workbook.xml.rels", Relacion("worksheets/sheet1.xml", "worksheet"));
        Entrada(zip, "xl/worksheets/sheet1.xml", new XDocument(new XElement(n + "worksheet", new XElement(n + "sheetData", filas.Select(f => new XElement(n + "row", f.Select(v => new XElement(n + "c", new XAttribute("t", "inlineStr"), new XElement(n + "is", new XElement(n + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), v))))))))));
    }
    private static void Word(List<string[]> filas, string ruta)
    {
        using var archivo = new FileStream(ruta, FileMode.Create, FileAccess.Write); using var zip = new ZipArchive(archivo, ZipArchiveMode.Create); XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        XElement P(string texto) => new(w + "p", new XElement(w + "r", new XElement(w + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), texto)));
        Entrada(zip, "[Content_Types].xml", Tipos("/word/document.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", false)); Entrada(zip, "_rels/.rels", Relacion("word/document.xml", "officeDocument"));
        Entrada(zip, "word/document.xml", new XDocument(new XElement(w + "document", new XElement(w + "body", P("MSP · Bitácora de auditoría"), P(TenantContext.Actual.DescripcionAlcance), filas.Skip(1).Select(f => P(string.Join(" | ", f)))))));
    }
    // PDF multipágina sin dependencias adicionales, con caracteres latinos y sin recortar registros.
    private static void Pdf(List<BitacoraRegistro> registros, string ruta)
    {
        var lineas = new List<string> { "MSP - BITÁCORA DE AUDITORÍA", TenantContext.Actual.DescripcionAlcance, $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", "" };
        foreach (var r in registros)
        {
            string texto = string.Join(" | ", Fila(r)).Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            for (int i = 0; i < texto.Length; i += 100) lineas.Add(texto.Substring(i, Math.Min(100, texto.Length - i)));
            lineas.Add("");
        }
        var paginas = lineas.Chunk(48).ToList(); var objetos = new List<string> { "<< /Type /Catalog /Pages 2 0 R >>", "", "<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>" };
        var hijos = new List<string>();
        foreach (var pagina in paginas)
        {
            int id = objetos.Count + 1; hijos.Add($"{id} 0 R");
            objetos.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R >> >> /Contents {id + 1} 0 R >>");
            string Escape(string s) => s.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
            string contenido = "BT /F1 9 Tf 32 752 Td 14 TL\n" + string.Join("\n", pagina.Select(l => $"({Escape(l)}) Tj T*")) + $"\n(Página {hijos.Count} de {paginas.Count}) Tj\nET\n";
            objetos.Add($"<< /Length {Encoding.Latin1.GetByteCount(contenido)} >>\nstream\n{contenido}endstream");
        }
        objetos[1] = $"<< /Type /Pages /Kids [{string.Join(" ", hijos)}] /Count {paginas.Count} >>";
        using var ms = new MemoryStream(); void Escribir(string s) { var b = Encoding.Latin1.GetBytes(s); ms.Write(b); }
        Escribir("%PDF-1.4\n"); var offsets = new List<long>();
        for (int i = 0; i < objetos.Count; i++) { offsets.Add(ms.Position); Escribir($"{i + 1} 0 obj\n{objetos[i]}\nendobj\n"); }
        long xref = ms.Position; Escribir($"xref\n0 {objetos.Count + 1}\n0000000000 65535 f \n"); foreach (var o in offsets) Escribir(o.ToString("D10", CultureInfo.InvariantCulture) + " 00000 n \n");
        Escribir($"trailer << /Size {objetos.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF"); File.WriteAllBytes(ruta, ms.ToArray());
    }
}

