using System.Data;
using Microsoft.Data.SqlClient;
using MSP.Entidades;
using Microsoft.Data.SqlClient;

namespace MSP.Datos
{
    public sealed class ConexionFactory
    {
        private static readonly Lazy<ConexionFactory> _instancia = new(() => new ConexionFactory());
        public static ConexionFactory Instancia => _instancia.Value;

        private readonly string _connectionString;

        private ConexionFactory()
        {
            _connectionString = "Data Source=DESKTOP-QJNQ9RN\\KATHRASHEL;Initial Catalog=MSP_CentralizacionMultiTenant;User ID=sa;Password=Mytaetae25uwu;Trust Server Certificate=True;";
        }

        public SqlConnection CrearConexion()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();

            if (TenantContext.Actual.EstaAutenticado)
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
