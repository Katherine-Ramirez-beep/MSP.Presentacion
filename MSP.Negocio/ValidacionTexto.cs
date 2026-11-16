namespace MSP.Negocio;
internal static class ValidacionTexto
{
    internal static void Longitud(string? valor, int maximo, string campo)
    { if (valor?.Length > maximo) throw new ArgumentException($"{campo}: máximo {maximo} caracteres."); }
}
