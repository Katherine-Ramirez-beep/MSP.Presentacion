using System.Data;
using Microsoft.Data.SqlClient;
using MSP.Entidades;

namespace MSP.Datos
{
    public class AuditoriaRepositorio
    {
        public void RegistrarBitacora(BitacoraRegistro reg)
        {
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand("sp_Bitacora_Insertar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UsuarioId", (object?)reg.UsuarioId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NombreUsuario", reg.NombreUsuario);
            cmd.Parameters.AddWithValue("@Rol", (object?)reg.Rol ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ProvinciaId", (object?)reg.ProvinciaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Modulo", reg.Modulo);
            cmd.Parameters.AddWithValue("@Accion", reg.Accion);
            cmd.Parameters.AddWithValue("@Descripcion", reg.Descripcion);
            cmd.ExecuteNonQuery();
        }

        public List<BitacoraRegistro> ListarBitacora(int? provinciaIdFiltro)
        {
            var lista = new List<BitacoraRegistro>();
            const string sql = @"
                SELECT BitacoraId, FechaHora, UsuarioId, NombreUsuario, Rol, ProvinciaId, Modulo, Accion, Descripcion
                FROM Bitacora
                WHERE (@ProvinciaId IS NULL OR ProvinciaId = @ProvinciaId)
                ORDER BY FechaHora DESC;";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProvinciaId", (object?)provinciaIdFiltro ?? DBNull.Value);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new BitacoraRegistro
                {
                    BitacoraId = r.GetInt64(0),
                    FechaHora = r.GetDateTime(1),
                    UsuarioId = r.IsDBNull(2) ? null : r.GetInt32(2),
                    NombreUsuario = r.GetString(3),
                    Rol = r.IsDBNull(4) ? "N/A" : r.GetString(4),
                    ProvinciaId = r.IsDBNull(5) ? null : r.GetInt32(5),
                    Modulo = r.GetString(6),
                    Accion = r.GetString(7),
                    Descripcion = r.GetString(8)
                });
            }
            return lista;
        }

        public void RegistrarAlertaSeguridad(AlertaSeguridadTenant alerta)
        {
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand("sp_Alertas_Insertar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UsuarioOrigen", alerta.UsuarioOrigen);
            cmd.Parameters.AddWithValue("@ProvinciaInvolucradaId", (object?)alerta.ProvinciaInvolucradaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoriaAlerta", alerta.CategoriaAlerta);
            cmd.Parameters.AddWithValue("@NivelRiesgo", alerta.NivelRiesgo);
            cmd.Parameters.AddWithValue("@MensajeTecnico", alerta.MensajeTecnico);
            cmd.ExecuteNonQuery();
        }

        public List<AlertaSeguridadTenant> ListarAlertas(int? provinciaIdFiltro)
        {
            var lista = new List<AlertaSeguridadTenant>();
            const string sql = @"
                SELECT a.AlertaId, a.FechaHora, a.UsuarioOrigen, a.ProvinciaInvolucradaId,
                       ISNULL(p.Nombre, N'Global / Nacional') AS NombreProvincia,
                       a.CategoriaAlerta, a.NivelRiesgo, a.MensajeTecnico, a.Resuelta
                FROM AlertasSeguridadTenant a
                LEFT JOIN Provincias p ON a.ProvinciaInvolucradaId = p.ProvinciaId
                WHERE (@ProvinciaId IS NULL OR a.ProvinciaInvolucradaId = @ProvinciaId)
                ORDER BY a.FechaHora DESC;";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProvinciaId", (object?)provinciaIdFiltro ?? DBNull.Value);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new AlertaSeguridadTenant
                {
                    AlertaId = r.GetInt64(0),
                    FechaHora = r.GetDateTime(1),
                    UsuarioOrigen = r.GetString(2),
                    ProvinciaInvolucradaId = r.IsDBNull(3) ? null : r.GetInt32(3),
                    NombreProvincia = r.GetString(4),
                    CategoriaAlerta = r.GetString(5),
                    NivelRiesgo = r.GetString(6),
                    MensajeTecnico = r.GetString(7),
                    Resuelta = r.GetBoolean(8)
                });
            }
            return lista;
        }

        public void MarcarAlertaResuelta(long alertaId)
        {
            const string sql = "UPDATE AlertasSeguridadTenant SET Resuelta = 1 WHERE AlertaId = @Id;";
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", alertaId);
            cmd.ExecuteNonQuery();
        }
    }
}
