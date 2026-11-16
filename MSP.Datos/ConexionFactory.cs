using System.Data;
using Microsoft.Data.SqlClient;
using MSP.Entidades;

namespace MSP.Datos
{
    public sealed class ConexionFactory
    {
        private static readonly Lazy<ConexionFactory> _instancia = new(() => new ConexionFactory());
        public static ConexionFactory Instancia => _instancia.Value;

        private readonly string _connectionString;

        private ConexionFactory()
        {
            _connectionString = Environment.GetEnvironmentVariable("MSP_CONNECTION_STRING") ?? "Data Source=.;Initial Catalog=MSP_CentralizacionMultiTenant;Integrated Security=True;Trust Server Certificate=True;Connect Timeout=10;";
        }

        public SqlConnection CrearConexion()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Siempre reiniciar el contexto al obtener una conexión del pool.
            {
                using (var cmd = new SqlCommand("sp_set_session_context", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@key", "IsAdminNacional");
                    cmd.Parameters.AddWithValue("@value", TenantContext.Actual.EsAdminNacional ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand("sp_set_session_context", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@key", "TenantProvinciaId");
                    cmd.Parameters.AddWithValue("@value", (object?)TenantContext.Actual.TenantProvinciaId ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            return conn;
        }
    }
}

