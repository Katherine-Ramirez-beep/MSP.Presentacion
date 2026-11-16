using System.Data;
using Microsoft.Data.SqlClient;
using MSP.Entidades;

namespace MSP.Datos
{
    public class EstablecimientoRepositorio
    {
        public List<EstablecimientoDto> Listar(int? provinciaId, int? tipoId, bool? activo, string? texto)
        {
            var lista = new List<EstablecimientoDto>();
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand("sp_Establecimientos_Listar", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@ProvinciaId", (object?)provinciaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoId", (object?)tipoId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Activo", (object?)activo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Texto", string.IsNullOrWhiteSpace(texto) ? DBNull.Value : texto.Trim());

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new EstablecimientoDto
                {
                    EstablecimientoId = Convert.ToInt32(r["EstablecimientoId"]),
                    Codigo = r["Codigo"].ToString()!,
                    Nombre = r["Nombre"].ToString()!,
                    ProvinciaId = Convert.ToInt32(r["ProvinciaId"]),
                    Provincia = r["Provincia"].ToString()!,
                    RegionSalud = r["RegionSalud"].ToString()!,
                    TipoEstablecimientoId = Convert.ToInt32(r["TipoEstablecimientoId"]),
                    TipoEstablecimiento = r["TipoEstablecimiento"].ToString()!,
                    Direccion = r["Direccion"].ToString()!,
                    Municipio = r["Municipio"].ToString()!,
                    Telefono = r["Telefono"].ToString()!,
                    Activo = Convert.ToBoolean(r["Activo"])
                });
            }
            return lista;
        }

        public bool ExisteCodigo(string codigo, int? excluirId = null)
        {
            const string sql = @"
                SELECT COUNT(1) FROM Establecimientos 
                WHERE Codigo = @Codigo AND Activo = 1 AND (@ExcluirId IS NULL OR EstablecimientoId <> @ExcluirId);";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Codigo", codigo.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@ExcluirId", (object?)excluirId ?? DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public void Insertar(Establecimiento e)
        {
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand("sp_Establecimientos_Insertar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Codigo", e.Codigo);
            cmd.Parameters.AddWithValue("@Nombre", e.Nombre);
            cmd.Parameters.AddWithValue("@ProvinciaId", e.ProvinciaId);
            cmd.Parameters.AddWithValue("@TipoId", e.TipoEstablecimientoId);
            cmd.Parameters.AddWithValue("@Direccion", e.Direccion);
            cmd.Parameters.AddWithValue("@Municipio", e.Municipio);
            cmd.Parameters.AddWithValue("@Telefono", (object?)e.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UsuarioId", e.UsuarioCreacionId);
            cmd.ExecuteNonQuery();
        }

        public void Actualizar(Establecimiento e)
        {
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand("sp_Establecimientos_Actualizar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", e.EstablecimientoId);
            cmd.Parameters.AddWithValue("@Codigo", e.Codigo);
            cmd.Parameters.AddWithValue("@Nombre", e.Nombre);
            cmd.Parameters.AddWithValue("@ProvinciaId", e.ProvinciaId);
            cmd.Parameters.AddWithValue("@TipoId", e.TipoEstablecimientoId);
            cmd.Parameters.AddWithValue("@Direccion", e.Direccion);
            cmd.Parameters.AddWithValue("@Municipio", e.Municipio);
            cmd.Parameters.AddWithValue("@Telefono", (object?)e.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Activo", e.Activo);
            cmd.Parameters.AddWithValue("@UsuarioId", e.UsuarioModificacionId);
            cmd.ExecuteNonQuery();
        }

        public List<ConsolidadoProvinciaDto> ObtenerConsolidado(int? provinciaIdFiltro)
        {
            var lista = new List<ConsolidadoProvinciaDto>();
            const string sql = @"
                SELECT * FROM vw_ConsolidadoNacional 
                WHERE (@ProvinciaId IS NULL OR ProvinciaId = @ProvinciaId)
                ORDER BY TotalEstablecimientos DESC, Provincia ASC;";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProvinciaId", (object?)provinciaIdFiltro ?? DBNull.Value);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new ConsolidadoProvinciaDto
                {
                    ProvinciaId = Convert.ToInt32(r["ProvinciaId"]),
                    CodigoProvincia = r["CodigoProvincia"].ToString()!,
                    Provincia = r["Provincia"].ToString()!,
                    RegionSalud = r["RegionSalud"].ToString()!,
                    TotalEstablecimientos = Convert.ToInt32(r["TotalEstablecimientos"]),
                    Activos = Convert.ToInt32(r["Activos"]),
                    Inactivos = Convert.ToInt32(r["Inactivos"])
                });
            }
            return lista;
        }
    }
}